rem c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnPremCred /target:debugging
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnPremCred /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnPremAppOnly /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnlineCred /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /property:PnPConfigurationToTest=OnlineAppOnly /target:BuildAndUnitTestPnP
c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe PnPCore.targets /target:PushResultsToGitHub
exit
