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

namespace KilnLockdown
{
    public class KilnLocker : Plugin, IPluginPersonDisplay
    {
        public KilnLocker(CPluginApi api) : base(api) { }

        public CDialogItem[] PersonDisplayEdit(CPerson person)
        {
            CDialogItem[] retVal = null;
            var editDisplayHTML = new StringBuilder();

            //if (IsEligible(person))
            {
                editDisplayHTML.Append("<input type='radio' name='useKiln' id='useKilnTrue' value='true' />");
                editDisplayHTML.Append("<label for='useKilnTrue'>Yes</label>");
                editDisplayHTML.Append("<input type='radio' name='useKiln' id='useKilnFalse' value='false' />");
                editDisplayHTML.Append("<label for='useKilnFalse'>No</label>");

                retVal = new CDialogItem[] { new CDialogItem(editDisplayHTML.ToString(), "Allow Kilin", "Is this user allowed to use kiln?") };
            }

            return retVal;
        }

        private bool IsEligible(CPerson person)
        {
            return api.SiteConfiguration.IsKilnEnabled && !person.IsSiteAdmin();
        }

        public string[] PersonDisplayListFields(CPerson person)
        {
            return new string[] { "PersonDisplayListFields 1 " + person.sFullName };
        }

        public string[] PersonDisplayListHeaders()
        {
            return new string[] { "PersonDisplayListHeaders 2" };
        }
    }
}
