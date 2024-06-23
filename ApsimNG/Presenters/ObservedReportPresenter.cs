
using System;
using Models;
using Models.Core;
using Models.Factorial;
using Models.Storage;
using UserInterface.Presenters;
using UserInterface.Views;

namespace UserInterface.Presenters
{
    public class ObservedReportPresenter: IPresenter
    {
        /// <summary> The explorer presenter</summary>
        private ObservedReport observedReport;
        private ExplorerPresenter explorerPresenter;

        private ObservedReportView observedReportView;

        /// <summary> The data storage</summary>
        private IDataStore dataStore;

        /// <summary> The data store presenter object</summary>
        private DataStorePresenter dataStorePresenter;

        private PropertyPresenter propertyPresenter;

        public void Attach(object model, object view, ExplorerPresenter explorerPresenter)
        {
            this.explorerPresenter = explorerPresenter;
            observedReport = model as ObservedReport;
            observedReportView = view as ObservedReportView;

            Simulations simulations = observedReport.FindAncestor<Simulations>();
            if (simulations != null)
            {
                dataStore = simulations.FindChild<IDataStore>();
            }

            dataStorePresenter = new DataStorePresenter(new string[] { observedReport.Name });
            propertyPresenter = new PropertyPresenter();

            Simulation simulation = observedReport.FindAncestor<Simulation>();
            Experiment experiment = observedReport.FindAncestor<Experiment>();
            Zone paddock = observedReport.FindAncestor<Zone>();

            // Only show data which is in scope of this report.
            // E.g. data from this zone and either experiment (if applicable) or simulation.
            if (paddock != null)
                dataStorePresenter.ZoneFilter = paddock;
            if (experiment != null)
                dataStorePresenter.ExperimentFilter = experiment;
            else if (simulation != null)
                dataStorePresenter.SimulationFilter = simulation;

            dataStorePresenter.Attach(dataStore, this.observedReportView.DataStoreView, explorerPresenter);
            this.observedReportView.TabIndex = this.observedReport.ActiveTabIndex;
        }

        public void Detach()
        {
            observedReport.ActiveTabIndex = observedReportView.TabIndex;
            dataStorePresenter?.Detach();
        }


    }


}