using System;
using System.Collections.Generic;
using Android.Views;
using System.IO;

namespace Lab7Sqlite
{
    public class TextParser
    {
        string delimiter;
        int numFields;

        public TextParser(string columnDelimiter, int numberOfFields)
        {
            delimiter = columnDelimiter;
            numFields = numberOfFields;
        }

        public List<string[]> ParseText(Stream stream)
        {
            List<string[]> rows = new List<string[]>();

            string[] delim = new string[1];
            delim[0] = delimiter;

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    rows.Add(reader.ReadLine().Split(delim, numFields, StringSplitOptions.None));
                }
            }

            return rows;
        }
    }
}