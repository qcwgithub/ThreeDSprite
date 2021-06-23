using System;
using System.Collections;
using System.Collections.Generic;

namespace Script
{
    public class CsvHelper
    {
        List<string[]> lines;
        int rowIndex;
        Dictionary<string, int> name2ColumnIndex;
        public CsvHelper(string[] headers, List<string[]> lines)
        {
            this.name2ColumnIndex = new Dictionary<string, int>();
            for (int i = 0; i < headers.Length; i++)
            {
                this.name2ColumnIndex.Add(headers[i], i);
            }

            this.lines = lines;
            this.rowIndex = -1;
        }

        string[] currentRow;
        public bool readRow()
        {
            this.rowIndex++;
            if (this.rowIndex >= this.lines.Count)
            {
                return false;
            }
            this.currentRow = this.lines[this.rowIndex];
            return true;
        }

        string getCell(string name)
        {
            int columnIndex;
            if (!this.name2ColumnIndex.TryGetValue(name, out columnIndex))
            {
                return null;
            }
            return this.currentRow[columnIndex];
        }

        public string readString(string name)
        {
            return this.getCell(name);
        }

        public int readInt(string name, int default_ = 0)
        {
            var cell = this.getCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return int.Parse(cell);
        }

        public float readFloat(string name, float default_ = 0f)
        {
            var cell = this.getCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return float.Parse(cell);
        }
        
        public T readObject<T>(string name, T default_ = null) where T : class
        {
            var cell = this.getCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            if (cell.IndexOf(CsvUtils.COMMA_REPLACEMENT) >= 0)
            {
                cell = cell.Replace(CsvUtils.COMMA_REPLACEMENT, ',');
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cell);
        }

        public T readEnum<T>(string name) where T : Enum
        {
            var cell = this.getCell(name);
            return (T)Enum.Parse(typeof(T), cell);
        }
    }

    public class CsvUtils
    {
        public const string IGNORE_LINE_FLAG = "#";
        public const char CELL_SPLITER = ',';
        public const char COMMA_REPLACEMENT = '|';

        public static CsvHelper parse(string text)
        {
            string[] lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] headers = null;
            List<string[]> lines2 = new List<string[]>();
            foreach (var line in lines)
            {
                if (line.StartsWith(IGNORE_LINE_FLAG))
                {
                    continue;
                }
                string[] cells = line.Split(CELL_SPLITER);
                if (headers == null)
                {
                    headers = cells;
                    continue;
                }

                lines2.Add(cells);
            }
            return new CsvHelper(headers, lines2);
        }
    }
}
