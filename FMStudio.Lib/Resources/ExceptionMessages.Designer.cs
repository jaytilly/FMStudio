﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FMStudio.Lib.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FMStudio.Lib.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not connect to the database..
        /// </summary>
        internal static string InitializeProject_CouldNotConnectToDatabase {
            get {
                return ResourceManager.GetString("InitializeProject_CouldNotConnectToDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified dll does not appear to be a migrations dll..
        /// </summary>
        internal static string InitializeProject_CouldNotFindFluentMigratorReference {
            get {
                return ResourceManager.GetString("InitializeProject_CouldNotFindFluentMigratorReference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not load the specified migrations dll..
        /// </summary>
        internal static string InitializeProject_CouldNotLoadDll {
            get {
                return ResourceManager.GetString("InitializeProject_CouldNotLoadDll", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not initialize database, no database type specified..
        /// </summary>
        internal static string InitializeProject_NoDatabaseTypeSpecified {
            get {
                return ResourceManager.GetString("InitializeProject_NoDatabaseTypeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified migrations dll does not exist..
        /// </summary>
        internal static string InitializeProject_PathToMigrationsDoesNotExist {
            get {
                return ResourceManager.GetString("InitializeProject_PathToMigrationsDoesNotExist", resourceCulture);
            }
        }
    }
}
