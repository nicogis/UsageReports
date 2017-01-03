# Usage Reports ArcGIS Server

## Requirements
.Net Framework 4.5
ArcGIS Server 10.5.0 or superior

## Description
Generate a xlsx with the following metrics: 
* RequestCount —the number of requests received
* RequestsFailed —the number of requests that failed
* RequestsTimedOut —the number of requests that timed out
* RequestMaxResponseTime —the maximum response time
* RequestAvgResponseTime —the average response time
* ServiceActiveInstances—the maximum number of active (running) service instances sampled at 1 minute intervals, for a specified service

This tool is useful for monitoring/tuning arcgis server services 
You can create your batch file and schedule the application console in **Task Scheduler Windows**
In this initial version the metrics are analyzed for all services. 

For details [Create Usage]
(http://resources.arcgis.com/en/help/arcgis-rest-api/index.html#/Create_Usage_Report/02r30000027n000000/)

## Syntax
```Studioat.ArcGISServer.UsageReports -h```

-s  (required) list of servers (delimiter ';')

-u  (required) list of admins (delimiter ';')

-p  (required) list of password (delimiter ';')

-o  (required) output folder xlsx

-a  (optional) Time interval in minutes. Server metrics are summarized and returned for time slices using the specified interval. The time range for the report, specified using the since parameter (and from and to when since is CUSTOM) is split into multiple slices, each covering a time interval. Server metrics are then summarized for each time slice and returned as data points in the report data.
    When the aggregationInterval is not specified, the following defaults are used:
    LAST_DAY: 30 minutes
    LAST_WEEK: 4 hours
    LAST_MONTH: 24 hours
    CUSTOM: 30 minutes up to 1 day, 4 hours up to 1 week, 1 day up to 30 days, and 1 week for longer periods.

-c  (optional default LAST_DAY) The time duration for the report.
    Values: LAST_DAY, LAST_WEEK, LAST_MONTH, CUSTOM
    LAST_DAY represents a time range spanning the previous 24 hours.
    LAST_WEEK represents a time range spanning the previous 7 days.
    LAST_MONTH represents a time range spanning the previous 30 days.
    CUSTOM represents a time range that is specified using the from and to parameters.

-f  (required if use -c CUSTOM) For the beginning period of the report dd-MM-yyyy-HH:mm:ss. Used when sinceTyep (-c) parameter is CUSTOM

-t  (required if use -c CUSTOM) For the ending period of the report dd-MM-yyyy-HH:mm:ss. Used when sinceType (-c) parameter is CUSTOM

-i  (optional default false) include system folders

## Sample:
```
Studioat.ArcGISServer.UsageReports -s http://yourHostname:6080/arcgis;http://myserver.cloudapp.net/arcgis -u siteadmin;siteadmin -p myPwd1;myPwd2 -o C:\Temp\OutputUsageReports -a 60 -c LAST_MONTH
```
