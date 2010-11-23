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
using System.Collections;

namespace KilnLockdown.Locker
{
    public partial class KilnLocker : IPluginPersonDisplay
    {
        public CDialogItem[] PersonDisplayEdit(CPerson person)
        {
            var allowKiln = new CDialogItem();
            var retVal = new CDialogItem[] { allowKiln };
            allowKiln.sLabel = "Kiln Access";
            allowKiln.sInstructions = "Is this user allowed to use kiln?";

            if (IsEligible(person))
            {
                var personHasKilnAccess = PersonHasKilnAccess(person);

                allowKiln.sContent =
                    Forms.RadioInput("sAllowKiln", "Yes", personHasKilnAccess, "Yes", "sAllowKilnYes")
                     +
                     Forms.RadioInput("sAllowKiln", "No", !personHasKilnAccess, "Now", "sAllowKilnNo")
                     ;
            }
            else
            {
                //set default setting to yes if admin and no if normal user
                var setDefault = person.IsSiteAdmin();
                var enabledAttrs = new Dictionary<string, string> { { "disabled", "true" } };

                allowKiln.sContent =
                    Forms.RadioInput("sAllowKiln", "Yes", setDefault, "Yes", "sAllowKilnYes", enabledAttrs)
                     +
                     Forms.RadioInput("sAllowKiln", "No", !setDefault, "No", "sAllowKilnNo", enabledAttrs)
                     ;
            }

            return retVal;
        }

        private bool IsEligible(CPerson person)
        {
            return api.SiteConfiguration.IsKilnEnabled && !person.IsSiteAdmin();
        }

        public string[] PersonDisplayListFields(CPerson person)
        {
            return new string[] { PersonHasKilnAccess(person) ? "Yes" : "No" };
        }

        public string[] PersonDisplayListHeaders()
        {
            return new string[] { "Can Access Kiln" };
        }
    }
}
