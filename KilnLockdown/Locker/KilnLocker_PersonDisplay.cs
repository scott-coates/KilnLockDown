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
using System.Collections;

namespace KilnLockdown.Locker
{
    public partial class KilnLocker : IPluginPersonDisplay
    {
        public CDialogItem[] PersonDisplayEdit(CPerson person)
        {
            CDialogItem[] retVal = null;

            //only if editing existing user
            if (person.ixPerson > 0 && api.Person.GetCurrentPerson().fAdministrator)
            {
                var allowKiln = new CDialogItem();

                retVal = new CDialogItem[] { allowKiln };

                allowKiln.sLabel = "Kiln Access";
                allowKiln.sInstructions = "Is this user allowed to use Kiln?";

                if (IsEligible(person))
                {
                    var personHasKilnAccess = PersonHasKilnAccess(person).GetValueOrDefault();

                    allowKiln.sContent =
                        Forms.RadioInput(_radioInputName, "1", personHasKilnAccess, "Yes", "sAllowKilnYes")
                         +
                         Forms.RadioInput(_radioInputName, "0", !personHasKilnAccess, "No", "sAllowKilnNo")
                         ;
                }
                else
                {
                    //set default setting to yes if admin and no if normal user
                    var setDefault = person.IsSiteAdmin();
                    var enabledAttrs = new Dictionary<string, string> { { "disabled", "true" } };

                    allowKiln.sContent =
                        Forms.RadioInput(_radioInputName, "1", setDefault, "Yes", "sAllowKilnYes", enabledAttrs)
                         +
                         Forms.RadioInput(_radioInputName, "0", !setDefault, "No", "sAllowKilnNo", enabledAttrs)
                         ;

                    if (!api.SiteConfiguration.IsKilnEnabled)
                    {
                        allowKiln.sInstructions += " (This option is not configurable until Kiln is installed)";
                    }
                }
            }

            return retVal;
        }

        public string[] PersonDisplayListFields(CPerson person)
        {
            return new string[] { PersonKilnAccessString(person) };
        }

        public string[] PersonDisplayListHeaders()
        {
            return new string[] { "Can Access Kiln" };
        }
    }
}
