//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ArcGIS Server Statistics")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Studio A&T s.r.l.")]
[assembly: AssemblyProduct("ArcGIS Server Usage Reports")]
[assembly: AssemblyCopyright("Copyright © Studio A&T s.r.l. 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("D5BF4FAD-9AF1-4199-8840-04AF6068CB0E")]

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
[assembly: AssemblyFileVersion("1.0.0.0")]

/// <summary>
/// Gets the values from the AssemblyInfo.cs file for the current executing assembly
/// </summary>
/// <example>        
/// string company = AssemblyInfo.Company;
/// string product = AssemblyInfo.Product;
/// string copyright = AssemblyInfo.Copyright;
/// string trademark = AssemblyInfo.Trademark;
/// string title = AssemblyInfo.Title;
/// string description = AssemblyInfo.Description;
/// string configuration = AssemblyInfo.Configuration;
/// string fileversion = AssemblyInfo.FileVersion;
/// Version version = AssemblyInfo.Version;
/// string versionFull = AssemblyInfo.VersionFull;
/// string versionMajor = AssemblyInfo.VersionMajor;
/// string versionMinor = AssemblyInfo.VersionMinor;
/// string versionBuild = AssemblyInfo.VersionBuild;
/// string versionRevision = AssemblyInfo.VersionRevision;
/// </example>
public static class AssemblyInfo
{
    /// <summary>
    /// Gets Company
    /// </summary>
    public static string Company
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyCompanyAttribute>(a => a.Company);
        }
    }

    /// <summary>
    /// Gets Product
    /// </summary>
    public static string Product
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyProductAttribute>(a => a.Product);
        }
    }

    /// <summary>
    /// Gets Copyright
    /// </summary>
    public static string Copyright
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
        }
    }

    /// <summary>
    /// Gets Trademark
    /// </summary>
    public static string Trademark
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyTrademarkAttribute>(a => a.Trademark);
        }
    }

    /// <summary>
    /// Gets Title
    /// </summary>
    public static string Title
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
        }
    }

    /// <summary>
    /// Gets Description
    /// </summary>
    public static string Description
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
        }
    }

    /// <summary>
    /// Gets Configuration
    /// </summary>
    public static string Configuration
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
        }
    }

    /// <summary>
    /// Gets FileVersion
    /// </summary>
    public static string FileVersion
    {
        get
        {
            return GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
        }
    }

    /// <summary>
    /// Gets Version
    /// </summary>
    public static Version Version
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }

    /// <summary>
    /// Gets VersionFull
    /// </summary>
    public static string VersionFull
    {
        get
        {
            return Version.ToString();
        }
    }

    /// <summary>
    /// Gets VersionMajor
    /// </summary>
    public static string VersionMajor
    {
        get
        {
            return Version.Major.ToString();
        }
    }

    /// <summary>
    /// Gets VersionMinor
    /// </summary>
    public static string VersionMinor
    {
        get
        {
            return Version.Minor.ToString();
        }
    }

    /// <summary>
    /// Gets VersionBuild
    /// </summary>
    public static string VersionBuild
    {
        get
        {
            return Version.Build.ToString();
        }
    }

    /// <summary>
    /// Gets VersionRevision
    /// </summary>
    public static string VersionRevision
    {
        get
        {
            return Version.Revision.ToString();
        }
    }

    /// <summary>
    /// Gets Attribute
    /// </summary>
    /// <typeparam name="T">type of attribute</typeparam>
    /// <param name="value">name of attribute</param>
    /// <returns>value of attribute</returns>
    private static string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
    {
        T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        return value.Invoke(attribute);
    }
}