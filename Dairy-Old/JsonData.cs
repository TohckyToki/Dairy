using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Dairy {
    public static class JsonData {
        public static string ToJsonString(DataTable data) {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (DataRow row in data.Rows) {
                sb.Append("{");
                foreach (DataColumn col in data.Columns) {
                    if (col.DataType == typeof(DateTime)) {
                        sb.Append($"{col.ColumnName}: '{row.Field<DateTime>(col.ColumnName).ToShortDateString()}',");
                    } else {
                        sb.Append($"{col.ColumnName}: '{row.Field<object>(col.ColumnName)?.ToString()}',");
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        public static bool ToDbData(string str, ref DataTable table) {
            JObject[] arr;
            try {
                arr = JsonConvert.DeserializeObject<JObject[]>(str);
            } catch (Exception) {
                return false;
            }
            if (!CheckIsReadable(arr, table)) {
                return false;
            }
            var dt = new DataTable();
            foreach (DataColumn col in table.Columns) {
                dt.Columns.Add(col.ColumnName, typeof(string));
            }
            foreach (dynamic row in arr) {
                dt.Rows.Add();
                foreach (DataColumn col in table.Columns) {
                     if (col.DataType == typeof(bool)) {
                        dt.Rows[dt.Rows.Count - 1].SetField(col.ColumnName, (string)row[col.ColumnName]);
                    } else {
                        dt.Rows[dt.Rows.Count - 1].SetField(col.ColumnName, $"'{(string)row[col.ColumnName]}'");
                    }
                }
            }
            table = dt;
            return true;
        }

        private static bool CheckIsReadable(JObject[] arr, DataTable table) {
            if (arr.Length <= 0) return false;
            var result = true;
            for (int i = 0; i < table.Columns.Count; i++) {
                var item = table.Columns[i];
                if (!arr[0].ContainsKey(item.ColumnName)) {
                    result = false;
                    break;
                }
            }
            return result;
        }

    }
}
