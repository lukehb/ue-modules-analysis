# Unreal Engine Modules Analysis
Analysing modules in Unreal Engine to reduce complexity and build time.


I added a new tool mode to UBT to generate a GraphViz graph of UE modules.

1. Put `GenerateModulesGraphMode.cs` in `Engine\Source\Programs\UnrealBuildTool\Modes`
2. Compile UBT.
3. Call UBT with the new tool mode, like so: `UnrealBuildTool.exe -mode=ModuleGraphExport UE4Editor Win64 Debug`

![Core Module](./densest_module_in_the_known_universe.png)

## Most used modules
When running this new tool mode it also outputs the most used modules, like so:

```
Core - used by 1178 modules.
CoreUObject - used by 989 modules.
Engine - used by 868 modules.
SlateCore - used by 505 modules.
Slate - used by 490 modules.
UnrealEd - used by 446 modules.
InputCore - used by 380 modules.
EditorStyle - used by 296 modules.
RenderCore - used by 267 modules.
RHI - used by 248 modules.
Projects - used by 184 modules.
PropertyEditor - used by 172 modules.
Json - used by 138 modules.
ApplicationCore - used by 133 modules.
DesktopPlatform - used by 118 modules.
TargetPlatform - used by 118 modules.
AssetTools - used by 112 modules.
AssetRegistry - used by 95 modules.
LevelEditor - used by 90 modules.
ToolMenus - used by 80 modules.
Settings - used by 77 modules.
MeshDescription - used by 73 modules.
```
