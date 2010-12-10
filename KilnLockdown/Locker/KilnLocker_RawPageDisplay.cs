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
            var hideKilnCSS = new StringBuilder();

            hideKilnCSS.Append(".tabKiln { display:none; }")//kiln tab at the top of the screen
                .Append("#Menu_AppKiln { display:none; } ");//kiln image in the 'my settings' page

            return hideKilnCSS.ToString();
        }

        public PermissionLevel RawPageVisibility()
        {
            return PermissionLevel.Public;
        }
    }
}
