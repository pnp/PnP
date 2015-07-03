# PnP Unit Test report for %configuration% on %testdate% #
This page is showing the results of the PnP unit test run.

## Test configuration ##
This report contains the unit test results from the following run:

Parameter | Value
----------|------
PnP Unit Test configuration | %configuration%
Test run date | %testdate%
Test run time | %testtime%
PnP branch | %pnpbranch%
Visual Studio build configuration | %vsbuildconfiguration%

## Test summary ##
During this test run %numberoftests% tests have been executed with following outcome:

Parameter | Value
----------|------
Executed tests | %numberoftests%
Elapsed time | %elapsedtime%
Passed tests | %passedtests%
Failed tests | **%failedtests%**
Skipped tests | %skippedtests%
Was canceled | %testwascanceled%
Was aborted | %testwasaborted%
Error | %testerror%

## Test run details ##

### Failed tests ###
<table>
<tr>
<td><b>Test name</b></td>
<td><b>Test outcome</b></td>
<td><b>Duration</b></td>
<td><b>Message</b></td>
</tr>
%failedtestdetails%
</table>


### Skipped tests ###
<table>
<tr>
<td><b>Test name</b></td>
<td><b>Test outcome</b></td>
<td><b>Duration</b></td>
<td><b>Message</b></td>
</tr>
%skippedtestdetails%
</table>


### Passed tests ###
<table>
<tr>
<td><b>Test name</b></td>
<td><b>Test outcome</b></td>
<td><b>Duration</b></td>
</tr>
%passedtestdetails%
</table>



