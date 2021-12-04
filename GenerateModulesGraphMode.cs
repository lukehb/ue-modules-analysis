// Copyright Epic Games, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DotNETCommon;

namespace UnrealBuildTool
{
	/// <summary>
	/// Exports a GraphViz file containing all module relationships for the given build target.
	/// </summary>
	[ToolMode("ModuleGraphExport", ToolModeOptions.XmlConfig | ToolModeOptions.BuildPlatforms | ToolModeOptions.SingleInstance)]
	class GenerateModulesGraphMode : ToolMode
	{

		/// <summary>
		/// Execute this command
		/// </summary>
		/// <param name="Arguments">Command line arguments</param>
		/// <returns>Exit code (always zero)</returns>
		public override int Execute(CommandLineArguments Arguments)
		{
			Arguments.ApplyTo(this);

			List<TargetDescriptor> TargetDescriptors = TargetDescriptor.ParseCommandLine(Arguments, false, false);
			foreach (TargetDescriptor TargetDescriptor in TargetDescriptors)
			{
				// Create the target
				UEBuildTarget Target = UEBuildTarget.Create(TargetDescriptor, bSkipRulesCompile: false, bUsePrecompiled: false);

				// Get the output file
				FileReference OutputFile = TargetDescriptor.AdditionalArguments.GetFileReferenceOrDefault("-OutputFile=", null);
				if (OutputFile == null)
				{
					OutputFile = Target.ReceiptFileName.ChangeExtension(".dot");
				}

				// We are writing a fresh GraphViz file, don't append onto an existing one.
				if(FileReference.Exists(OutputFile))
				{
					FileReference.Delete(OutputFile);
				}

				HashSet<string> ProcessedModules = new HashSet<string>();
				Dictionary<string, int> ModuleUseCount = new Dictionary<string, int>();
				StringBuilder ModuleDepsString = new StringBuilder();

				EmitOutput(OutputFile, "digraph {" );
				EmitOutput(OutputFile, "graph [ overlap = scale, ranksep=5 ]");
				
				// Make the Core module the root of our graph
				EmitOutput(OutputFile, "root=\"Core\";");

				foreach (UEBuildBinary Binary in Target.Binaries)
				{
					foreach (UEBuildModule Module in Binary.Modules)
					{
						// No need to process the same module twice
						if(ProcessedModules.Contains(Module.Name))
						{
							continue;
						}

						if(!ModuleUseCount.ContainsKey(Module.Name))
						{
							ModuleUseCount.Add(Module.Name, 0);
						}

						HashSet<UEBuildModule> DirectorDependencies = new HashSet<UEBuildModule>();

						// Find all the modules that this module is directly depending on as first level dependencies.
						Module.PublicDependencyModules.ForEach(ModuleX => DirectorDependencies.Add(ModuleX));
						Module.PublicIncludePathModules.ForEach(ModuleX => DirectorDependencies.Add(ModuleX));
						Module.PrivateDependencyModules.ForEach(ModuleX => DirectorDependencies.Add(ModuleX));
						Module.PrivateIncludePathModules.ForEach(ModuleX => DirectorDependencies.Add(ModuleX));
						Module.DynamicallyLoadedModules.ForEach(ModuleX => DirectorDependencies.Add(ModuleX));

						ModuleDepsString.Append(string.Format("{0} -> {{", Module.Name));
						foreach (UEBuildModule ModuleDependency in DirectorDependencies)
						{
							ModuleDepsString.Append(" ");
							ModuleDepsString.Append(ModuleDependency);
							if(ModuleUseCount.ContainsKey(ModuleDependency.Name))
							{
								ModuleUseCount[ModuleDependency.Name] = ModuleUseCount[ModuleDependency.Name] + 1;
							}
							else
							{
								ModuleUseCount.Add(ModuleDependency.Name, 1);
							}
						}
						ModuleDepsString.Append(" };");
						EmitOutput(OutputFile, ModuleDepsString.ToString());
						ModuleDepsString.Clear();

						ProcessedModules.Add(Module.Name);

					}
				}

				// Sort module use count dictionary
				foreach (KeyValuePair<string, int> Item in ModuleUseCount.OrderByDescending(Item => Item.Value))
				{
					Log.TraceInformation("{0} - used by {1} modules.", Item.Key, Item.Value);
				}

				// Closing brace for GraphViz .dot file.
				EmitOutput(OutputFile, "}");
				Log.TraceInformation("Output GraphViz file written to {0}", OutputFile);
			}
			return 0;
		}

		private void EmitOutput(FileReference OutputFile, string Output)
		{
			FileReference.AppendAllLines(OutputFile, new string[] { Output } );
			// Log.TraceInformation("{0}", ModuleDepsString.ToString());
		}

	}
}