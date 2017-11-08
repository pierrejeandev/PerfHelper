using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace PerfHelper.Config
{
    /// <summary>
    /// PerfHelper custom configuration section. 
    /// </summary>
    class PerfHelperConfig : ConfigurationSection
    {
        /// <summary>
        /// Categories and counters for general system monitoring
        /// </summary>
        [ConfigurationProperty("SystemCategories")]
        public CategoryCollection SystemCategories
        {
            get
            {
                return this["SystemCategories"] as CategoryCollection;
            }
        }

        /// <summary>
        /// Categories and counters for general SQL Server Database Engine  (MSSQL) service instances
        /// </summary>
        [ConfigurationProperty("MsSqlCategories")]
        public CategoryCollection MsSqlCategories
        {
            get
            {
                return this["MsSqlCategories"] as CategoryCollection;
            }
        }

        /// <summary>
        /// Categories and counters for general SQL Server Analisys Services (SSAS) service instances
        /// </summary>
        [ConfigurationProperty("SsasCategories")]
        public CategoryCollection SsasCategories
        {
            get
            {
                return this["SsasCategories"] as CategoryCollection;
            }
        }

        /// <summary>
        /// Categories and counters for general SQL Server Integration Services (SSIS) service instances
        /// </summary>
        [ConfigurationProperty("SsisCategories")]
        public CategoryCollection SsisCategories
        {
            get
            {
                return this["SsisCategories"] as CategoryCollection;
            }
        }

        /// <summary>
        /// Categories and counters for general SQL Server Reporting Services (SSRS) service instances
        /// </summary>
        [ConfigurationProperty("SsrsCategories")]
        public CategoryCollection SsrsCategories
        {
            get
            {
                return this["SsrsCategories"] as CategoryCollection;
            }
        }

        /// <summary>
        /// Categories and counters for general ASP.Net web application
        /// </summary>
        [ConfigurationProperty("AspNetCategories")]
        public CategoryCollection AspNetCategories
        {
            get
            {
                return this["AspNetCategories"] as CategoryCollection;
            }
        }

        /// <summary>
        /// List of instance to be forced for w3wp processes FIXME: Not implemented yet
        /// </summary>
        [ConfigurationProperty("ForceInstancesW3wp", IsRequired = false, DefaultValue = "")]
        public String ForceInstancesW3wp
        {
            get
            {
                return (String)this["ForceInstancesW3wp"];
            }
        }

        /// <summary>
        /// List of instance to be forced for DTExec processes FIXME: Not implemented yet
        /// </summary>
        [ConfigurationProperty("ForceInstancesDTExec", IsRequired = false, DefaultValue = "")]
        public String ForceInstancesDTExec
        {
            get
            {
                return (String)this["ForceInstancesDTExec"];
            }
        }

        /// <summary>
        /// Starting part of the XML template. This value is written first to the template.
        /// </summary>
        [ConfigurationProperty("XmlTemplateStart", IsRequired = true)]
        public String XmlTemplateStart
        {
            get
            {
                return (String)this["XmlTemplateStart"];
            }
        }

        /// <summary>
        /// Ending part of the XML template. This value is written last to the template.
        /// </summary>
        [ConfigurationProperty("XmlTemplateEnd", IsRequired = true)]
        public String XmlTemplateEnd
        {
            get
            {
                return (String)this["XmlTemplateEnd"];
            }
        }
    }
}
