using System;
using System.Data;
using System.IO;
using System.Linq;

namespace APSIM.Shared.Utilities
{
    /// <summary>
    /// Utilities for working with input files
    /// </summary>
    public class FileUtilities
    {
        /// <summary>
        /// Read in a .csv or .xlsx file and return the contents as a datatable
        /// If file is an excel file, the first sheet will be read.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>Contents ass DataTable</returns>
        public static DataTable ReadDataFile(string filepath)
        {
            return ReadDataFile(filepath, null);
        }

        /// <summary>
        /// Read in a .csv or .xlsx file and return the contents as a datatable
        /// Sheet name is not required for csv, and if not provided for excel, 
        /// the first sheet will be read instead.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="sheetName"></param>
        /// <returns>Contents ass DataTable</returns>
        public static DataTable ReadDataFile(string filepath, string sheetName)
        {
            string extension = Path.GetExtension(filepath).ToLower();
            if (extension == ".csv")
            {
                string sheet = sheetName;
                if (sheet == null)
                    sheet = "Table";
                string fileContents = File.ReadAllText(filepath);
                return DataTableUtilities.FromCSV(sheet, fileContents);
            }
            else if (ExcelUtilities.EXCEL_FORMATS_OPEN.Contains(extension) || ExcelUtilities.EXCEL_FORMATS_OLD.Contains(extension))
            {
                string sheet = sheetName;
                if (sheet == null)
                    sheet = ExcelUtilities.GetWorkSheetNames(filepath).FirstOrDefault();
                if (sheet == null)
                    throw new Exception($"Cannot read a sheet name from Excel file '{filepath}'");

                // Read Properties sheet (header row expected)
                DataTable propsTable = ExcelUtilities.ReadExcelFileData(filepath, sheet, headerRow: true);
                if (propsTable == null)
                    throw new Exception($"Unable to read '{sheet}' from Excel File '{filepath}'");
                return propsTable;
            }
            else
            {
                throw new Exception($"Unable to read '{filepath}', extension '{extension}' not recognised");
            }
        }
    }
}
