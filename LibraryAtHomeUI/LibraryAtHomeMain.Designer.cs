﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibraryAtHomeUI {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class LibraryAtHomeMain : global::System.Configuration.ApplicationSettingsBase {
        
        private static LibraryAtHomeMain defaultInstance = ((LibraryAtHomeMain)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new LibraryAtHomeMain())));
        
        public static LibraryAtHomeMain Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("libraryathome")]
        public string LastLibraryOpened {
            get {
                return ((string)(this["LastLibraryOpened"]));
            }
            set {
                this["LastLibraryOpened"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Personal\\library\\books\\ebooks")]
        public string EbookFolder {
            get {
                return ((string)(this["EbookFolder"]));
            }
            set {
                this["EbookFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("localhost")]
        public string RepositoryHost {
            get {
                return ((string)(this["RepositoryHost"]));
            }
            set {
                this["RepositoryHost"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool LibraryExists {
            get {
                return ((bool)(this["LibraryExists"]));
            }
            set {
                this["LibraryExists"] = value;
            }
        }
    }
}
