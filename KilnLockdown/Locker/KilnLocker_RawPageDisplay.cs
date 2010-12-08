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
            string retVal = null;
            var person = api.Person.GetCurrentPerson();

            if (IsEligible(person))
            {
                if (!PersonHasKilnAccess(person).GetValueOrDefault())
                {
                    string action = api.Request["action"];

                    if (!string.IsNullOrEmpty(action))
                    {
                        switch (action.ToLower())
                        {
                            case "block":
                                retVal = BlockKiln();
                                break;
                            case "hide":
                                retVal = HideKiln();
                                break;
                        }
                    }
                }
            }

            return retVal;
        }

        private string HideKiln()
        {
            var hideKilnScript = new StringBuilder();

            hideKilnScript.Append("$(document).ready(function() {")
                .Append("$('.tabKiln').remove();") //kiln tab at the top of the screen
                .Append("$('#Menu_AppKiln').remove();")
                .Append("});"); //kiln image in the 'my settings' page

            return hideKilnScript.ToString();
        }

        private string BlockKiln()
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
