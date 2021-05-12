<#
.Synopsis
    Performs code analysis using the ReSharper CLT, InspectCode & DupFinder.
.DESCRIPTION
    Takes file path to solution file, output directory for reports and excluded file extensions as parameters, and then exports HTML report of code analysis for the solution.
.EXAMPLE
    . .\Code_Analysis.ps1 -SolutionFilePath "..\WebApp.sln" -ExcludedExtensions "js,css,html"; DupFinder-Analysis
.EXAMPLE
    . .\Code_Analysis.ps1 -SolutionFilePath "..\WebApp.sln" -ExcludedExtensions "js,css,html,xml,xslt,xlsx,pdf,asmx,ascx,ashx"; InspectCode-Analysis
#>

[CmdletBinding()]
Param
(
    [Parameter(Mandatory = $true,
        Position = 0)]
    [string]$SolutionFilePath,

    [Parameter(Mandatory = $false,
        Position = 1)]
    [string]$OutputDirPath = '..\output\',

    [Parameter(Mandatory = $false,
        Position = 2)]
    [string]$ExcludedExtensions = 'js,css'


)
[string[]]$SolutionPathList = $SolutionFilePath.Split('\', [System.StringSplitOptions]::RemoveEmptyEntries)
[string[]]$ExcludedList = $ExcludedExtensions.Split(',', [System.StringSplitOptions]::RemoveEmptyEntries)
[string]$SolutionDirPath = ($SolutionPathList[0..$($SolutionPathList.Count - 2)] -join '\')
[string]$JetBrainsDirPath = 'jetbrains-resharper-clt'

function New-DotSettings-File {
    [Xml]$settingsXml = Get-Content .\$($JetBrainsDirPath)\Templates\DotSettings.xml
    [string]$keysXml = ""
    Foreach ($e in $ExcludedList) {

        $keysXml += "<s:Boolean x:Key=""/Default/CodeInspection/ExcludedFiles/FileMasksToSkip/=_002A_002A_002E$($e)/@EntryIndexedValue"">True</s:Boolean>"

    }
    $settingsXml.ResourceDictionary.InnerXml = $keysXml
    $DotSettingsFileName = $SolutionPathList[$SolutionPathList.Count - 1]
    $settingsXml.Save("$($SolutionDirPath)\$($DotSettingsFileName).DotSettings")

}

function Invoke-InspectCode {
    $OutputFileName = '$($OutputDirPath)resharper-inspectcode-report.xml';
    Invoke-Expression ".\$($JetBrainsDirPath)\InspectCode.exe $($SolutionFilePath) --o=$($OutputFileName)"
}

function Invoke-DupFinder {
    $OutputFileName = '$($OutputDirPath)resharper-dupfinder-report.xml';
    Invoke-Expression ".\$($JetBrainsDirPath)\DupFinder.exe $($SolutionFilePath) --o=$($OutputFileName)"
}

function Export-DupFinder-Report {
    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
    $xslt.Load(".\$($JetBrainsDirPath)\Templates\dupfinder.xslt");
    $XmlFileName = "$($OutputDirPath)\resharper-dupfinder-report.xml";
    $HtmlFileName = "$($OutputDirPath)\resharper-dupfinder-report.html";
    $xslt.Transform("$($XmlFileName)", "$($HtmlFileName)");
}

function Export-InspectCode-Report {
    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
    $xslt.Load(".\$($JetBrainsDirPath)\Templates\inspectcode.xslt");
    $XmlFileName = "$($OutputDirPath)\resharper-inspectcode-report.xml";
    $HtmlFileName = "$($OutputDirPath)\resharper-inspectcode-report.html";
    $xslt.Transform("$($XmlFileName)", "$($HtmlFileName)");
}

function Publish-DupFinder-Analysis {
    New-DotSettings-File
    Invoke-DupFinder
    Export-DupFinder-Report
}

function Publish-InspectCode-Analysis {
    New-DotSettings-File
    Invoke-InspectCode
    Export-InspectCode-Report
}