# PerfHelper
Simple tool to generate Windows Performance Monitor templates

This software is published under the MIT License (see LICENSE)

# Use

The tool create a perfmon template based on the configuration file. The configuration contains the template header and footer, and the list of counters to include. The template is customized and unique to each machine, as it contains only the counter that exists on each machine.

1. Open a CMD as administrator
2. Run PerfHelper and redirect output to a template xml file
`$ PerfHelper > mytemplate.xml`
3. Create a perfmon collector from the template

# Configuration

The couter selection is based on regular expression matched against the categories (some time called objects) and couters names.

This is usefull for categories where the name is not constant. For example for the SQL Server Buffer Manager category, the name  depends on the name of the instance: 
* Default instance: the categorie is named `SQLServer:Buffer Manager`
* Named instance: the categorie is named `MSSQL$<instance name>:Buffer Manager`

The regular expression `(SQLServer|MSSQL\$.*):Buffer Manager` matches all of theses case. Also, if more than one instance is installer on the machine, all instances counters will be included in the template.

The configuration file is made of 
* 6 categories group: SystemCategories, AspNetCategories, MsSqlCategories, SsasCategories, SsisCategories, SsrsCategories. These are just to organize things.
* Each categories group contains a list of categories.
* Each categorie contains 
  * a regular expression used to match a category name, 
  * an optional regular expression used to match instances name, 
  * and a list of counters
* Each counter contains a regular expression used to match a counter name

The configuration also contains the header and footer used to create the xml template.

# Dump mode
You can call `PerfHelper -dump > countersList.tsv` to get a list of all counters existing on the machine. The list is outputed as a Tab-separated values file.

As each for instance bases categories, each counter can be applyed to all instances, the dump will output counters and instances on separate lines: Some lines in the output will contains a counter, other lines will contains an instance name.

Example:

    MachineName	CategoryType	CategoryName	ItemType	ItemName	ItemType
    .	MultiInstance	Processor	Instance	0
    .	MultiInstance	Processor	Instance	1
    .	MultiInstance	Processor	Instance	_Total
    .	MultiInstance	Processor	Counter	% Processor Time	Timer100NsInverse
    .	MultiInstance	Processor	Counter	% User Time	Timer100Ns
    .	MultiInstance	Processor	Counter	% Privileged Time	Timer100Ns
    
# Requisit
 * Framework .Net 2.0 or more recent.
 * The solution is based on Visual Studio 2012. But it will probably compile with any thing capable of compiling .Net 2.0 code.

