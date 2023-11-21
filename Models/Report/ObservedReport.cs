using System;
using System.Collections.Generic;
using Models.Core;
using Models.PreSimulationTools;
using Models.Storage;

namespace Models
{

    /// <summary>
    /// A ObservedReport class for creation of a report for showing observed and reported data.
    /// </summary>
    [Serializable]
    [ViewName("UserInterface.Views.ReportView")]
    [PresenterName("UserInterface.Presenters.ObservedReportPresenter")]
    [ValidParent(ParentType = typeof(Simulation))]
    public class ObservedReport : Report
    {
        /// <summary>
        /// ColumnNames from ObservedInput.
        /// </summary>
        private string[] ColumnNames { get; set; }

        /// <summary>Link to a storage service.</summary>
        [Link]
        private IDataStore storage = null;

        /// <summary>
        /// Connect event handlers.
        /// </summary>
        /// <param name="sender">Sender object..</param>
        /// <param name="args">Event data.</param>
        [EventSubscribe("SubscribeToEvents")]
        private void OnConnectToEvents(object sender, EventArgs args)
        {
            SubscribeToEvents();
        }

        /// <summary>
        /// Subscribe to events provided
        /// </summary>
        new protected void SubscribeToEvents()
        {
            //VariableNames = new string[] { "[Wheat].Leaf.Wt" };
            ColumnNames = (storage as Model).FindChild<ObservedInput>().ColumnNames;
            EventNames = new string[] { "[Clock].EndOfDay" };
            List<string> confirmedColumnNames = new();
            foreach (string columnName in ColumnNames)
                if (NameMatchesAPSIMModel(columnName) != null)
                    confirmedColumnNames.Add(columnName);

            base.SubscribeToEvents();
        }

        /// <summary>Invoked when a simulation is completed.</summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        [EventSubscribe("Completed")]
        new protected void OnCompleted(object sender, EventArgs e)
        {
            base.OnCompleted(sender, e);
        }

        /// <summary>DO NOT use in pre-sim step, FindByPath uses links that break serialization</summary>
        private IVariable NameMatchesAPSIMModel(string columnName)
        {
            Simulations sims = Parent as Simulations;

            string cleanedName = columnName;
            //strip ( ) out of columns that refer to arrays
            if (columnName.Contains('(') || columnName.Contains(')'))
            {
                int openPos = cleanedName.IndexOf('(');
                cleanedName = cleanedName.Substring(0, openPos);
            }

            string[] nameParts = cleanedName.Split('.');
            IModel firstPart = sims.FindDescendant(nameParts[0]);
            if (firstPart == null)
                return null;

            sims.Links.Resolve(firstPart, true, true, false);
            string fullPath = firstPart.FullPath;
            for (int i = 1; i < nameParts.Length; i++)
                fullPath += "." + nameParts[i];

            IVariable variable = sims.FindByPath(fullPath);
            return null;
        }
    }
}
