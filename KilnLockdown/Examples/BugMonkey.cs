using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using FogCreek.FogBugz.Plugins;
using FogCreek.FogBugz.Plugins.Api;
using FogCreek.FogBugz.Plugins.Entity;
using FogCreek.FogBugz.Plugins.Interfaces;
using FogCreek.FogBugz;
using FogCreek.FogBugz.UI;
using FogCreek.FogBugz.UI.Dialog;

namespace InjectHTML
{
    public abstract class BugMonkey : Plugin,
        IPluginRawPageDisplay,
        IPluginPageDisplay,
        IPluginBinaryPageDisplay,
        IPluginStaticJS,
        IPluginStaticCSS,
        IPluginConfigPageDisplay,
        IPluginAdminMenu
    {
        public BugMonkey(CPluginApi api)
            : base(api)
        { }

        protected string GetTextArea(string sKey)
        {
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();
            string s = "";
            if (kvt.ContainsKey(sKey)) s = kvt.GetValue(sKey);
            
            return string.Format("<textarea name=\"{0}" + sKey + "\" rows=\"15\" cols=\"120\">", api.PluginPrefix) +
                HttpUtility.HtmlEncode(s) +
                "</textarea><br />";
        }

        protected string PermissionOption(string sValue, string sDesc, string sCurrentValue)
        {
            string s = @"<option value=""" + sValue + @"""";
            if (sValue == sCurrentValue)
                s += @" selected=""selected"" ";
            s += @">" + sDesc + @"</option>";
            return s;
        }

        protected string PermissionSelect(string sToken, string sName, string sCurrentValue)
        {
            return @"Run this code: " +
                Forms.SelectInput(sToken + sName, new string[] {
                    "only for logged on Administrators",
                    "only for logged on licensed users (not community users)",
                    "only for logged on users (including community users)",
                    "only for non-logged on users",
                    "only for non-logged on users and community users",
                    "for all visitors to my site"},
                    sCurrentValue, new string[] { "Administrator", "Normal", "Community", "Anonymous", "AnonymousCommunity", "Public" }
                   ) + "<br />";
        }

        protected string HtmlAddForm()
        {
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();
            string sJS = "", sCSS = "", jsPermission = "Normal", cssPermission = "Normal";
            if (kvt.ContainsKey("js")) sJS = kvt.GetValue("js");
            if (kvt.ContainsKey("css")) sCSS = kvt.GetValue("css");
            if (kvt.ContainsKey("jsPermission")) jsPermission = kvt.GetValue("jsPermission");
            if (kvt.ContainsKey("cssPermission")) cssPermission = kvt.GetValue("cssPermission");

            return string.Format(@"
                <form action=""{0}"" method=""POST"">
                <input type=""hidden"" name=""{1}actionToken"" value=""{2}"" />
                <input type=""hidden"" name=""{1}sAction"" value=""setScript"" />",
                    api.Url.PluginPageUrl(),
                    api.PluginPrefix,
                    api.Security.GetActionToken("setScript")) +
                @"<h2>Javascript</h2>
                <p>Enter any javascript that you would like to appear on every FogBugz page. <b>jQuery syntax is allowed!</b> This script will run after the document has been fully loaded.</p>
                <p>For a sample test, try <code>alert('Hello Kiwi!');</code></p>
                <p><i>Note: the case editing page is not a full page reload, so to get js to run on that transition you need to use a timeout.</i> <code>var f = function () { /* something interesting */ setTimeout( f, 1000) }; setTimeout(f, 1000);</code></p>
                " +
                PermissionSelect(api.PluginPrefix, "jsPermission", jsPermission) +
                GetTextArea("js") +
                @"<h2>CSS</h2>
                <p>Enter any CSS you wanted added to every FogBugz page.</p>
                <p>For a sample test, try <code>body { background-color:red !important; }</code></p>" +
                PermissionSelect(api.PluginPrefix, "cssPermission", cssPermission) +
                GetTextArea("css") +
                @"<input type=""submit"" value=""Save"" /></form>";
        }

        #region IPluginRawPageDisplay Members

        public string RawPageDisplay()
        {
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();

            if (kvt.ContainsKey("js"))
            {
                if (kvt.ContainsKey("jsPermission"))
                {
                    CPerson p = api.Person.GetCurrentPerson();
                    switch (kvt.GetValue("jsPermission"))
                    {
                        case "Anonymous": // if you are logged in, no js
                            if (p.GetPermissionLevel() > PermissionLevel.Public)
                                return "";
                            break;
                        case "AnonymousCommunity": // if you are logged in and not a community member, no js
                            if (p.GetPermissionLevel() > PermissionLevel.Community)
                                return "";
                            break;
                    }
                }
                return kvt.GetValue("js");
            }

            return "";
        }

        protected PermissionLevel PermissionFromString(string s)
        {
            switch (s)
            {
                case "Public":
                case "Anonymous":
                case "AnonymousCommunity":
                    return PermissionLevel.Public;
                case "Normal":
                    return PermissionLevel.Normal;
                case "Community":
                    return PermissionLevel.Community;
                case "Administrator":
                    return PermissionLevel.Administrator;
            }
            return PermissionLevel.Normal;
        
        }

        public PermissionLevel RawPageVisibility()
        {
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();
            if (kvt.ContainsKey("jsPermission"))
                return PermissionFromString(kvt.GetValue("jsPermission"));            
            return PermissionLevel.Normal;
        }

        #endregion

        #region IPluginBinaryPageDisplay Members

        public byte[] BinaryPageDisplay()
        {
            api.Response.ContentType = "text/css";
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

            if (kvt.ContainsKey("css"))
            {
                if (kvt.ContainsKey("cssPermission"))
                {
                    CPerson p = api.Person.GetCurrentPerson();
                    switch (kvt.GetValue("cssPermission"))
                    {
                        case "Anonymous": // if you are logged in, no css
                            if (p.GetPermissionLevel() > PermissionLevel.Public)
                                return enc.GetBytes("");
                            break;
                        case "AnonymousCommunity": // if you are logged in and not a community member, no css
                            if (p.GetPermissionLevel() > PermissionLevel.Community)
                                return enc.GetBytes("");
                            break;
                    }
                }

                return enc.GetBytes(kvt.GetValue("css"));
            }
            return enc.GetBytes("");
        }

        public PermissionLevel BinaryPageVisibility()
        {
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();
            if (kvt.ContainsKey("cssPermission"))
                return PermissionFromString(kvt.GetValue("cssPermission"));
            return PermissionLevel.Normal;
        }

        #endregion
        #region IPluginStaticJS Members

        public string[] StaticJSFiles()
        {
            return new string[] { "js/custom.js" };
        }

        #endregion

        #region IPluginPageDisplay Members

        public string PageDisplay()
        {
            CPluginKeyValueTable kvt = api.Database.GetKeyValueTable();
            if (api.Request[api.AddPluginPrefix("actionToken")] != null &&
                api.Security.ValidateActionToken(api.Request[api.AddPluginPrefix("actionToken")], "setScript") &&
                api.Request[api.AddPluginPrefix("sAction")].ToString() == "setScript")
            {
                kvt.SetValue("js", api.Request[api.AddPluginPrefix("js")]);
                kvt.SetValue("css", api.Request[api.AddPluginPrefix("css")]);
                kvt.SetValue("jsPermission", api.Request[api.AddPluginPrefix("jsPermission")]);
                kvt.SetValue("cssPermission", api.Request[api.AddPluginPrefix("cssPermission")]);
                kvt.Commit();
            }

            return HtmlAddForm();
        }

        public PermissionLevel PageVisibility()
        {
            return PermissionLevel.Administrator;
        }

        #endregion

        #region IPluginStaticCSS Members

        public string[] StaticCSSFiles()
        {
            return new string[] { "css/custom.css" };
        }

        #endregion

        #region IPluginAdminMenu Members

        public CNavMenuLink[] AdminMenuLinks()
        {
            if (api.Person.GetCurrentPerson().IsSiteAdmin())
                return new CNavMenuLink[] { new CNavMenuLink("Set Site Javascript & CSS", api.Url.PluginPageUrl()) };

            return new CNavMenuLink[] { };
        }

        #endregion

        #region IPluginConfigPageDisplay Members

        public string ConfigPageDisplay()
        {
            return PageDisplay();
        }

        #endregion
    }
}
