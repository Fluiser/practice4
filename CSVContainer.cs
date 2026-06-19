using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace practice4
{


    public class CSVContainer
    {
        private string[] CSVColumns = new string[] { "id", "region", "state", "area_k_km_q", "population_k" };
        public struct CSVRow
        {
            public int id;
            public string region;
            public string state;
            public decimal area;
            public decimal population;
        };


        private List<CSVRow> _rows = new List<CSVRow>();

        public List<CSVRow> rows => _rows;

        Decimal tryParseDecFromStr(string str)
        {
            try
            {
                return Decimal.Parse(str);
            } catch(FormatException)
            {
                return new Decimal(0);
            }
        }

        public void addRecord(CSVRow row)
        {
            _rows.Add(row);
        }

        public void replaceRecord(int index, CSVRow row)
        {
            _rows[index] = row;
        }

        public void deleteRecord(int index)
        {
            _rows.RemoveAt(index);
        }

        public void replaceRows(List<CSVRow> rows)
        {
            _rows = rows;
        }

        public List<string> parseCSVString(string source)
        {
            List<string> list = new List<string>();
            string str = "";

            bool isBegin = true;
            
            for(int i = 0; i < source.Length - 1; ++i)
            {
                char c = source[i];
                char nextC = source[i + 1];

                if(c == '"' && nextC == ';')
                {
                    ++i;
                    str += c;
                    list.Add(str);
                    str = "";
                    isBegin = true;
                } else if(c == '"' && nextC == '"' && !isBegin)
                {
                    ++i;
                    str += c;
                } else
                {
                    str += c;
                    isBegin = false;
                }
            }

            list.Add(str);

            return list;
        }

        public bool parse(string text)
        {
            if (text == null) return false;

            string[] rows = text.Split('\n');

            if (rows.Length < 1 || rows[0].ToLower().Trim() != string.Join(";", CSVColumns).ToLower().Trim()) return false;

            for(int i = 1; i < rows.Length; ++i)
            {
                string[] values = parseCSVString(rows[i]).ToArray();

                for (int v = 0; v < values.Length; ++v)
                    values[v] = Regex.Replace(values[v], "^\"|\"$", "");

                if (values.Length != CSVColumns.Length) continue;
                

                CSVRow row = new CSVRow();

                int.TryParse(values[0], out row.id);
                row.region = values[1];
                row.state = values[2];

                row.area = tryParseDecFromStr(values[3]);
                row.population = tryParseDecFromStr(values[4]);

                _rows.Add(row);
            }

            return true;
        }
        public void clear()
        {
            _rows.Clear();
        }

        private static string strToCSVVal(string val)
        {
            return Regex.Replace(
                    val
                , "\"", "\"\"");
        }
        public string getString()
        {
            string s = string.Join(";", CSVColumns).ToLower().Trim() + "\n";

            foreach (var v in _rows)
            {
                s += $"\"{v.id}\";\"{strToCSVVal(v.region)}\";\"{strToCSVVal(v.state)}\";\"{v.area}\";\"{v.population}\"\n";
            }

            return s;
        }
    }
}
