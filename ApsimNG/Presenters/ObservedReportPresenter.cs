using System;
using Models;
using Models.Core;
using Models.Factorial;
using Models.Storage;
using UserInterface.EventArguments;
using UserInterface.Interfaces;
using UserInterface.Presenters;
using UserInterface.Views;

namespace ApsimNG.Presenters
{

    public class ObservedReportPresenter : IPresenter
    {
        /// <summary>
        /// Used by the intellisense to keep track of which editor the user is currently using.
        /// Without this, it's difficult to know which editor (variables or events) to
        /// insert an intellisense item into.
        /// </summary>
        private object currentEditor;

        /// <summary> The ObservedReport object </summary>
        private ObservedReport observedReport;

        /// <summary> The report view</summary>
        private IReportView view;

        /// <summary> The explorer presenter</summary>
        private ExplorerPresenter explorerPresenter;

        /// <summary> The data storage</summary>
        private IDataStore dataStore;

        /// <summary> The data store presenter object</summary>
        private DataStorePresenter dataStorePresenter;

        /// <summary> The intellisense object.</summary>
        private IntellisensePresenter intellisense;

        public void Attach(object model, object view, ExplorerPresenter explorerPresenter)
        {
            this.observedReport = model as ObservedReport;
            this.explorerPresenter = explorerPresenter;
            this.view = view as IReportView;
            this.intellisense = new IntellisensePresenter(view as ViewBase);
            intellisense.ItemSelected += OnIntellisenseItemSelected;

            Simulations simulations = observedReport.FindAncestor<Simulations>();
            if (simulations != null)
            {
                dataStore = simulations.FindChild<IDataStore>();
            }

            dataStorePresenter = new DataStorePresenter(new string[] { observedReport.Name });
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

            dataStorePresenter.Attach(dataStore, this.view.DataStoreView, explorerPresenter);
            this.view.TabIndex = this.observedReport.ActiveTabIndex;

        }

        public void Detach()
        {
            dataStorePresenter?.Detach();
            intellisense.Cleanup();
        }

        /// <summary>
        /// Invoked when the user selects an item in the intellisense.
        /// Inserts the selected item at the caret.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Event arguments.</param>
        private void OnIntellisenseItemSelected(object sender, IntellisenseItemSelectedArgs args)
        {
            if (string.IsNullOrEmpty(args.ItemSelected))
                return;
            else if (string.IsNullOrEmpty(args.TriggerWord))
            {
                if (currentEditor is IEditorView)
                    (currentEditor as IEditorView).InsertAtCaret(args.ItemSelected);
                else
                    (currentEditor as IEditView).InsertAtCursor(args.ItemSelected);
            }
            else
            {
                if (currentEditor is IEditorView)
                    (currentEditor as IEditorView).InsertCompletionOption(args.ItemSelected, args.TriggerWord);
                else
                    (currentEditor as IEditView).InsertCompletionOption(args.ItemSelected, args.TriggerWord);
            }
        }

        /// <summary>
        /// The view is asking for items for its intellisense.
        /// </summary>
        /// <param name="sender">Editor that the user is typing in.</param>
        /// <param name="e">Event Arguments.</param>
        /// <param name="properties">Whether or not property suggestions should be generated.</param>
        /// <param name="methods">Whether or not method suggestions should be generated.</param>
        /// <param name="events">Whether or not event suggestions should be generated.</param>
        private void GetCompletionOptions(object sender, NeedContextItemsArgs e, bool properties, bool methods, bool events)
        {
            try
            {
                string currentLine = GetLine(e.Code, e.LineNo - 1);
                currentEditor = sender;
                if (!e.ControlShiftSpace && intellisense.GenerateGridCompletions(currentLine, e.ColNo, observedReport, properties, methods, events, false, e.ControlSpace))
                    intellisense.Show(e.Coordinates.X, e.Coordinates.Y);
            }
            catch (Exception err)
            {
                explorerPresenter.MainPresenter.ShowError(err);
            }
        }

        /// <summary>
        /// Gets a specific line of text, preserving empty lines.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="lineNo">0-indexed line number.</param>
        /// <returns>String containing a specific line of text.</returns>
        private string GetLine(string text, int lineNo)
        {
            // string.Split(Environment.NewLine.ToCharArray()) doesn't work well for us on Windows - Mono.TextEditor seems 
            // to use unix-style line endings, so every second element from the returned array is an empty string.
            // If we remove all empty strings from the result then we also remove any lines which were deliberately empty.

            // TODO : move this to APSIM.Shared.Utilities.StringUtilities?
            string currentLine;
            using (System.IO.StringReader reader = new System.IO.StringReader(text))
            {
                int i = 0;
                while ((currentLine = reader.ReadLine()) != null && i < lineNo)
                {
                    i++;
                }
            }
            return currentLine;
        }
    }
}
