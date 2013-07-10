using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rhino.PlugIns;


// Plug-in Description Attributes - all of these are optional
// These will show in Rhino's option dialog, in the tab Plug-ins
[assembly: PlugInDescription(DescriptionType.Address, "SF Bay Area")]
[assembly: PlugInDescription(DescriptionType.Country, "USA")]
[assembly: PlugInDescription(DescriptionType.Email, "n/a")]
[assembly: PlugInDescription(DescriptionType.Phone, "n/a")]
[assembly: PlugInDescription(DescriptionType.Fax, "n/a")]
[assembly: PlugInDescription(DescriptionType.Organization, "TC")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "Check Food For Rhino website or GIT Hub")]
[assembly: PlugInDescription(DescriptionType.WebSite, "n/a")]


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Rhino LSystem")] // Plug-In title is extracted from this
[assembly: AssemblyDescription("Rhino implementation of the LSystem Engine")]
[assembly: AssemblyConfiguration("n/a")]
[assembly: AssemblyCompany("RhinoLSystemPlugin")]
[assembly: AssemblyProduct("Rhino LSystem Engine Plugin")]
[assembly: AssemblyCopyright("Copyright © Tom Copple, 2013: MIT License")]
[assembly: AssemblyTrademark("n/a")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyInformationalVersion("2")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("472cd7a1-4072-4bf5-b083-49f337e8cc06")] // This will also be the Guid of the Rhino plug-in

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.9.0")]
[assembly: AssemblyFileVersion("0.9.0")]
