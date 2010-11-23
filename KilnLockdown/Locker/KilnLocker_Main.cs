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
        protected const string _pluginId =
            "IKilnAccess@scoarescoare.com";

        public KilnLocker(CPluginApi api) : base(api) { }

        private bool PersonHasKilnAccess(CPerson person)
        {
            bool retVal = false;

            int canAccess = Convert.ToInt32(person.GetPluginField(_pluginId, "ixCanAccessKiln"));

            if (canAccess == 1)
            {
                retVal = true;
            }

            return retVal;
        }
    }
}
