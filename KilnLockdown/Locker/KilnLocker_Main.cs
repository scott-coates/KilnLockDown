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
    public partial class KilnLocker : Plugin
    {
        private const string _kilnAccessTable = "KilnLock";
        private string _ixCanAccessKiln = "ixCanAccessKiln";
        private string _ixPersonKilnAccess = "ixPersonKilnAccess";

        public const string PluginId = "scoarescoare_KLD";

        public KilnLocker(CPluginApi api) : base(api) { }

        private string PersonKilnAccessString(CPerson person)
        {
            string retVal = "N/A";

            bool? hasAccess = PersonHasKilnAccess(person);

            if (hasAccess.HasValue)
            {
                retVal = hasAccess.Value ? "Yes" : "No";
            }

            return retVal;
        }

        private bool IsLicensedUser(CPerson person)
        {
            return !person.fCommunity && !person.fVirtual;
        }

        private bool? PersonHasKilnAccess(CPerson person)
        {
            bool? retVal = null;

            if (IsLicensedUser(person))
            {
                var canAccess = person.GetPluginField(PluginId, "") as int?;

                if (canAccess == null)
                {
                    //set if first time
                    retVal = SetDefaultKilnAccess(person);
                }
                else if (canAccess == 1)
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        private bool SetDefaultKilnAccess(CPerson person)
        {
            bool retVal;

            if (person.fAdministrator)
            {
                retVal = true;
                person.SetPluginField(PluginId, _ixCanAccessKiln, 1);
            }
            else
            {
                retVal = false;
                person.SetPluginField(PluginId, _ixCanAccessKiln, 0);
            }

            person.Commit();

            return retVal;
        }
    }
}
