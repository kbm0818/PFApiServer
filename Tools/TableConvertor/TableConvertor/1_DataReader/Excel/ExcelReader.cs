using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace TableConvertor
{
    internal class ExcelReader : IReader
    {
        [SupportedOSPlatform("windows")]
        public void Read(string path, out List<DataTable> table)
        {
            table = new List<DataTable>();

            if (System.IO.File.Exists(path))
            {
                System.IO.FileInfo fi = new FileInfo(path);
                using (OleDbConnection conn = new OleDbConnection($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fi.Name};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";"))
                {
                    try
                    {
                        conn.Open();

                        if (conn.State != ConnectionState.Open)
                            return;

                        var worksheets = conn.GetSchema("Tables");

                        foreach (DataRow dr in worksheets.Rows)
                        {
                            string Query = $" SELECT A.* FROM [{dr["TABLE_NAME"]}] AS A ";

                            OleDbCommand cmd = new OleDbCommand(Query, conn);
                            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                            
                            DataTable dt = new DataTable();
                            da.FillSchema(dt, SchemaType.Source);
                            da.Fill(dt);

                            string tableName = dt.TableName.ToLower();
                            if (tableName.Contains("$") == false)
                                continue;

                            if (dt == null || dt.Rows.Count == 0)
                                continue;

                            dt.TableName = dt.TableName.Trim('$');
                            table.Add(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        Console.WriteLine($"OleDb Exception : {ex.Message} FIleName = {fi.Name}");
                    }

                    conn.Close();
                }
            }
        }
    }
}
