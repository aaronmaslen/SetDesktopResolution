﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SetDesktopResolution.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Information")]
        public global::Serilog.Events.LogEventLevel MinimumLogDisplayLevel {
            get {
                return ((global::Serilog.Events.LogEventLevel)(this["MinimumLogDisplayLevel"]));
            }
            set {
                this["MinimumLogDisplayLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int MinimumRefreshRate {
            get {
                return ((int)(this["MinimumRefreshRate"]));
            }
            set {
                this["MinimumRefreshRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int MaximumRefreshRate {
            get {
                return ((int)(this["MaximumRefreshRate"]));
            }
            set {
                this["MaximumRefreshRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Only32BitColor {
            get {
                return ((bool)(this["Only32BitColor"]));
            }
            set {
                this["Only32BitColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OverrideModeFilter {
            get {
                return ((bool)(this["OverrideModeFilter"]));
            }
            set {
                this["OverrideModeFilter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ProcessPlusChildren")]
        public global::SetDesktopResolution.Common.Wmi.ProcessDetectionMode ProcessDetectionMode {
            get {
                return ((global::SetDesktopResolution.Common.Wmi.ProcessDetectionMode)(this["ProcessDetectionMode"]));
            }
            set {
                this["ProcessDetectionMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IncludeInterlacedModes {
            get {
                return ((bool)(this["IncludeInterlacedModes"]));
            }
            set {
                this["IncludeInterlacedModes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100000")]
        public long MaxLogItems {
            get {
                return ((long)(this["MaxLogItems"]));
            }
            set {
                this["MaxLogItems"] = value;
            }
        }
    }
}
