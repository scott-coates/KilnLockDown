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
        public const string PluginId = "scoarescoare_KLD";

        public KilnLocker(CPluginApi api) : base(api) { }

        private string PersonKilnAccessString(CPerson person)
        {
            string retVal = "N/A";

            if (!person.fCommunity && !person.fVirtual)
            {
                bool hasAccess = PersonHasKilnAccess(person);
                retVal = hasAccess ? "Yes" : "No";
            }

            return retVal;
        }

        private bool PersonHasKilnAccess(CPerson person)
        {
            bool retVal = false;
            
            int canAccess = Convert.ToInt32(person.GetPluginField(PluginId, "ixCanAccessKiln"));

            if (canAccess == 1)
            {
                retVal = true;
            }

            return retVal;
        }
    }
}
