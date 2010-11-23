/* Copyright 2009 Fog Creek Software, Inc. */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

/* FogBugz namespaces-- make sure you add the neccesary assembly references to
 * the following DLL files contained in C:\Program Files\FogBugz\Website\bin\ 
 * FogBugz.dll, FogCreek.Plugins.dll, FogCreek.Plugins.InterfaceEvents.dll     */
using FogCreek.FogBugz.Plugins;
using FogCreek.FogBugz.Plugins.Api;
using FogCreek.FogBugz.Plugins.Interfaces;
using FogCreek.FogBugz;
using FogCreek.FogBugz.UI;

namespace IPluginSettingsMenu_Example
{
    /* Class Declaration: Inherit from Plugin, expose <INTERFACE NAME> */
    public abstract class IPluginSettingsMenu_Example : Plugin, IPluginSettingsMenu, IPluginPageDisplay
    {
        /* Constructor: We'll just initialize the inherited Plugin class, which 
         * takes the passed instance of CPluginApi and sets its "api" member variable. */
        public IPluginSettingsMenu_Example(CPluginApi api)
            : base(api)
        {
        }

        #region IPluginSettingsMenu Members

        public CNavMenuLink[] SettingsMenuLinks()
        {
            /* link to the plugin's page if the current user has at lease normal user
             * permissions */
            if (api.Person.GetCurrentPerson().GetPermissionLevel() >= PermissionLevel.Normal)
            {
                return new CNavMenuLink[] { new CNavMenuLink("Example Plugin", api.Url.PluginPageUrl()) };
            }
            else
                return new CNavMenuLink[] { };
        }

        #endregion

        #region IPluginPageDisplay Members

        public string PageDisplay()
        {
            return string.Format(@"{0}
                <p>This is where the Plugin page would be</p>
                <p>The current date and time is: {1}",
                FogCreek.FogBugz.UI.PageDisplay.Headline("Example Plugin Page"),
                DateTime.Now.ToString()
            );
        }

        public PermissionLevel PageVisibility()
        {
            return PermissionLevel.Normal;
        }

        #endregion
    }
}

