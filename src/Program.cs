/*
MIT License (see LICENSE)
Copyright (c) 2017 Pierre-Jean Deville / Umanis.com
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace PerfHelper
{
    class Program
    {
        /// <summary>
        /// Main 
        /// </summary>
        /// <param name="args"></param>
        static void Main(String[] args)
        {
            // Check the executing user privilèges
            if (!IsUserAdministrator())
            {
                Console.Error.WriteLine("This program to be run with administrative profilèges. \"Use Run as 'Administrator' option to launch the CMD window.\"");
                return;
            }

            // Check 1st argument
            if (args.Length >= 1 && args[0] == "-dump")
            {
                // Dump mode: just print availlable counter on this system in a tabular separated text file

                // header
                Console.WriteLine("MachineName\tCategoryType\tCategoryName\tItemType\tItemName\tItemType");

                // Load system counters
                PerformanceCounterCategory[] cats = System.Diagnostics.PerformanceCounterCategory.GetCategories();

                // Loop over counters
                foreach (PerformanceCounterCategory cat in cats)
                {
                    // load instances of this category
                    String[] instances = cat.GetInstanceNames();

                    // load counter of this category
                    PerformanceCounter[] counters;
                    if (instances.Length != 0)
                    {
                        // if at least one instance exits, print the list of instances and load counters of the 1st instance
                        foreach (String instance in instances)
                        {
                            Console.WriteLine(string.Concat(cat.MachineName, "\t", cat.CategoryType, "\t", cat.CategoryName, "\tInstance\t", instance));
                        }
                        // load counters of the 1st instance
                        counters = cat.GetCounters(instances[0]);
                    }
                    else
                    {
                        // load counter for a category without instances
                        counters = cat.GetCounters();
                    }

                    // loop over counters to print counters data
                    foreach (PerformanceCounter counter in counters)
                    {
                        try
                        {
                            Console.WriteLine(string.Concat(cat.MachineName, "\t", cat.CategoryType, "\t", cat.CategoryName, "\tCounter\t", counter.CounterName, "\t", counter.CounterType));
                        }
                        catch(Exception)
                        { } // just ignore
                    }
                }
                
                return;
                // end of dump mode
            }

            // Normal mode : Create a perfmon template from the config file and the counters existing on the system
            {
                // Load configuration
                PerfHelper.Config.PerfHelperConfig conf = (PerfHelper.Config.PerfHelperConfig)ConfigurationManager.GetSection("PerfHelperConfig");

                Console.Error.WriteLine("Loading Local machine counters ...");

                // load performances counters
                PerformanceCounterCategory[] cats = System.Diagnostics.PerformanceCounterCategory.GetCategories();

                // Loca lvariable to store found counters
                List<String> formatedCounters = new List<String>();

                // Find counters for General System categories
                SelectCounter(cats, conf.SystemCategories, "", formatedCounters);
                // Find counters for SQL server instances 
                SelectCounter(cats, conf.MsSqlCategories, "", formatedCounters);
                // Find counters for SSAS
                SelectCounter(cats, conf.SsasCategories, "", formatedCounters);
                // Find counters for SSIS
                SelectCounter(cats, conf.SsisCategories, "", formatedCounters);
                // Find counters for SSRS
                SelectCounter(cats, conf.SsrsCategories, "", formatedCounters);
                // Find counters for Asp.net
                SelectCounter(cats, conf.AspNetCategories, "", formatedCounters);

                // Sort counters
                formatedCounters.Sort();

                // Write template with selected counters to console stdout
                Console.Out.Write(conf.XmlTemplateStart);
                foreach (String c in formatedCounters)
                {
                    Console.WriteLine(c);
                }
                Console.Out.Write(conf.XmlTemplateEnd);
            }
        }

        /// <summary>
        /// Select counters from a configured list of categories and instances
        /// </summary>
        /// <param name="osCategories">Performance counters from the OS</param>
        /// <param name="confCategories">List of configured categories</param>
        /// <param name="instanceName">not used</param>
        /// <param name="formatedCounters">list to output found counters, formated for the template</param>
        private static void SelectCounter(PerformanceCounterCategory[] osCategories, Config.CategoryCollection confCategories, String instanceName, List<String> formatedCounters)
        {
            // Nested loops over system counters and over configured counters
            foreach (PerformanceCounterCategory osCategorie in osCategories)
            {
                foreach (Config.Category confCategorie in confCategories)
                {
                    // If the category is selected
                    if (confCategorie.NameRe.IsMatch(osCategorie.CategoryName))
                    {
                        confCategorie.CategoryFound = true;

                        String[] instances = osCategorie.GetInstanceNames();

                        // Local pre-selected list of counters
                        List<PerformanceCounter> counters = new List<PerformanceCounter>();

                        // Load list of counters for this category: this depends on the instance name for multininstances
                        if (osCategorie.CategoryType == PerformanceCounterCategoryType.MultiInstance)
                        {
                            // Multi instance category
                            if (confCategorie.Instances == "")
                            {
                                // config issue
                                Console.Error.WriteLine(String.Concat("Incoherent configuration for category '", osCategorie.CategoryName, "' (matched by '", confCategorie.Name, "'): This category has instances, but no instances was specified in the configuration. Please set an instance name or a list of instances or '*' for all instances."));
                                Console.Error.WriteLine(String.Concat("List of instances: ", String.Join(", ", instances)));
                            }
                            else
                            {
                                foreach (String instance in instances)
                                {
                                    // Select instances
                                    if (confCategorie.InstancesRe.IsMatch(instance))
                                    {
                                        // add counter to pre-selected list
                                        counters.AddRange(osCategorie.GetCounters(instance));
                                        confCategorie.InstanceFound = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Single instance category
                            if (confCategorie.Instances != "")
                            {
                                // config issue
                                Console.Error.WriteLine(String.Concat("Incoherent configuration for category '", osCategorie.CategoryName, "' (matched by '", confCategorie.Name, "'): This category has only one instance, but an instance selector was specified: '", confCategorie.Instances, "'."));
                            }
                            else
                            {
                                confCategorie.InstanceFound = true;
                                // add counters to pre-selected list
                                counters.AddRange(osCategorie.GetCounters());
                            }
                        }

                        // Nested loop over counters pre-selected list, and over counters from the configuration
                        foreach (PerformanceCounter counter in counters)
                        {
                            foreach (Config.Counter confCounter in confCategorie.Counters)
                            {
                                // If the counter is selected in the config
                                if (confCounter.NameRe.IsMatch(counter.CounterName))
                                {
                                    // formating counter for the template
                                    if (osCategorie.CategoryType == PerformanceCounterCategoryType.MultiInstance)
                                    {
                                        // 		<Counter>\ASP.NET Applications(__Total__)\Sessions Active</Counter>
                                        formatedCounters.Add(String.Concat("		<Counter>\\",
                                            String.Concat(osCategorie.CategoryName, "(", counter.InstanceName, ")\\", counter.CounterName).Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;"),
                                            "</Counter>"));
                                    }
                                    else
                                    {
                                        // 		<Counter>\Memory\% Committed Bytes In Use</Counter>
                                        formatedCounters.Add(String.Concat("		<Counter>\\",
                                            String.Concat(osCategorie.CategoryName, "\\", counter.CounterName).Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;"),
                                            "</Counter>"));
                                    }
                                    confCounter.Found = true;
                                } // if (confCounter.NameRe.IsMatch(counter.CounterName))

                            }
                        }
                        // END of Nested loop over counters pre-selected list, and over counters from the configuration

                    } // if (confCategorie.NameRe.IsMatch(osCategorie.CategoryName))

                }
            }
            // END of Nested loops over system counters and over configured counters

            // Check for category/instances/counters not found and print output about counter tha were not founds
            foreach (Config.Category confCat in confCategories)
            {
                if (!confCat.CategoryFound)
                {
                    // Missing category
                    Console.Error.WriteLine(String.Concat("Warning: No category matching '", confCat.Name, "' has been found"));
                }
                else
                {
                    if (!confCat.InstanceFound)
                    {
                        // Found Category with no matched instances
                        Console.Error.WriteLine(String.Concat("Warning: No instances matching '", confCat.Instances, "' has been found for category '", confCat.Name, "'"));
                    }
                    else
                    {
                        // missing counters for found category with found instance
                        foreach (Config.Counter confCounter in confCat.Counters)
                        {
                            if (!confCounter.Found)
                            {
                                Console.Error.WriteLine(String.Concat("Warning: No counter matching '", confCounter.Name, "' has been found for category '", confCat.Name, "'"));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if the executing user has the required privileges. Need to be run as Administrator.
        /// </summary>
        /// <returns></returns>
        public static Boolean IsUserAdministrator()
        {
            // bool variable to hold our return value
            Boolean isAdmin = false;
            WindowsIdentity user = null;
            try
            {
                //get the currently logged in user
                user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);

                // checking if we are in the local administrators group
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
            return isAdmin;
        }


        /// <summary>
        /// Chack if IIS is installed on this machine by looking for a category named "Internet Information Services Global"
        /// </summary>
        /// <param name="cats"></param>
        /// <returns></returns>
        private static Boolean IsIISInstalled(PerformanceCounterCategory[] cats)
        {
             String categoryName = "Internet Information Services Global";
             Boolean found = false;
             foreach (PerformanceCounterCategory cat in cats)
             {
                 if (cat.CategoryName == categoryName)
                 {
                     found = true;
                     break;
                 }
             }
             return found;
        }

        /// <summary>
        /// Chack if SSIS is installed on this machine by looking for a category named "SQLServer:SSIS Service *"
        /// </summary>
        /// <param name="cats"></param>
        /// <returns></returns>
        private static Boolean IsSSISInstalled(PerformanceCounterCategory[] cats)
        {
            String categoryName = "SQLServer:SSIS Service";
            Boolean found = false;
            foreach (PerformanceCounterCategory cat in cats)
            {
                if (cat.CategoryName.StartsWith(categoryName))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}
