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
    public abstract partial class KilnLocker : IPluginStaticJS
    {
        public string[] StaticJSFiles()
        {
            string[] retVal = new string[]{};

            var person = api.Person.GetCurrentPerson();
            if (IsEligible(person))
            {
                if (!PersonHasKilnAccess(person).GetValueOrDefault())
                {
                    if (InKiln())
                    {
                        //if the user is trying to get to kiln, redirect them to homepage
                        retVal = new string[] { "js/BlockKiln.js" };
                    }
                    else
                    {
                        //if the user is just on fogbugz, hide the kiln tab
                    }
                }
            }

            return retVal;
        }

        private bool InKiln()
        {
            var retVal = false;

            if (api.Url.BaseUrl().ToLower().Contains("kiln"))
            {
                retVal = true;
            }

            return retVal;
        }
    }
}
