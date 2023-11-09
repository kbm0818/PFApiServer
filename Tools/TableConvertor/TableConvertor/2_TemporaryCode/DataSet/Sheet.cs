using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace TableConvertor
{
    internal class Sheet
    {
        public string Name { get; private set; }

        public string DataCsFullFileName => string.Format(Const.DATA_CS_FULL_FILE_NAME, Name);
        public string BinFullFileName => string.Format(Const.BYTES_FULL_FILE_NAME, Name);
        public string DataCsFileName => $"{Name}{Const.DATA_CS_FILE_EXTENSION}";

        public List<Column> Columns { get; set; } = new List<Column>();
        public List<List<string>> Rows { get; set; } = new List<List<string>>();

        public Sheet(string name)
        {
            Name = name;
        }

        public void ByteWrite(BinaryWriter sw)
        {
            // Data Count
            sw.Write(Rows.Count);

            Dictionary<int, int> serialNoDic = new Dictionary<int, int>();

            bool serialNoFound = false;

            for (int r = 0; r < Rows.Count; ++r)
            {
                for (int c = 0; c < Columns.Count; ++c)
                {
                    IType type = Columns[c].Type!;
                    type.Write(sw, Rows[r][c]);
                }
            }

            // not found serial no
            if (serialNoFound == false)
            {
                new ArgumentException($"Not Found SerialNo Table Name = {Name}");
                return;
            }
        }

        public static Sheet ConvertData(System.Data.DataTable table)
        {
            Sheet sheet = new(table.TableName);

            try
            {
                DataRow dr = table.Rows[0]; //row of column type
                int nItemCount = dr.ItemArray.Length;

                for (int colIndex = 0; colIndex < table.Columns.Count; ++colIndex)
                {
                    DataColumn dc = table.Columns[colIndex];
                    object itemOfType = dr.ItemArray[colIndex]!;
                    sheet.Columns.Add(new Column(dc.ColumnName, itemOfType.ToString()!.Replace(" ", "")));
                }

                int nRowCount = table.Rows.Count;
                for (int i = 1; i < nRowCount; ++i)
                {
                    List<string> values = new List<string>();

                    DataRow drOfValue = table.Rows[i];
                    for (int n = 0; n < nItemCount; ++n)
                        values.Add(drOfValue.ItemArray[n]!.ToString()!);

                    sheet.Rows.Add(values);
                }
            }
            catch (Exception e)
            {
                new ArgumentException(e.Message);
            }

            return sheet;
        }

        public static Sheet ConvertEnum(System.Data.DataTable table)
        {
            Sheet sheet = new(table.TableName);

            try
            {
                int nRowCount = table.Rows.Count;
                for (int i = 0; i < nRowCount; ++i)
                {
                    List<string> values = new List<string>();

                    DataRow drOfValue = table.Rows[i];
                    for (int n = 0; n < 2; ++n)
                        values.Add(drOfValue.ItemArray[n]!.ToString()!);

                    sheet.Rows.Add(values);
                }
            }
            catch (Exception e)
            {
                new ArgumentException(e.Message);
            }

            return sheet;
        }
    }
}
