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
using FogCreek.FogBugz.Database;

namespace KilnLockdown.Locker
{
    public partial class KilnLocker : IPluginDatabase
    {
        public CTable[] DatabaseSchema()
        {
            CTable personKilnLock = api.Database.NewTable(api.Database.PluginTableName(_kilnAccessTable));
            personKilnLock.sDesc = "Tracks which users have access to kiln.";
            personKilnLock.AddAutoIncrementPrimaryKey(_ixPersonKilnAccess);
            personKilnLock.AddIntColumn("ixPerson", true, 0);
            personKilnLock.AddSmallIntColumn(_ixCanAccessKiln, true, 0);

            return new CTable[] { personKilnLock };
        }

        public int DatabaseSchemaVersion()
        {
            return 1;
        }

        public void DatabaseUpgradeBefore(int ixVersionFrom, int ixVersionTo, CDatabaseUpgradeApi apiUpgrade) { }

        public void DatabaseUpgradeAfter(int ixVersionFrom, int ixVersionTo, CDatabaseUpgradeApi apiUpgrade) { }
    }
}
