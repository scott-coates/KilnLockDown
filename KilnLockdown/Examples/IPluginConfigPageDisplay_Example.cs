/* Copyright 2009 Fog Creek Software, Inc. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

/* FogBugz namespaces-- make sure you add the neccesary assembly references to
 * DLL files contained in C:\Program Files\FogBugz\website\bin\ */
using FogCreek.FogBugz;
using FogCreek.FogBugz.Plugins;
using FogCreek.FogBugz.Plugins.Api;
using FogCreek.FogBugz.Plugins.Interfaces;
using FogCreek.FogBugz.UI;

namespace IPluginConfigPageDisplay_Example
{
    /* Class Declaration: Inherit from Plugin, implement IPluginConfigPageDisplay */
    public  class IPluginConfigPageDisplay_Example : Plugin, IPluginConfigPageDisplay
    {
        /* Constructor: We'll just initialize the inherited Plugin class, which 
         * takes the passed instance of CPluginApi and sets its "api" member variable. */
        public IPluginConfigPageDisplay_Example(CPluginApi api): base(api)
        {
        }
    
        #region IPluginConfigPageDisplay Members

        public string  ConfigPageDisplay()
        {
            string sHTML = "";
            sHTML += GetHeader();
            sHTML += GetForm();
            return sHTML;
        }

        #endregion

        protected string GetHeader()
        {
            /* Generate an informational header, making use of the FogBugz UI library,
             * and grabbing this page's URL from the plugin API. */
            
            return String.Format(
                @"<p>{0}</p>
                  <p>
                        This configuration page, automatically linked to from the 
                        <a href=""default.asp?pg=pgPlugins"">Plugins page</a>, should
                        provide any controls necessary for FogBugz site administrators.
                  </p>            
                  <p>The Plugin API tells me this page's URL is: <b>{1}</b></p>",
                
                PageDisplay.Headline("Sample Configuration Page"),           
                api.Url.PluginConfigPageUrl());
        }

        protected string GetForm()
        {           
            /* Generate a form that will post a value to the server and display
             * that value taken from the request object (if it exists).
             * 
             * IMPORTANT NOTE: The unique prefix of this plugin must be prepended to the
             * names of all form elements, but the values of those elements can then be
             * gathered from the request object without using the prefix.
             * 
             * In keeping with security best practices, this plugin does not display
             * unless a required action token validates.
             * 
             * In this case, the "password" value is posted and retrieved. */

            string sCurrentPassMessage = "The secret password is: <b>Kiwi</b>";
            if (api.Request[api.AddPluginPrefix("password")] != null)
            {
                if ((api.Request[api.AddPluginPrefix("actionToken")] != null) &&
                api.Security.ValidateActionToken(api.Request[api.AddPluginPrefix("actionToken")].ToString()))
                {
                    sCurrentPassMessage = string.Format(
                        "The secret password is: <b>{0}</b>",
                        HttpUtility.HtmlEncode(api.Request[api.AddPluginPrefix("password")].ToString())
                    );
                }
                else
                {
                    sCurrentPassMessage = "Action token invalid!";
                }
            }

            return String.Format(
                @"<form action=""{0}"" method=""POST"">
                        <input type=""hidden"" name=""{1}actionToken"" value=""{2}"">
                        <p>{3}</p>
                        <p><b>Change secret Password:</b> {4} {5}</p>
                </form>",
                
                HttpUtility.HtmlAttributeEncode(api.Url.PluginConfigPageUrl()),
                api.PluginPrefix,
                api.Security.GetActionToken(),
                sCurrentPassMessage,
                Forms.TextInput(api.PluginPrefix + "password", ""),
                Forms.SubmitButton("idSubmit", "Submit")
                );
        }
    }
}