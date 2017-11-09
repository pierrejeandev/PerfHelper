# PerfHelper
Simple cli tool to generate Windows Performance Monitor templates customized for each machine.
The purpose is to avoid repeated manual selection of counters, which can be a source of errors.

This tool is useful when you have a limited number of servers you whish to monitor and you don't have fully featured monitoring software that include all the counters you need.

This software is published under the MIT License (see LICENSE)

# Use

The tool creates a perfmon template based on a configuration file. The configuration contains a selection of counters to include and the template created contains all counters that exists on the server and that are selected in the configuration file. This way, the template is adapted to each machine.

The configuration file included with the sources contains counters for various service from the SQL Server suite. You can modify this configuration to match more closely your needs.

1. Open a CMD as administrator
2. Run PerfHelper and redirect output to a template xml file
`$ PerfHelper > mytemplate.xml`
3. Create a perfmon collector from the template

To compile the tool from sources, see below 'Compiling without Visual Studio'.

# Configuration

The counter selection is based on regular expression matched against the categories (some time called objects) and counters names.

This is useful for categories where the name is not constant. For example for the SQL Server Buffer Manager category, the name  depends on the name of the instance: 
* Default instance: the category is named `SQLServer:Buffer Manager`
* Named instance: the category is named `MSSQL$<instance name>:Buffer Manager`

The regular expression `(SQLServer|MSSQL\$.*):Buffer Manager` matches all of theses case. Also, if more than one instance is installer on the machine, all instances counters will be included in the template.

The configuration file is made of 
* 6 categories group: SystemCategories, AspNetCategories, MsSqlCategories, SsasCategories, SsisCategories, SsrsCategories. These are just to organize things.
* Each categories group contains a list of categories.
* Each category contains 
  * a regular expression used to match a category name, 
  * an optional regular expression used to match instances name, 
  * and a list of counters
* Each counter contains a regular expression used to match a counter name

The configuration also contains the header and footer used to create the xml template.

# Dump mode
You can call `PerfHelper -dump > countersList.tsv` to get a list of all counters existing on the machine. The list is outputted as a Tab-separated values file.

As each for instance bases categories, each counter can be applied yed to all instances, the dump will output counters and instances on separate lines: Some lines in the output will contains a counter, other lines will contains an instance name.

Example:

    MachineName	CategoryType	CategoryName	ItemType	ItemName	ItemType
    .	MultiInstance	Processor	Instance	0
    .	MultiInstance	Processor	Instance	1
    .	MultiInstance	Processor	Instance	_Total
    .	MultiInstance	Processor	Counter	% Processor Time	Timer100NsInverse
    .	MultiInstance	Processor	Counter	% User Time	Timer100Ns
    .	MultiInstance	Processor	Counter	% Privileged Time	Timer100Ns
    
# Requisites
 * Framework .Net 2.0 or more recent.
 * The solution is based on Visual Studio 2012. But it will probably compile with any thing capable of compiling .Net 2.0 code.

# Compiling without Visual Studio

Instruction to compile the tool without Visual Studio nor git tooling.

1. Download source files from this page, using the `Clone or download` -> `Download ZIP` button
2. Unzip in the folder of your choice
2. Open a cmd and navigate into the src folder
3. Run `C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe  PerfHelper.sln` (change the framework version if required)

The compiled executable is found in the `bin\Debug` subfolder. If you copy / move the executable, always keep the executable and the configuration file together.

