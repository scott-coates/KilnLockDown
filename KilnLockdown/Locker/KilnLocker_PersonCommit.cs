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
    public partial class KilnLocker : IPluginPersonCommit
    {
        public void PersonCommitAfter(CPerson person)
        {
            /*Yay - it worked*/
        }

        public bool PersonCommitBefore(CPerson person)
        {
            if (person.ixPerson < 1) //if we're adding a new user
            {
                SetDefaultKilnAccess(person);
            }
            else
            {
                string sAllowKiln = api.Request[_radioInputName];

                int canAccess = Convert.ToInt32(sAllowKiln);

                person.SetPluginField(PluginId, _ixCanAccessKiln, canAccess);
            }

            return true;
        }

        public void PersonCommitRollback(CPerson person)
        {
            throw new NotImplementedException();
        }
    }
}
