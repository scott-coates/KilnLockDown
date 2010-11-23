/* Copyright 2009 Fog Creek Software, Inc. */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Data;

/* FogBugz namespaces-- make sure you add the neccesary assembly references to
 * the following DLL files contained in C:\Program Files\FogBugz\Website\bin\ 
 * FogBugz.dll, FogCreek.Plugins.dll, FogCreek.Plugins.InterfaceEvents.dll     */
using FogCreek.FogBugz.Plugins;
using FogCreek.FogBugz.Plugins.Api;
using FogCreek.FogBugz.Plugins.Entity;
using FogCreek.FogBugz.Plugins.Interfaces;
using FogCreek.FogBugz;
using FogCreek.FogBugz.UI;
using FogCreek.FogBugz.UI.Dialog;
using FogCreek.FogBugz.Database;
using System.Collections;

namespace IPluginPersonJoin_Display_Commit_Example_2
{
    /* Class Declaration: Inherit from Plugin, expose IPluginPersonJoin, IPluginPersonDisplay,
     * IPluginPersonCommit */
    public abstract class IPluginPersonJoin_Display_Commit_Example_2 : Plugin, IPluginPersonJoin,
        IPluginPersonDisplay, IPluginPersonCommit, IPluginDatabase
    {
        /* The plugin Id is a required argument for CPerson.SetPluginField and 
         * CPerson.GetPluginField */

        protected const string sPluginId =
            "IPluginPersonJoin_Display_Commit_Example_2@fogcreek.com";

        /* We'll need this member variable to store the "CodeName" value in the 
         * pre-commit phase, so that if we need to roll back we can replace the original
         * value. */

        protected string preCommitCodeNameLevel = "";

        /* Constructor: We'll just initialize the inherited Plugin class, which 
         * takes the passed instance of CPluginApi and sets its "api" member variable. */
        public IPluginPersonJoin_Display_Commit_Example_2(CPluginApi api)
            : base(api)
        {
        }

        #region IPluginPersonJoin Members

        public string[] PersonJoinTables()
        {
            /* All tables specified here must have an integer ixPerson column so FogBugz can
             * perform the necessary join. */

            return new string[] { "PersonCodeName" };
        }

        #endregion


        public string GetSecretCodeText(CPerson Person)
        {
            if (Person == null)
                return "";

            return Convert.ToString(Person.GetPluginField(sPluginId, "sCodeName"));
        }


        #region IPluginPersonDisplay Members

        public CDialogItem[] PersonDisplayEdit(CPerson Person)
        {
            /* We're returning 2 dialog items: an input text box allowing the user to 
            * enter a code name, and a checkbox that allows the user to force PersonCommitBefore
            * to return false. */

            CDialogItem dItem1 = new CDialogItem();
            dItem1.sLabel = "Secret Code Name";
            dItem1.sContent = Forms.TextInput(api.PluginPrefix + "sCodeName", GetSecretCodeText(Person));

            CDialogItem dItem2 = new CDialogItem();
            dItem2.sLabel = "Commit Behavior (Code Name Plugin)";
            dItem2.sContent =
                Forms.CheckboxInput(api.PluginPrefix + "sCommitFail", "true", false)
                + "Force commit failure";

            return new CDialogItem[] { dItem1, dItem2 };
        }

        public string[] PersonDisplayListFields(CPerson Person)
        {
            string retStr1 = HttpUtility.HtmlEncode(
                Convert.ToString(
                    Person.GetPluginField(sPluginId, "sCodeName")
                )
            );

            return new string[] { retStr1 };
        }

        public string[] PersonDisplayListHeaders()
        {
            return new string[] { "Secret Code Name" };
        }

        #endregion

        #region IPluginPersonCommit Members

        public void PersonCommitAfter(CPerson Person)
        {
            api.Notifications.AddMessage(@"""CodeName"" plugin PersonCommitAfter called");
        }

        public bool PersonCommitBefore(CPerson Person)
        {
            /* Set the preCommitCodeNameLevel member variable */

            if (api.Request[api.AddPluginPrefix("sCodeName")] != null)
            {
                preCommitCodeNameLevel = Convert.ToString(Person.GetPluginField(sPluginId,
                        "sCodeName"));

                Person.SetPluginField(sPluginId, "sCodeName",
                        Convert.ToString(api.Request[api.AddPluginPrefix("sCodeName")]));
            }

            /* If the user checked the "Make commit fail" box, make it fail! */

            if (Convert.ToBoolean(api.Request[api.AddPluginPrefix("sCommitFail")]))
            {
                api.Notifications.AddMessage(
                    @"""CodeName"" plugin returning FALSE from PersonCommitBefore");
                return false;
            }
            else
            {
                api.Notifications.AddMessage(
                    @"""CodeName"" plugin returning TRUE from PersonCommitBefore");
                return true;
            }
        }

        public void PersonCommitRollback(CPerson Person)
        {
            /* Roll back to pre-commit value */
            Person.SetPluginField(sPluginId, "sCodeName", preCommitCodeNameLevel);
            api.Notifications.AddMessage(@"""CodeName"" plugin PersonCommitRollback called");
        }

        #endregion

        #region IPluginDatabase Members

        public CTable[] DatabaseSchema()
        {
            /* for this plugin, we'll need a table repesenting the possible levels of
             * CodeName, and a Person-to-CodeName table to allow for a join */

            CTable PersonCodeName = api.Database.NewTable(api.Database.PluginTableName("PersonCodeName"));
            PersonCodeName.sDesc = "Assigns Persons to Code Names";
            PersonCodeName.AddAutoIncrementPrimaryKey("ixPersonCodeName");
            PersonCodeName.AddIntColumn("ixPerson", true, 1);
            PersonCodeName.AddTextColumn("sCodeName", "The code name of the Person");

            return new CTable[] { PersonCodeName };

        }

        public int DatabaseSchemaVersion()
        {
            return 1;
        }

        public void DatabaseUpgradeAfter(int ixVersionFrom, int ixVersionTo,
            CDatabaseUpgradeApi apiUpgrade)
        {

        }

        public void DatabaseUpgradeBefore(int ixVersionFrom, int ixVersionTo,
            CDatabaseUpgradeApi apiUpgrade)
        {

        }

        #endregion
    }
}