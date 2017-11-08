using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace PerfHelper.Config
{
    /// <summary>
    /// Configuration element : Collection of counter element
    /// </summary>
    [ConfigurationCollection(typeof(Counter), AddItemName = "Counter")]
    class CounterCollection : ConfigurationElementCollection
    {
        public Counter this[int index]
        {
            get
            {
                return base.BaseGet(index) as Counter;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Counter();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Counter)element).Name;
        } 
    }
}
