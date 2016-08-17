<p align="center">
  <img src="https://github.com/ManuelHoss/Multiagent-Selfconstruction/blob/master/Wiki/Images/HomeHeader.png" width="650"/>
</p>
# Multiagent-selfconstruction
## SETUP
1. Copy SelfConstruction.addin in C:\ProgramData\Autodesk\Revit\Addins\2016.
2. Adjust <Assembly>-path in SelfConstruction.addin to the outputpath of your project. (E.g.: "C:\multiagent-selfconstruction\CSharp\SelfConstruction\SelfConstruction\bin\x64\Debug\SelfConstruction.dll")
3. Activate "Use Managed Compability Mode" in Visual Studio. (Debug -> Options -> General)
4. Select x64 to run Selfconstruction in Revit or x86 to run Unit tests.
5. Enter debug information in SelfConstruction -> Properties:
 * Start external program: C:\Program Files\Autodesk\Revit 2016\Revit.exe (Your Revit path)
 * Command line arguments: C:\multiagent-selfconstruction\CSharp\SelfConstruction\SelfConstructionVorlage.rvt /language ENU (Adjust Path to your project path)
6. To execute the plugin, run solution in Visual Studio -> Revit opens -> In Revit open the Add-Ins tab -> On "External Tools" click "SelfConstruction".


<a href="https://zenhub.com"><img src="https://raw.githubusercontent.com/ZenHubIO/support/master/zenhub-badge.png"></a>
