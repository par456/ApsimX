using System;
using Models.Core;
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
        // /// <summary>Link to a storage service.</summary>
        //[Link]
        //private IDataStore storage = null;

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
            //(storage as Model).FindChild<ObservedInput>().ColumnNames;
            this.VariableNames = new string[] { "[Wheat].Leaf.Wt" };
            this.EventNames = new string[] { "[Clock].EndOfDay" };

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
    }
}
