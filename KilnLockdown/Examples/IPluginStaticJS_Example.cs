/* Copyright 2009 Fog Creek Software, Inc. */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

/* FogBugz namespaces-- make sure you add the neccesary assembly references to
 * DLL files contained in C:\Program Files\FogBugz\website\bin\ */
using FogCreek.FogBugz;
using FogCreek.FogBugz.Plugins;
using FogCreek.FogBugz.Plugins.Interfaces;
using FogCreek.FogBugz.Plugins.Api;
using FogCreek.FogBugz.UI;

namespace IPluginStaticJS_Example
{
    /* Class Declaration: Inherit from Plugin, implement IPluginStaticJS */
    public abstract class IPluginStaticJS_Example : Plugin, IPluginStaticJS, IPluginExtrasMenu
    {
        /* Constructor: We'll just initialize the inherited Plugin class, which 
         * takes the passed instance of CPluginApi and sets its "api" member variable. */
        public IPluginStaticJS_Example(CPluginApi api): base(api)
        {
        }


        #region IPluginStaticJS Members

        public string[] StaticJSFiles()
        {
            /* This file name is relative to the plugin's /static/ folder. */ 
            return new string[] { "js/custom_message.js" };
        }

        #endregion


        #region IPluginExtrasMenu Members

        public CNavMenuLink[] ExtrasMenuLinks()
        {
            return new CNavMenuLink[]{new CNavMenuLink("Say Hello with Javascript!", 
                "javascript:insertCustomHeader('Hellohi!');")};
        }

        #endregion
    }
}