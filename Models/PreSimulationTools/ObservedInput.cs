using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using APSIM.Shared.Utilities;
using ExcelDataReader;
using Models.Core;
using Models.Core.Run;
using Models.Storage;

namespace Models.PreSimulationTools
{

    /// <summary>
    /// Reads the contents of a specific sheet from an EXCEL file and stores into the DataStore. 
    /// </summary>
    [Serializable]
    [ViewName("UserInterface.Views.PropertyView")]
    [PresenterName("UserInterface.Presenters.PropertyPresenter")]
    [ValidParent(ParentType = typeof(DataStore))]
    public class ObservedInput : Model, IPreSimulationTool, IReferenceExternalFiles, IObservedInput
    {
        /// <summary>
        /// Stores information about a column in an observed table
        /// </summary>
        public class ColumnInfo 
        {
            /// <summary></summary>
            public string Name;
            /// <summary></summary>
            public string Units;
            /// <summary></summary>
            public bool IsApsimVariable;
            /// <summary></summary>
            public Type DataType;
            /// <summary></summary>
            public List<string> Warnings;
        }

        private string[] filenames;

        /// <summary>
        /// The DataStore.
        /// </summary>
        [Link]
        private IDataStore storage = null;

        /// <summary>
        /// Gets or sets the file name to read from.
        /// </summary>
        [Description("EXCEL file names")]
        [Tooltip("Can contain more than one file name, separated by commas.")]
        [Display(Type = DisplayType.FileNames)]
        public string[] FileNames
        {
            get
            {
                return this.filenames;
            }
            set
            {
                Simulations simulations = FindAncestor<Simulations>();
                if (simulations != null && simulations.FileName != null && value != null)
                    this.filenames = value.Select(v => PathUtilities.GetRelativePath(v, simulations.FileName)).ToArray();
                else
                    this.filenames = value;
            }
        }

        /// <summary>
        /// List of Excel sheet names to read from.
        /// </summary>
        private string[] sheetNames;

        /// <summary>
        /// Gets or sets the list of EXCEL sheet names to read from.
        /// </summary>
        [Description("EXCEL sheet names (csv)")]
        public string[] SheetNames
        {
            get
            {
                return sheetNames;
            }
            set
            {
                if (value == null)
                {
                    sheetNames = new string[0];
                }
                else
                {
                    string[] formattedSheetNames = new string[value.Length];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (Char.IsNumber(value[i][0]))
                            formattedSheetNames[i] = "\"" + value[i] + "\"";
                        else
                            formattedSheetNames[i] = value[i];
                    }

                    sheetNames = formattedSheetNames;
                }
            }
        }

        /// <summary>Return our input filenames</summary>
        public string[] ColumnNames
        {
            get; private set;
        }


        /// <summary>Return our input filenames</summary>
        public IEnumerable<string> GetReferencedFileNames()
        {
            return FileNames.Select(f => f.Trim());
        }

        /// <summary>Remove all paths from referenced filenames.</summary>
        public void RemovePathsFromReferencedFileNames()
        {
            for (int i = 0; i < FileNames.Length; i++)
                FileNames[i] = Path.GetFileName(FileNames[i]);
        }

        /// <summary>
        /// Main run method for performing our calculations and storing data.
        /// </summary>
        public void Run()
        {
            //Clear the tables at the start, since we need to read into them again
            foreach (string sheet in SheetNames)
                if (storage.Reader.TableNames.Contains(sheet))
                    storage.Writer.DeleteTable(sheet);

            foreach (string fileName in FileNames)
            {
                string absoluteFileName = PathUtilities.GetAbsolutePath(fileName.Trim(), storage.FileName);
                if (!File.Exists(absoluteFileName))
                    throw new Exception($"Error in {Name}: file '{absoluteFileName}' does not exist");

                List<DataTable> tables = LoadFromExcel(absoluteFileName);
                foreach (DataTable table in tables)
                {
                    DataTable validatedTable = ValidateColumns(table);

                    // Don't delete previous data existing in this table. Doing so would
                    // cause problems when merging sheets from multiple excel files.
                    storage.Writer.WriteTable(table, false);
                    storage.Writer.WaitForIdle();
                }
            }

            GetAPSIMColumnsFromObserved();
        }
        
        /// <summary>
        /// </summary>
        public List<DataTable> LoadFromExcel(string filepath)
        {
            if (Path.GetExtension(filepath).Equals(".xls", StringComparison.CurrentCultureIgnoreCase))
                throw new Exception($"EXCEL file '{filepath}' must be in .xlsx format.");

            List<DataTable> tables = new List<DataTable>();

            // Open the file
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Reading from a OpenXml Excel file (2007 format; *.xlsx)
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    // Read all sheets from the EXCEL file as a data set.
                    DataSet dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    // Write all sheets that are specified in 'SheetNames' to the data store
                    foreach (DataTable table in dataSet.Tables)
                        if (SheetNames.Any(str => string.Equals(str.Trim(), table.TableName, StringComparison.InvariantCultureIgnoreCase)))
                            tables.Add(table);
                }
            }
            return tables;
        }

        /// <summary>
        /// </summary>
        public DataTable ValidateColumns(DataTable table)
        {
            Simulations sims = this.FindAncestor<Simulations>();
            List<ColumnInfo> infos = new List<ColumnInfo>();

            List<(string, Type)> replaceColumns = new List<(string, Type)>();

            //determine what type each column is
            foreach (DataColumn column in table.Columns) 
            {
                ColumnInfo info = new ColumnInfo();
                info.Name = column.ColumnName;

                info.DataType = null;
                bool stop = false;
                for(int i = 0; i < table.Rows.Count && !stop; i++)
                {
                    DataRow row = table.Rows[i];
                    Type cellType = InputUtilities.GetTypeOfCell(row[column]);

                    //if a cell is a string, then stop because string is the most generic type
                    if (cellType == typeof(string))
                    {
                        info.DataType = cellType;
                        stop = true;
                    }
                    //If it's a double, set this to double, since if it was a string, it would have stopped
                    if (cellType == typeof(double))
                        info.DataType = cellType;

                    //If it's a int, set type to int only if the type is not a double
                    if (cellType == typeof(int) && info.DataType != typeof(double))
                        info.DataType = cellType;

                    //If it's a datetime, set type to datetime only if the type is not a double or int
                    if (cellType == typeof(DateTime) && info.DataType != typeof(double) && info.DataType != typeof(int))
                        info.DataType = cellType;
                }

                IVariable variable = InputUtilities.NameMatchesAPSIMModel(sims, column.ColumnName);
                if (variable != null)
                {
                    info.IsApsimVariable = true;
                    info.Units = variable.UnitsLabel;
                }
                else
                {
                    info.IsApsimVariable = false;
                    info.Units = "";
                }

                info.Warnings = new List<string>();
                infos.Add(info);

                //Check if column is formatted for the wrong type of data
                if (info.DataType != null && column.DataType != info.DataType) {
                    replaceColumns.Add((column.ColumnName, info.DataType));
                }
            }

            foreach ((string, Type) replace in replaceColumns) 
            {
                string name = replace.Item1;
                Type type = replace.Item2;

                DataColumn column = table.Columns[name];
                int ordinal = column.Ordinal;

                DataColumn newColumn = new DataColumn("NewColumn"+name, type);
                table.Columns.Add(newColumn);
                newColumn.SetOrdinal(ordinal);

                foreach (DataRow row in table.Rows)
                {
                    string content = row[name].ToString();
                    if (string.IsNullOrEmpty(content))
                        row[newColumn.ColumnName] = DBNull.Value;
                    else if (type == typeof(DateTime)) 
                        row[newColumn.ColumnName] = DateUtilities.GetDate(content);
                    else if (type == typeof(int))
                        row[newColumn.ColumnName] = int.Parse(content);
                    else if (type == typeof(double)) 
                        row[newColumn.ColumnName] = double.Parse(content);
                    else
                        row[newColumn.ColumnName] = content;
                }

                table.Columns.Remove(name);
                newColumn.ColumnName = name;
            }

            return table;
        }

        /// <summary>From the list of columns read in, get a list of columns that match apsim variables.</summary>
        public void GetAPSIMColumnsFromObserved()
        {
            storage?.Writer.Stop();
            storage?.Reader.Refresh();

            List<string> tableNames = new List<string>();
            List<string> inputNames = new List<string>();
            foreach (string name in SheetNames)
            {
                tableNames.Add(name);
                inputNames.Add(Name);
            }

            List<string> errors = new List<string>();
            List<string> columnNames = new List<string>();
            for (int i = 0; i < tableNames.Count; i++)
            {
                string tableName = tableNames[i];
                DataTable dt = storage.Reader.GetData(tableName);
                string[] columnsNames = dt.GetColumnNames();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string columnName = columnsNames[j];
                    columnNames.Add(columnName);
                }
            }

            ColumnNames = columnNames.ToArray();
            return;
        }

    }
}
