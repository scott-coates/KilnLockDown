﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FogCreek.Plugins;
using KilnLockdown.Locker;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Kiln Lock Down")]
[assembly: AssemblyDescription("Prevents access to Kiln for those who don't need it.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Scoarescoare Software")]
[assembly: AssemblyProduct("KilnLockdown")]
[assembly: AssemblyCopyright("Copyright ©  2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d57c26ac-592f-4018-9795-acb7610187af")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.2.1.0")]

/*FogCreek Plugin Info*/
[assembly: AssemblyFogCreekPluginIdAttribute(KilnLocker.PluginId)]
[assembly: AssemblyFogCreekMajorVersionAttribute(3)]
[assembly: AssemblyFogCreekMinorVersionMinAttribute(5)]
[assembly: AssemblyFogCreekEmailAddressAttribute("scoarescoare@gmail.com")]
//[assembly: AssemblyFogCreekWebsiteAttribute("http://scoarescoare.com")]