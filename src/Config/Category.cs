/*
MIT License (see LICENSE)
Copyright (c) 2017 Pierre-Jean Deville / Umanis.com
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace PerfHelper.Config
{
    /// <summary>
    /// Configuration element to select a performance counter Category. A category contains a list of counters and optionaly a list of instances.
    /// </summary>
    class Category : ConfigurationElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Category()
        {
            this.CategoryFound = false;
            this.InstanceFound = false;
        }

        /// <summary>
        /// Regular expression that match a performance counter category
        /// </summary>
        [ConfigurationProperty("Name", IsRequired = true)]
        public String Name
        {
            get
            {
                return (String)this["Name"];
            }
        }

        /// <summary>
        /// Instance of the Regular expression that match a performance counter category
        /// </summary>
        private Regex _NameRe = null;
        /// <summary>
        /// Instance of the Regular expression that match a performance counter category
        /// </summary>
        public Regex NameRe
        {
            get
            {
                if (_NameRe == null)
                {
                    //if(this.Name.StartsWith("^") || this.Name.EndsWith("$"))
                    //    _NameRe = new Regex(this.Name);
                    //else
                        _NameRe = new Regex(String.Concat("^", this.Name, "$"));
                }
                return _NameRe;
            }
        }

        /// <summary>
        /// Regular expression that match the selected instances
        /// </summary>
        [ConfigurationProperty("Instances", IsRequired = false, DefaultValue="")]
        public String Instances
        {
            get
            {
                return (String)this["Instances"];
            }
        }

        /// <summary>
        /// Instance of the Regular expression that match an Instances
        /// </summary>
        private Regex _InstancesRe = null;
        /// <summary>
        /// Instance of the Regular expression that match an Instances
        /// </summary>
        public Regex InstancesRe
        {
            get
            {
                if (_InstancesRe == null)
                    _InstancesRe = new Regex(String.Concat("^", this.Instances, "$"));
                return _InstancesRe;
            }
        }

        /// <summary>
        /// List of counters
        /// </summary>
        [ConfigurationProperty ("Counters")]
        public CounterCollection Counters
        {
            get
            {
                return this["Counters"] as CounterCollection;
            }
        }
        
        /// <summary>
        /// Regulat expression that match the selected instances
        /// </summary>
        [ConfigurationProperty("ForceInstances", IsRequired = false, DefaultValue = false)]
        public Boolean ForceInstances
        {
            get
            {
                return (Boolean)this["ForceInstances"];
            }
        }
        

        /// <summary>
        /// Has this Category been Found in the machine category (not a config element, internal use only)
        /// </summary>
        internal Boolean CategoryFound { get; set; }
        /// <summary>
        /// Has sone Instances been Found in the machine category (not a config element, internal use only)
        /// </summary>
        internal Boolean InstanceFound { get; set; }
    }
}
