using System.Reflection;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle("TweakScalerFSBuoyancyIntegrator")]
[assembly: AssemblyDescription("Integrates the TweakScaler to the Target Module")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(TweakScaleCompanion.FS.LegalMamboJambo.Company)]
[assembly: AssemblyProduct(TweakScaleCompanion.FS.LegalMamboJambo.Product)]
[assembly: AssemblyCopyright(TweakScaleCompanion.FS.LegalMamboJambo.Copyright)]
[assembly: AssemblyTrademark(TweakScaleCompanion.FS.LegalMamboJambo.Trademark)]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(TweakScaleCompanion.FS.Version.Number)]
[assembly: AssemblyFileVersion(TweakScaleCompanion.FS.Version.Number)]
[assembly: KSPAssembly("TweakScalerFSBuoyancyIntegrator", TweakScaleCompanion.FS.Version.major, TweakScaleCompanion.FS.Version.minor)]
[assembly: KSPAssemblyDependency("TweakScaleCompanion_FS", TweakScaleCompanion.FS.Version.major, TweakScaleCompanion.FS.Version.minor)]
[assembly: KSPAssemblyDependency("TweakScalerFSbuoyancy", TweakScaleCompanion.FS.Version.major, TweakScaleCompanion.FS.Version.minor)]
[assembly: KSPAssemblyDependency("Firespitter", 0, 0)]
[assembly: KSPAssemblyDependency("KSPe.Light.TweakScale", 2, 5)]
[assembly: KSPAssemblyDependency("Scale", 2, 4)]
//[assembly: KSPAssemblyDependency("Scale_Redist", 1, 0)] // KSP 1.12.2 screwed up the Dependency check!!!
