using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace practice4
{


    internal class CSVContainer
    {
        private string[] CSVColumns = new string[] { "region", "state", "area_k_km_q", "population_k" };
        private struct CSVRow
        {
            public string region;
            public string state;
            public decimal area;
            public decimal population;
        };


        private List<CSVRow> _rows = new List<CSVRow>();

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

        public List<string> parseCSVString(string source)
        {
            List<string> list = new List<string>();
            string str = "";

            bool prevIsEcraned = false;
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
                    prevIsEcraned = false;
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
                row.region = values[0];
                row.state = values[1];

                row.area = tryParseDecFromStr(values[2]);
                row.population = tryParseDecFromStr(values[3]);

                _rows.Add(row);
            }

            return true;
        }
        private static string strToCSVVal(string val)
        {
            return Regex.Replace(
                    Regex.Replace(val, ";", "\";\"")
                , "\"", "\"\"");
        }
        public string getStr()
        {
            string s = string.Join(";", CSVColumns).ToLower().Trim() + "\n";

            foreach (var v in _rows)
            {
                s += $"\"{strToCSVVal(v.region)}\";\"{strToCSVVal(v.state)}\";\"{v.area}\";\"{v.population}\"\n";
            }

            return s;
        }
    }
}
