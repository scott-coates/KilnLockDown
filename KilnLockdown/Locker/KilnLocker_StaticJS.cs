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
    public partial class KilnLocker : IPluginStaticJS
    {
        public string[] StaticJSFiles()
        {
            string[] retVal = new string[] { };

            var person = api.Person.GetCurrentPerson();

            if (IsEligible(person))
            {
                if (!PersonHasKilnAccess(person).GetValueOrDefault())
                {
                    retVal = new string[] { "js/BlockKiln.js", "js/HideKiln.js" };
                }
            }

            return retVal;
        }
    }
}
