namespace Contoso.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Processes the input CSV
    /// </summary>
    public class CsvProcessor
    {
        #region Fields

        /// <summary>
        /// The delimiter as string
        /// </summary>
        private string delimiterAsString;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        private char Delimiter
        {
            get
            {
                return this.DelimiterAsString[0];
            }
        }

        /// <summary>
        /// Gets or sets the delimiter as string.
        /// </summary>
        /// <value>
        /// The delimiter as string.
        /// </value>
        private string DelimiterAsString
        {
            get
            {
                if (this.delimiterAsString == null)
                {
                    try
                    {
                        this.delimiterAsString = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                        if (this.delimiterAsString.Length != 1)
                        {
                            this.delimiterAsString = ",";
                        }
                    }
                    catch (Exception)
                    {
                        this.delimiterAsString = ",";
                    }
                }

                return this.delimiterAsString;
            }

            set
            {
                this.delimiterAsString = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Executes the specified reader.
        /// </summary>
        /// <param name="reader">The reader instance.</param>
        /// <param name="action">The logic to execute.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">If the reader instance is null</exception>
        public void Execute(TextReader reader, Action<Collection<string>, LogHelper> action, LogHelper logger)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            int lineNum = -1;

            try
            {
                string line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNum++;

                    if (lineNum == 0)
                    {
                        string[] separator = new string[] { this.DelimiterAsString };
                        string[] commaSeperator = new string[] { "," };

                        int length = line.Split(separator, StringSplitOptions.None).Length;

                        if (line.Split(commaSeperator, StringSplitOptions.None).Length > length)
                        {
                            this.DelimiterAsString = commaSeperator[0];
                        }
                    }
                    else if ((lineNum <= 1) || !string.IsNullOrEmpty(line.Trim()))
                    {
                        try
                        {
                            Collection<string> entries = this.ParseLineIntoEntries(line);
                            action.Invoke(entries, logger);
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(string.Empty, ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogException(string.Empty, ex);
            }
        }

        /// <summary>
        /// Parses the line into entries.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>A collection of columns</returns>
        private Collection<string> ParseLineIntoEntries(string line)
        {
            Collection<string> list = new Collection<string>();
            char[] lineArray = line.ToCharArray();
            string str = string.Empty;
            bool flag = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (!flag && string.IsNullOrEmpty(str))
                {
                    if (char.IsWhiteSpace(lineArray[i]))
                    {
                        continue;
                    }

                    if (lineArray[i] == '"')
                    {
                        flag = true;
                        continue;
                    }
                }

                if (flag && (lineArray[i] == '"'))
                {
                    if (((i + 1) < line.Length) && (lineArray[i + 1] == '"'))
                    {
                        i++;
                    }
                    else
                    {
                        if (((i + 1) < line.Length) && (lineArray[i + 1] != this.Delimiter))
                        {
                            return null;
                        }

                        flag = false;
                        continue;
                    }
                }

                if (flag || (lineArray[i] != this.Delimiter))
                {
                    str = str + lineArray[i];
                }
                else
                {
                    str = str.Trim();
                    list.Add(str);
                    str = string.Empty;
                }
            }

            str = str.Trim();
            list.Add(str);

            return list;
        }

        #endregion Methods
    }
}