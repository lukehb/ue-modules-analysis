# ue-modules-analysis
Analysing modules in Unreal Engine to reduce complexity and build time.


I added a new tool mode to UBT to generate a GraphViz graph of UE modules.

1. Put `GenerateModulesGraphMode.cs` in `Engine\Source\Programs\UnrealBuildTool\Modes`
2. Compile UBT.
3. Call UBT with the new tool mode, like so: `UnrealBuildTool.exe -mode=ModuleGraphExport UE4Editor Win64 Debug`

![Core Module](./densest_module_in_the_known_universe.png)
