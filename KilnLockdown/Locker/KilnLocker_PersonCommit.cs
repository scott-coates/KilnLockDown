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
    public partial class KilnLocker : IPluginPersonCommit
    {
        public void PersonCommitAfter(CPerson person)
        {
            /*Yay - it worked*/
        }

        public bool PersonCommitBefore(CPerson person)
        {
            return true;
        }

        public void PersonCommitRollback(CPerson person)
        {
            throw new NotImplementedException();
        }
    }
}
