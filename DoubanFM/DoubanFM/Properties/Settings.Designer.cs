﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubanFM.Properties
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {

        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        //用户名
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string UserID
        {
            get
            {
                return ((string)(this["UserID"]));
            }
            set
            {
                this["UserID"] = value;
            }
        }

        //密码
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string UserPassword
        {
            get
            {
                return ((string)(this["UserPassword"]));
            }
            set
            {
                this["UserPassword"] = value;
            }
        }

        //关闭时的频道号
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int CloseNo
        {
            get
            {
                return ((int)(this["CloseNo"]));
            }
            set
            {
                this["CloseNo"] = value;
            }
        }

        //频道名
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("fm.公共频道")]
        public string ChannelName
        {
            get
            {
                return ((string)(this["ChannelName"]));
            }
            set
            {
                this["ChannelName"] = value;
            }
        }

        //用户名
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("华语频道")]
        public string SubChannelName
        {
            get
            {
                return ((string)(this["SubChannelName"]));
            }
            set
            {
                this["SubChannelName"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("105")]
        public double BKR
        {
            get
            {
                return ((double)(this["BKR"]));
            }
            set
            {
                this["BKR"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("105")]
        public double BKG
        {
            get
            {
                return ((double)(this["BKG"]));
            }
            set
            {
                this["BKG"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("105")]
        public double BKB
        {
            get
            {
                return ((double)(this["BKB"]));
            }
            set
            {
                this["BKB"] = value;
            }
        }
    } 
}