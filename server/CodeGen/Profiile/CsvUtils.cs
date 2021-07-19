using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;

namespace Script
{
    public class CsvHelper
    {
        List<string[]> lines;
        int rowIndex;
        Dictionary<string, int> name2ColumnIndex;
        int startColumn;
        int endColumn;
        public CsvHelper(string[] headers, List<string[]> lines, int startColumn = -1, int endColumn = -1)
        {

            if (startColumn == -1 || endColumn == -1)
            {
                startColumn = 0;
                endColumn = headers.Length - 1;
            }
            this.startColumn = startColumn;
            this.endColumn = endColumn;

            this.name2ColumnIndex = new Dictionary<string, int>();
            for (int i = startColumn; i <= endColumn; i++)
            {
                this.name2ColumnIndex.Add(headers[i], i);
            }

            this.lines = lines;
            this.rowIndex = -1;
        }

        public void ResetRowIndex()
        {
            this.rowIndex = -1;
        }

        string[] currentRow;
        public bool ReadRow()
        {
            this.rowIndex++;
            if (this.rowIndex >= this.lines.Count)
            {
                return false;
            }
            this.currentRow = this.lines[this.rowIndex];
            for (int i = startColumn; i <= endColumn; i++)
            {
                if (!string.IsNullOrEmpty(this.currentRow[i]))
                {
                    return true;
                }
            }
            return false;
        }

        string GetCell(string name)
        {
            int columnIndex;
            if (!this.name2ColumnIndex.TryGetValue(name, out columnIndex))
            {
                return null;
            }
            return this.currentRow[columnIndex];
        }

        public string ReadString(string name)
        {
            return this.GetCell(name);
        }

        public int ReadInt(string name, int default_ = 0)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return int.Parse(cell);
        }

        public BigInteger ReadBigInteter(string name, BigInteger default_)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return BigInteger.Parse(cell);
        }

        public float ReadFloat(string name, float default_ = 0f)
        {
            var cell = this.GetCell(name);
            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }
            return float.Parse(cell);
        }
/*
        public T ReadObject<T>(string name, T default_ = null) where T : class
        {
            var cell = this.GetCell(name);
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
*/
        public T ReadEnum<T>(string name) where T : Enum
        {
            var cell = this.GetCell(name);
            return (T)Enum.Parse(typeof(T), cell);
        }

        public T ReadEnum<T>(string name, T default_) where T : Enum
        {
            var cell = this.GetCell(name);

            if (string.IsNullOrEmpty(cell))
            {
                return default_;
            }

            return (T)Enum.Parse(typeof(T), cell);
        }
    }

    public class CsvUtils
    {
        public const string IGNORE_LINE_FLAG = "#";
        public const string NULL_CELL_FLAG = "NULL";
        public const char CELL_SPLITER = ',';
        public const char COMMA_REPLACEMENT = '|';

        public static void SplitToLines(string text, Action<string[]> handleLine)
        {
            string[] lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith(IGNORE_LINE_FLAG))
                {
                    continue;
                }
                string[] cells = line.Split(CELL_SPLITER);
                handleLine(cells);
            }
        }

        public static CsvHelper Parse(string text)
        {
            string[] headers = null;
            List<string[]> lines = new List<string[]>();
            SplitToLines(text, line =>
            {
                if (headers == null)
                    headers = line;
                else
                    lines.Add(line);
            });
            return new CsvHelper(headers, lines);
        }

        // parts ����0,2,4,6,8,10
        // [0,2]����һ������[4,6]��һ������[8,10]��һ����
        public static Dictionary<string, CsvHelper> ParseMultiple(string text, params int[] parts)
        {
            string[] tableNames = null;
            string[] headers = null;
            List<string[]> lines = new List<string[]>();
            SplitToLines(text, line =>
            {
                if (tableNames == null)
                    tableNames = line;
                else if (headers == null)
                    headers = line;
                else
                    lines.Add(line);
            });

            var dict = new Dictionary<string, CsvHelper>();
            for (int i = 0; i < parts.Length; i += 2)
            {
                var tableName = tableNames[parts[i]];
                var helper = new CsvHelper(headers, lines, parts[i], parts[i + 1]);
                dict.Add(tableName, helper);
            }

            return dict;
        }
    }
}

