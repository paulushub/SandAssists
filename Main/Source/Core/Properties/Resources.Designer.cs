﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4214
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sandcastle.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sandcastle.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to         * {margin:0; padding:0; font:12px Verdana,Arial}
        ///
        ///        #options {width:90%; margin:20px auto; text-align:right; color:#9ac1c9}
        ///        #options a {text-decoration:none; color:#9ac1c9}
        ///        #options a:hover {color:#033}
        ///
        ///        #acc {width:98%; list-style:none; color:#033; margin:0 auto 40px}
        ///        #acc h3 {width:98%; border:1px solid #9ac1c9; padding:2px 2px 4px; font-weight:bold; margin-top:5px; cursor:pointer;}
        ///        #acc h3:hover {background:#9ac1c9}
        ///        #acc .acc-section  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HtmlLogCss {
            get {
                return ResourceManager.GetString("HtmlLogCss", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to         var TINY = {};
        ///
        ///        function T$(i) {return document.getElementById(i); }
        ///        function T$$(e, p) {return p.getElementsByTagName(e); }
        ///
        ///        TINY.accordion = function() {
        ///            function slider(n) { this.n = n; this.a = [] }
        ///            slider.prototype.init = function(t, e, m, o, k) {
        ///                var a = T$(t), i = s = 0, n = a.childNodes, l = n.length; this.s = k || 0; this.m = m || 0;
        ///                for (i; i &lt; l; i++) {
        ///                    var v = n[i];
        ///             [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HtmlLogJs {
            get {
                return ResourceManager.GetString("HtmlLogJs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ended.
        /// </summary>
        internal static string LogEnded {
            get {
                return ResourceManager.GetString("LogEnded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string LogError {
            get {
                return ResourceManager.GetString("LogError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Info.
        /// </summary>
        internal static string LogInfo {
            get {
                return ResourceManager.GetString("LogInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Started.
        /// </summary>
        internal static string LogStarted {
            get {
                return ResourceManager.GetString("LogStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warn.
        /// </summary>
        internal static string LogWarn {
            get {
                return ResourceManager.GetString("LogWarn", resourceCulture);
            }
        }
    }
}
