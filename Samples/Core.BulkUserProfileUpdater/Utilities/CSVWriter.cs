using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Contoso.Core.Utilities
{
    /// <summary>
    /// Stores single row within the CSV file
    /// </summary>
    public class CSVUserEntry : List<string>
    {
        public string CellEntry
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Processes the output CSV
    /// </summary>
    public class CSVWriter : StreamWriter
    {
        #region Methods

        public CSVWriter(string filename)
            : base(filename) { }

        /// <summary>
        /// Executes the specified CSV writer.
        /// </summary>
        /// <param name="userdata">The user entry instance.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">If the userdata instance is null</exception>
        public void CSVWriteUser(CSVUserEntry userdata, LogHelper logger)
        {
            try
            {
                StringBuilder entry = new StringBuilder();

                bool FirstEntry = true;

                foreach (string cell in userdata)
                {
                    // Insert delimiter if not first column
                    if (!FirstEntry)
                        entry.Append(',');

                    // Insert value into builder
                    if (cell.IndexOfAny(new char[] { '"', ',' }) != -1)
                        entry.AppendFormat("\"{0}\"", cell.Replace("\"", "\"\""));
                    else
                        entry.Append(cell);

                    // Update first column flag
                    FirstEntry = false;
                }

                userdata.CellEntry = entry.ToString();

                // Output string to CSV file
                WriteLine(userdata.CellEntry);

            }
            catch (Exception ex)
            {
                logger.LogException(string.Empty, ex);
            }            
            
        }

        #endregion Methods


    }
}
