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

namespace IPluginPersonJoin_Display_Commit_Example_1
{
    /* Class Declaration: Inherit from Plugin, expose IPluginPersonJoin, IPluginPersonDisplay,
     * IPluginPersonCommit */
    public abstract class IPluginPersonJoin_Display_Commit_Example_1 : Plugin, IPluginPersonJoin,
        IPluginPersonDisplay, IPluginPersonCommit, IPluginDatabase
    {
        /* The plugin Id is a required argument for CPerson.SetPluginField and 
         * CPerson.GetPluginField */

        protected const string sPluginId =
            "IPluginPersonJoin_Display_Commit_Example_1@fogcreek.com";

        /* We'll need this member variable to store the "awesomeness" value in the 
         * pre-commit phase, so that if we need to roll back we can replace the original
         * value. */

        protected int preCommitAwesomenessLevel = 1;

        /* Constructor: We'll just initialize the inherited Plugin class, which 
         * takes the passed instance of CPluginApi and sets its "api" member variable. */
        public IPluginPersonJoin_Display_Commit_Example_1(CPluginApi api)
            : base(api)
        {
        }

        #region IPluginPersonJoin Members

        public string[] PersonJoinTables()
        {
            /* All tables specified here must have an integer ixPerson column so FogBugz can
             * perform the necessary join. */

            return new string[] { "PersonAwesomeness" };
        }

        #endregion


        #region IPluginPersonDisplay Members

        public CDialogItem[] PersonDisplayEdit(CPerson Person)
        {
            /* We're returning 2 dialog items: a drop-down box allowing the user to 
            * select a level of awesomeness, and a checkbox that allows the user
            * to force PersonCommitBefore to return false. */

            CDialogItem dItem1 = new CDialogItem();
            dItem1.sLabel = "Level of Awesomeness";
            dItem1.sContent = GetAwesomenessSelect(Person);

            CDialogItem dItem2 = new CDialogItem();
            dItem2.sLabel = "Commit Behavior (Awesomeness Plugin)";
            dItem2.sContent =
                Forms.CheckboxInput(api.PluginPrefix + "sCommitFail", "true", false)
                + "Force commit failure";

            return new CDialogItem[] { dItem1, dItem2 };
        }

        public string[] PersonDisplayListFields(CPerson Person)
        {
            string retStr1 = HttpUtility.HtmlEncode(
                GetAwesomenessString(Convert.ToInt32(
                    Person.GetPluginField(sPluginId, "ixAwesomeness"))
                )
            );

            return new string[] { retStr1 };
        }

        public string[] PersonDisplayListHeaders()
        {
            return new string[] { "Level of Awesomeness" };
        }

        #endregion

        protected string GetAwesomenessSelect(CPerson Person)
        {
            List<string> rgsAwesomenessLevels;
            List<string> rgsAwesomenessIxs;

            int ixSelectedIndex = 0;

            CSelectQuery sq = api.Database.NewSelectQuery(
                api.Database.PluginTableName("Awesomeness"));

            sq.AddSelect("*");

            /* Iterate through the data set and populate the drop-down */
            DataSet ds = sq.GetDataSet();

            if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                return string.Empty;

            int numOptions = ds.Tables[0].Rows.Count;

            rgsAwesomenessLevels = new List<string>();
            rgsAwesomenessIxs = new List<string>();

            for (int i = 0; i < numOptions; i++)
            {
                rgsAwesomenessLevels.Add(ds.Tables[0].Rows[i]["sAwesomenessLevel"].ToString());
                rgsAwesomenessIxs.Add(ds.Tables[0].Rows[i]["ixAwesomeness"].ToString());
            }

            /* If there's already an "Awesomeness" value, set the selected index of the drop-down
             * to the proper non-zero value */

            if (Convert.ToInt32(Person.GetPluginField(sPluginId, "ixAwesomeness")) != 0)
                ixSelectedIndex = Convert.ToInt32(
                    Person.GetPluginField(sPluginId, "ixAwesomeness")) - 1;

            return Forms.SelectInput(api.PluginPrefix + "ixAwesomenessSelect",
                rgsAwesomenessLevels.ToArray(), rgsAwesomenessIxs[ixSelectedIndex],
                rgsAwesomenessIxs.ToArray());
        }

        protected string GetAwesomenessString(int ixAwesomeness)
        {
            if (ixAwesomeness < 2)
                return "";

            string retStr = "";

            /* Create the query to get an awesomeness string corresponding to ixAwesomeness */

            CSelectQuery sq =
                api.Database.NewSelectQuery(api.Database.PluginTableName("Awesomeness"));
            sq.AddSelect(api.Database.PluginTableName("Awesomeness") + ".sAwesomenessLevel");
            sq.AddWhere
                (api.Database.PluginTableName("Awesomeness") + ".ixAwesomeness = @ixAwesomeness");
            sq.SetParamInt("ixAwesomeness", ixAwesomeness);

            DataSet ds = sq.GetDataSet();

            if (ds.Tables[0] != null)
                retStr = ds.Tables[0].Rows[0][0].ToString();

            ds.Dispose();
            return retStr;
        }

        #region IPluginPersonCommit Members

        public void PersonCommitAfter(CPerson Person)
        {
            api.Notifications.AddMessage(@"""Awesomeness"" plugin PersonCommitAfter called");
        }

        public bool PersonCommitBefore(CPerson Person)
        {
            /* Set the preCommitAwesomenessLevel member variable */

            if (api.Request[api.AddPluginPrefix("ixAwesomenessSelect")] != null)
            {
                preCommitAwesomenessLevel = Convert.ToInt32(Person.GetPluginField(sPluginId,
                        "ixAwesomeness"));

                if (Convert.ToInt32(api.Request[api.AddPluginPrefix("ixAwesomenessSelect")]) > 0)
                    Person.SetPluginField(sPluginId, "ixAwesomeness",
                        Convert.ToInt32(api.Request[api.AddPluginPrefix("ixAwesomenessSelect")]));
            }

            /* If the user checked the "Make commit fail" box, make it fail! */

            if (Convert.ToBoolean(api.Request[api.AddPluginPrefix("sCommitFail")]))
            {
                api.Notifications.AddMessage(
                    @"""Awesomeness"" plugin returning FALSE from PersonCommitBefore");
                return false;
            }
            else
            {
                api.Notifications.AddMessage(
                    @"""Awesomeness"" plugin returning TRUE from PersonCommitBefore");
                return true;
            }
        }

        public void PersonCommitRollback(CPerson Person)
        {
            /* Roll back to pre-commit value */
            Person.SetPluginField(sPluginId, "ixAwesomeness", preCommitAwesomenessLevel);
            api.Notifications.AddMessage(@"""Awesomeness"" plugin PersonCommitRollback called");
        }

        #endregion

        #region IPluginDatabase Members

        public CTable[] DatabaseSchema()
        {
            /* for this plugin, we'll need a table repesenting the possible levels of
             * Awesomeness, and a Person-to-Awesomeness table to allow for a join */

            CTable Awesomeness = api.Database.NewTable(api.Database.PluginTableName("Awesomeness"));
            Awesomeness.sDesc = "Levels of Awesomeness";
            Awesomeness.AddAutoIncrementPrimaryKey("ixAwesomeness");
            Awesomeness.AddTextColumn("sAwesomenessLevel", "Level of Awesomeness");

            CTable PersonAwesomeness = api.Database.NewTable(api.Database.PluginTableName("PersonAwesomeness"));
            PersonAwesomeness.sDesc = "Assigns Persons to levels of Awesomeness";
            PersonAwesomeness.AddAutoIncrementPrimaryKey("ixPersonAwesomeness");
            PersonAwesomeness.AddIntColumn("ixPerson", true, 1);
            PersonAwesomeness.AddIntColumn("ixAwesomeness", true, 2);

            return new CTable[] { Awesomeness, PersonAwesomeness };

        }

        public int DatabaseSchemaVersion()
        {
            return 1;
        }

        public void DatabaseUpgradeAfter(int ixVersionFrom, int ixVersionTo,
            CDatabaseUpgradeApi apiUpgrade)
        {
            /* Create 5 different awesomeness levels. Note that we'll have a "None 
             * specified" level, which our plugin will treat the same as NULL. */

            string[] AwesomenessLevels = 
                { "-- None specified --", "Low", "Moderate", "High", "Extreme" };

            for (int i = 0; i < AwesomenessLevels.Length; i++)
            {
                CInsertQuery iq = api.Database.NewInsertQuery(
                    api.Database.PluginTableName("Awesomeness"));

                iq.InsertString("sAwesomenessLevel", AwesomenessLevels[i]);
                iq.Execute();
            }
        }

        public void DatabaseUpgradeBefore(int ixVersionFrom, int ixVersionTo,
            CDatabaseUpgradeApi apiUpgrade)
        {

        }

        #endregion

    }
}