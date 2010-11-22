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
            personKilnLock.AddAutoIncrementPrimaryKey("ixPersonKilnAccess");
            personKilnLock.AddIntColumn("ixPerson", true, 0);
            personKilnLock.AddSmallIntColumn("ixCanAccessKiln", true, 0);

            return new CTable[] { personKilnLock };
        }

        public int DatabaseSchemaVersion()
        {
            return 1;
        }

        public void DatabaseUpgradeBefore(int ixVersionFrom, int ixVersionTo, CDatabaseUpgradeApi apiUpgrade) { }

        public void DatabaseUpgradeAfter(int ixVersionFrom, int ixVersionTo, CDatabaseUpgradeApi apiUpgrade)
        {
            //For the first revision, provide locking info for all users.

            //if admin, set allowAccess = true
            SetDefaultAdmins();

            //if normal, set allow = false
            SetDefaultNormalUsers();
        }

        private void SetDefaultNormalUsers()
        {
            CInsertSelectQuery insert = api.Database.NewInsertSelectQuery(api.Database.PluginTableName(_kilnAccessTable));
            insert.SelectQuery = api.Database.NewSelectQuery("Person");
            insert.SelectQuery.AddSelect("Person.ixPerson");
            insert.SelectQuery.AddWhere("Person.fAdministrator = 0");
            insert.SelectQuery.AddWhere("Person.fCommunity = 0");
            insert.SelectQuery.AddWhere("Person.fVirtual = 0");
            insert.SelectQuery.AddWhere("Person.fConfirmed = 1");
            insert.AddInsertIntoColumn("ixPerson");
            insert.Execute();
        }

        private void SetDefaultAdmins()
        {
            CInsertSelectQuery insert = api.Database.NewInsertSelectQuery(api.Database.PluginTableName(_kilnAccessTable));
            insert.SelectQuery = api.Database.NewSelectQuery("Person");
            insert.SelectQuery.AddSelect("Person.ixPerson");
            insert.SelectQuery.AddWhere("Person.fAdministrator = 1");
            insert.AddInsertIntoColumn("ixPerson");
            insert.Execute();

            //Sets all existing admins to true
            CUpdateQuery update = api.Database.NewUpdateQuery(api.Database.PluginTableName(_kilnAccessTable));
            update.UpdateInt("ixCanAccessKiln", 1);
            update.Execute();
        }
    }
}
