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
    public partial class KilnLocker : IPluginPersonDisplay
    {
        public CDialogItem[] PersonDisplayEdit(CPerson person)
        {
            CDialogItem[] retVal = null;

            //if (IsEligible(person))
            {
                var allowKiln = new CDialogItem();
                allowKiln.sLabel = "Is this user allowed to use kiln?";
                var values = new string[] { "Yes", "No" };
                allowKiln.sContent = Forms.RadioInputGroup(api.PluginPrefix + "sAllowKiln", values, "", values);
                retVal = new CDialogItem[] { allowKiln };
            }

            return retVal;
        }

        private bool IsEligible(CPerson person)
        {
            return api.SiteConfiguration.IsKilnEnabled && !person.IsSiteAdmin();
        }

        public string[] PersonDisplayListFields(CPerson person)
        {
            return new string[] { "Yes" };
        }

        public string[] PersonDisplayListHeaders()
        {
            return new string[] { "Can Access Kiln" };
        }
    }
}
