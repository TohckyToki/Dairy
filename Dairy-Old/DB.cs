using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using System.Data.SQLite.Linq;

namespace Dairy {
    public class DB {
        private SQLiteConnection cnn;
        private SQLiteCommand cmd;
        private SQLiteDataAdapter adpt;

        private DataTable dt;

        public DB() {
            cnn = new SQLiteConnection(Properties.Settings.Default.ConnectionString);
            cmd = new SQLiteCommand(cnn);
            adpt = new SQLiteDataAdapter(cmd);
            dt = new DataTable();
            cnn.Open();
        }

        public DataTable GetTable(bool readData = true) {
            var sql = $"select * from dairy{(readData ? "" : " where 1 <> 1")};";
            cmd.CommandText = sql;
            dt.Clear();
            adpt.Fill(dt);
            return dt;
        }

        public int UpdateTable(DataTable table) {
            var sql = $"insert or replace into dairy({string.Join(",", from DataColumn col in table.Columns select col.ColumnName)}) values {string.Join(",", from DataRow row in table.Rows select $"({string.Join(",", row.ItemArray)})")}";
            cmd.CommandText = sql;
            return RunTransaction();
        }

        public int GetPageCount() {
            var sql = "select count(rowid) from dairy where enable = true;";
            cmd.CommandText = sql;
            var dataCount = (long)cmd.ExecuteScalar();
            return (int)Math.Ceiling((double)dataCount / Properties.Settings.Default.PerPageCount);
        }

        public List<Dairy> GetData(int page) {
            var sql = $"select wrote_date, wheather, content from dairy where enable = true order by wrote_date {(Properties.Settings.Default.OrderByAsc ? "asc" : "desc")} limit {(page - 1) * Properties.Settings.Default.PerPageCount}, {Properties.Settings.Default.PerPageCount};";
            cmd.CommandText = sql;
            dt.Clear();
            adpt.Fill(dt);
            if (dt.Rows.Count == 0) return null;
            return (from DataRow row in dt.Rows
                    select new Dairy {
                        WroteDate = row.Field<DateTime>("wrote_date").ToShortDateString(),
                        Wheather = row.Field<string>("wheather"),
                        Content = row.Field<string>("content"),
                    }).ToList();
        }

        public Dairy GetDetail(string date) {
            var sql = $"select '{date}' wrote_date, wheather, thema, content from dairy where enable = true and wrote_date = '{date}';";
            cmd.CommandText = sql;
            dt.Clear();
            adpt.Fill(dt);
            return (from DataRow row in dt.Rows
                    select new Dairy {
                        IsNew = false,
                        WroteDate = row.Field<DateTime>("wrote_date").ToShortDateString(),
                        Wheather = row.Field<string>("wheather"),
                        Thema = row.Field<string>("thema"),
                        Content = row.Field<string>("content"),
                    }).FirstOrDefault();
        }

        public List<Dairy> GetDetail(List<string> date) {
            var sql = $"select wrote_date, wheather, thema, content from dairy where wrote_date in ('{ string.Join("','", date) }');";
            cmd.CommandText = sql;
            dt.Clear();
            adpt.Fill(dt);
            return (from DataRow row in dt.Rows
                    select new Dairy {
                        WroteDate = row.Field<DateTime>("wrote_date").ToShortDateString(),
                        Wheather = row.Field<string>("wheather"),
                        Thema = row.Field<string>("thema"),
                        Content = row.Field<string>("content"),
                    }).ToList();
        }

        public void GetDairyPosition(ref Dairy value) {
            string sqlPrevious, sqlNext;
            if (Properties.Settings.Default.OrderByAsc) {
                sqlPrevious = $"select wrote_date from dairy where wrote_date < '{value.WroteDate}' order by wrote_date desc limit 0, 1;";
                sqlNext = $"select wrote_date from dairy where wrote_date > '{value.WroteDate}' order by wrote_date asc limit 0, 1;";
            } else {
                sqlPrevious = $"select wrote_date from dairy where wrote_date > '{value.WroteDate}' order by wrote_date asc limit 0, 1;";
                sqlNext = $"select wrote_date from dairy where wrote_date < '{value.WroteDate}' order by wrote_date desc limit 0, 1;";
            }
            cmd.CommandText = sqlPrevious;
            var val = cmd.ExecuteScalar();
            value.HasPrevious = new Tuple<bool, string>(val != null, val?.GetType().GetMethod("ToShortDateString").Invoke(val, null).ToString());
            cmd.CommandText = sqlNext;
            val = cmd.ExecuteScalar();
            value.HasNext = new Tuple<bool, string>(val != null, val?.GetType().GetMethod("ToShortDateString").Invoke(val, null).ToString());
        }

        public bool CheckIsExists(string date) {
            var sql = $"select wrote_date from dairy where wrote_date = '{date}';";
            cmd.CommandText = sql;
            var val = cmd.ExecuteScalar();
            if (val != null) return true;
            return false;
        }

        public int ModifyData(Dairy value) {
            var sql = $"update dairy set wheather = '{value.Wheather}', thema = '{value.Thema}', content = '{value.Content}' where wrote_date = '{value.WroteDate}'; "; ;
            cmd.CommandText = sql;
            return RunTransaction();
        }

        public int AddData(Dairy value) {
            var sql = $"insert into dairy(wrote_date, wheather, thema, content, enable) values('{value.WroteDate}', '{value.Wheather}', '{value.Thema}', '{value.Content}', true);";
            cmd.CommandText = sql;
            return RunTransaction();
        }

        public int DeleteData(string date) {
            var sql = $"update dairy set enable = false where wrote_date = '{date}';";
            cmd.CommandText = sql;
            return RunTransaction();
        }

        public int DeleteData(List<string> date) {
            var sql = $"update dairy set enable = false where wrote_date in ('{ string.Join("','", date) }');";
            cmd.CommandText = sql;
            return RunTransaction();
        }

        private int RunTransaction() {
            int result;
            using (var trans = cnn.BeginTransaction()) {
                result = cmd.ExecuteNonQuery();
                try {
                    trans.Commit();
                } catch (Exception) {
                    trans.Rollback();
                    result = 0;
                }
            }
            return result;
        }

        public class Dairy {
            private string _wroteDate;
            public bool IsNew { get; set; }
            public bool IsSelected { get; set; }
            public Tuple<bool, string> HasPrevious { get; set; }
            public Tuple<bool, string> HasNext { get; set; }
            public string WroteDate {
                get => _wroteDate;
                set {
                    if (DateTime.TryParse(value, out var date)) {
                        _wroteDate = date.ToString("yyyy-MM-dd");
                    } else {
                        throw new Exception("Invalid date");
                    }
                }
            }
            public string Wheather { get; set; }
            public string Thema { get; set; }
            public string Content { get; set; }
        }

    }
}
