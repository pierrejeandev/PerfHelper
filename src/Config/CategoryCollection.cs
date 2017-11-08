/*
MIT License (see LICENSE)
Copyright (c) 2017 Pierre-Jean Deville / Umanis.com
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace PerfHelper.Config
{
    /// <summary>
    /// Configuration element : Collection of category element
    /// </summary>
    [ConfigurationCollection(typeof(Category), AddItemName = "Category")]
    class CategoryCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// List of counters found on the system, matching one the categories and counters specified in this list (Not a config element, internal use only)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal Counter this[int index]
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

        /// <summary>
        /// Create a new child configuration element for this collection (Implementation of ConfigurationElementCollection)
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Category();
        }

        /// <summary>
        /// Get an existing configuration element for this collection   (Implementation of ConfigurationElementCollection)
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Category)element).Name;
        } 
    }
}
