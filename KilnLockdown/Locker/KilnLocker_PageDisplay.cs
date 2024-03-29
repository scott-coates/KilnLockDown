﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FogCreek.FogBugz;
using FogCreek.FogBugz.Plugins;
using FogCreek.FogBugz.Plugins.Api;
using FogCreek.FogBugz.Plugins.Interfaces;
using FogCreek.FogBugz.UI;
using FogCreek.FogBugz.UI.Dialog;
using FogCreek.FogBugz.Plugins.Entity;

namespace KilnLockdown.Locker
{
    public partial class KilnLocker : IPluginPageDisplay
    {
        public string PageDisplay()
        {
            string retVal = String.Format("<p>{0}</p>", FogCreek.FogBugz.UI.PageDisplay.Headline("Access Denied"));

            retVal += "<p>You are not authorized to access Kiln.  Please contact your system administrator if you think you should have access.</p>";

            return retVal;

        }

        public PermissionLevel PageVisibility()
        {
            return PermissionLevel.Normal;
        }
    }
}
