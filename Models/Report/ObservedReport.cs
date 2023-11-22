using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Models.Core;
using Models.PreSimulationTools;
using Models.Storage;

namespace Models
{

    /// <summary>
    /// A ObservedReport class for creation of a report for showing observed and reported data.
    /// </summary>
    [Serializable]
    [ViewName("UserInterface.Views.PropertyView")]
    [PresenterName("UserInterface.Presenters.PropertyPresenter")]
    [ValidParent(ParentType = typeof(Simulation))]
    public class ObservedReport : Report
    {
        /// <summary>Link to a storage service.</summary>
        [Link]
        private IDataStore storage = null;

        private ObservedInput observedInput = null;

        /// <summary> First field name used for match.</summary>
        private string fieldNameUsedForMatch;

        /// <summary>
        /// Gets or sets the file name to read from.
        /// </summary>
        [Description("Report Frequency:")]
        [Tooltip("Event this report collects data during")]
        [Display(Type = DisplayType.None)]
        public string eventFrequency { get; set; }

        /// <summary>Gets or sets the field name used for match.</summary>
        [Description("Filter for Rows with Column:")]
        [Tooltip("When filtering columns to record, only rows with a value in the provided field will be considered")]
        [Display(Type = DisplayType.DropDown, Values = nameof(GetColumnNames))]
        public string FieldNameUsedForMatch
        {
            get { return fieldNameUsedForMatch; }
            set
            {
                if (value == "")
                    fieldNameUsedForMatch = null;
                else fieldNameUsedForMatch = value;
            }
        }

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
            observedInput = (storage as Model).FindChild<ObservedInput>();

            List<string> columns = observedInput.ColumnNames.ToList();
            columns = RemoveColumnsThatHaveNoData(columns);
            columns = RemoveExtraArrayColumns(columns);
            columns = RemoveNonAPSIMVariables(columns);
            columns = AddSquareBracketsToColumnName(columns);

            VariableNames = columns.ToArray();
            EventNames = new string[] { eventFrequency };

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
            Simulation simulation = FindAncestor<Simulation>();

            string cleanedName = columnName;
            //strip ( ) out of columns that refer to arrays
            if (columnName.Contains('(') || columnName.Contains(')'))
            {
                int openPos = cleanedName.IndexOf('(');
                cleanedName = cleanedName.Substring(0, openPos);
            }

            string[] nameParts = cleanedName.Split('.');
            IModel firstPart = simulation.FindDescendant(nameParts[0]);
            if (firstPart == null)
                return null;

            string fullPath = firstPart.FullPath;
            for (int i = 1; i < nameParts.Length; i++)
                fullPath += "." + nameParts[i];

            IVariable variable = simulation.FindByPath(fullPath);
            return variable;
        }

        private List<string> AddSquareBracketsToColumnName(List<string> columnNames)
        {
            List<string> formattedColumnNames = new();
            foreach (string columnName in columnNames)
            {
                string modelName = "[" + columnName;
                modelName = modelName.Insert(modelName.IndexOf("."), "]");
                formattedColumnNames.Add(modelName);

            }
            return formattedColumnNames;
        }

        private List<string> RemoveExtraArrayColumns(List<string> columnNames)
        {
            List<string> newColumnNames = new();
            foreach (string columnName in columnNames)
            {
                if (columnName.Contains("(") && columnName.Contains(")"))
                {
                    string newName = columnName.Split("(")[0];
                    if (!newColumnNames.Contains(newName))
                    {
                        newColumnNames.Add(newName);
                    }
                }
                else
                {
                    newColumnNames.Add(columnName);
                }
            }
            return newColumnNames;
        }

        private string BuildQueryForSimulationData(string observedInputName, string simulationName)
        {
            string query = string.Empty;
            query += "SELECT *\n";
            query += "FROM " + observedInputName + "\n";
            query += "WHERE SimulationID = " + simulationName + "\n";
            return query;
        }

        private List<string> RemoveColumnsThatHaveNoData(List<string> columnNames)
        {
            Simulation sim = FindAncestor<Simulation>();

            List<string> newColumnNames = new List<string>();
            for (int i = 0; i < observedInput.SheetNames.Length; i++)
            {
                List<string> names = new List<string> { sim.Name };
                List<int> ids = storage.Reader.ToSimulationIDs(names).ToList();

                string query = BuildQueryForSimulationData(observedInput.SheetNames[i], ids[0].ToString());
                DataTable predictedObservedData = storage.Reader.GetDataUsingSql(query);

                for (int j = 0; j < predictedObservedData.Columns.Count; j++)
                {
                    DataColumn col = predictedObservedData.Columns[j];
                    string colName = col.ColumnName;
                    int index = columnNames.IndexOf(colName);
                    if (index > -1 && !newColumnNames.Contains(colName))
                    {
                        bool hasData = false;
                        for (int k = 0; k < predictedObservedData.Rows.Count && !hasData; k++)
                        {
                            string value = predictedObservedData.Rows[k][col].ToString();
                            if (value.Length > 0)
                                hasData = true;
                        }
                        if (hasData)
                            newColumnNames.Add(colName);
                    }
                }
            }
            return newColumnNames;
        }

        private List<string> RemoveNonAPSIMVariables(List<string> columnNames)
        {
            List<string> newColumnNames = new List<string>();
            foreach (string columnName in columnNames)
                if (NameMatchesAPSIMModel(columnName) != null)
                    newColumnNames.Add(columnName);
            return newColumnNames;
        }

        /// <summary>
        /// Returns all list of column names
        /// </summary>
        public string[] GetColumnNames()
        {
            ObservedInput obs = (this.storage as Model).FindChild<ObservedInput>();
            string[] columnNames = obs.ColumnNames;
            return columnNames;
        }
    }
}
