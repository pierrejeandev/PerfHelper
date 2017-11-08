using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace PerfHelper.Config
{
    /// <summary>
    /// Configuration element to select a performance counter
    /// </summary>
    class Counter : ConfigurationElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Counter()
        {
            this.Found = false;
        }

        /// <summary>
        /// Regular expression that match a performance counter
        /// </summary>
        [ConfigurationProperty("Name", IsRequired = true)]
        public String Name
        {
            get
            {
                return this["Name"] as String;
            }
        }

        /// <summary>
        /// Instance of the Regular expression that match a performance counter
        /// </summary>
        private Regex _NameRe = null;
        /// <summary>
        /// Instance of the Regular expression that match a performance counter
        /// </summary>
        public Regex NameRe
        {
            get
            {
                if (_NameRe == null)
                    _NameRe = new Regex(String.Concat("^", this.Name, "$"));
                return _NameRe;
            }
        }

        /// <summary>
        /// Have we found this counter on this machine ? (not a config element, internal use only)
        /// </summary>
        internal Boolean Found { get; set; }
    }
}
