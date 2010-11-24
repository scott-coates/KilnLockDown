using System;
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
    public partial class KilnLocker : IPluginRawPageDisplay
    {
        public string RawPageDisplay()
        {
            var blockKilnScript = new StringBuilder();

            blockKilnScript.Append("if(window.location.toString().toLowerCase().indexOf('" + GetKilnInstallationURL().ToLower() + "') != -1)");
            blockKilnScript.Append("{\n");
            blockKilnScript.Append("window.location = '" + api.Url.PluginPageUrl() + "';");
            blockKilnScript.Append("\n}");

            return blockKilnScript.ToString();
        }

        public PermissionLevel RawPageVisibility()
        {
            return PermissionLevel.Normal;
        }
    }
}
