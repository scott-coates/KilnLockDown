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
using System.Web;

namespace KilnLockdown.Locker
{
    public partial class KilnLocker : IPluginConfigPageDisplay
    {
        public string ConfigPageDisplay()
        {
            string sHTML = "";
            sHTML += GetHeader();
            sHTML += GetForm();
            return sHTML;
        }

        protected string GetForm()
        {
            /* Generate a form that will post a value to the server and display
             * that value taken from the request object (if it exists).
             * 
             * IMPORTANT NOTE: The unique prefix of this plugin must be prepended to the
             * names of all form elements, but the values of those elements can then be
             * gathered from the request object without using the prefix.
             * 
             * In keeping with security best practices, this plugin does not display
             * unless a required action token validates.
             * 
             * In this case, the "kiln url" value is posted and retrieved. */

            string kilnURL = GetUrlForForm();

            return String.Format(
                @"<form action=""{0}"" method=""POST"">
                        <input type=""hidden"" name=""{1}actionToken"" value=""{2}"">
                        <p>{3} {4} {5}</p>
                </form>",

                HttpUtility.HtmlAttributeEncode(api.Url.PluginConfigPageUrl()),
                api.PluginPrefix,
                api.Security.GetActionToken(),
                Forms.Label(api.PluginPrefix + "info", "Kiln Installation URL:"),
                Forms.TextInput(api.PluginPrefix + "kilnURL", kilnURL),
                Forms.SubmitButton("submit", "Submit")
                );
        }

        private string GetUrlForForm()
        {
            string originalKilnURL = GetKilnInstallationURL();
            string kilnURL = string.Empty;

            if (api.Request[api.AddPluginPrefix(_kilnUrl)] != null)
            {
                if ((api.Request[api.AddPluginPrefix("actionToken")] != null) && api.Security.ValidateActionToken(api.Request[api.AddPluginPrefix("actionToken")].ToString()))
                {
                    kilnURL = HttpUtility.HtmlEncode(api.Request[api.AddPluginPrefix(_kilnUrl)].ToString());

                    if (string.IsNullOrEmpty(kilnURL))
                    {
                        api.Notifications.AddError("The Kiln URL cannot be blank.");
                    }
                    else
                    {
                        SetKilnInstallationURLAndDisplay(kilnURL);
                    }
                }
                else
                {
                    api.Notifications.AddError("Invalid token!");
                }
            }

            if (string.IsNullOrEmpty(kilnURL))
            {
                kilnURL = originalKilnURL;
            }
            return kilnURL;
        }

        protected string GetHeader()
        {
            string retVal = String.Format("<p>{0}</p>", PageDisplay.Headline("Kiln Lockdown Configuration"));

            retVal += "<p>Provide your kiln installation URL.  This URL will be blocked for users who don't have kiln access. </p>";

            return retVal;
        }
    }
}