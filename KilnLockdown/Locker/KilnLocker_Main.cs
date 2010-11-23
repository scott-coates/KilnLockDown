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
    public partial class KilnLocker : Plugin
    {
        private const string _kilnAccessTable = "KilnLock";
        private string _ixCanAccessKiln = "ixCanAccessKiln";
        private string _ixPersonKilnAccess = "ixPersonKilnAccess";
        private string _radioInputName = null;

        public const string PluginId = "scoarescoare_KLD";

        public KilnLocker(CPluginApi api)
            : base(api)
        {
            _radioInputName = api.PluginPrefix + "sAllowKiln";
        }

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
                int canAccess = Convert.ToInt32(person.GetPluginField(PluginId, _ixCanAccessKiln));

                if (canAccess == 1)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }

            return retVal;
        }

        private void SetDefaultKilnAccess(CPerson person)
        {
            if (IsLicensedUser(person))
            {
                if (person.fAdministrator)
                {
                    person.SetPluginField(PluginId, _ixCanAccessKiln, 1);
                }
                else
                {
                    person.SetPluginField(PluginId, _ixCanAccessKiln, 0);
                }
            }
        }

        private bool IsEligible(CPerson person)
        {
            return !person.fAdministrator;
        }
    }
}
