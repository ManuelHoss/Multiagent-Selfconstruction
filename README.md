# Multiagent-selfconstruction
## SETUP
1. Copy SelfConstruction.addin in C:\ProgramData\Autodesk\Revit\Addins\2016.
2. Adjust <Assembly>-path in SelfConstruction.addin to the outputpath of your project. (E.g.: "C:\multiagent-selfconstruction\CSharp\SelfConstruction\SelfConstruction\bin\x64\Debug\SelfConstruction.dll")
3. Activate "Use Managed Compability Mode" in Visual Studio. (Debug -> Options -> General)
4. Select x64 to run Selfconstruction in Revit or x86 to run Unit tests.
5. Enter Debug-Information in SelfConstruction -> Properties:
 * Start external program: C:\Program Files\Autodesk\Revit 2016\Revit.exe (Your Revit path)
 * Command line arguments: C:\multiagent-selfconstruction\CSharp\SelfConstruction\SelfConstructionVorlage.rvt (Adjust Path to your project path)
