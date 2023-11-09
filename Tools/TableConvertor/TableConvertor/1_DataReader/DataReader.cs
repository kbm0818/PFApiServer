using System.Data;

namespace TableConvertor
{
    internal static class DataReader
    {
        public static void Load()
        {
            var files = Directory.GetFiles(GenerateOption.FullSourcePath);

            List<System.Data.DataTable> allDatas = new();

            #region 엑셀파일 리드
            Console.WriteLine("Excel File Read Begin");
            var excelExtensions = Const.EXECEL_EXTENSION.Split(",");
            var excelFiles = files.Where(s => excelExtensions.Contains(Path.GetExtension(s).Replace(".","")) == true && s.Contains("~$") == false);
            IReader excelReader = new ExcelReader();
            foreach (var excelFile in excelFiles)
            {
                excelReader.Read(excelFile, out List<System.Data.DataTable>? table);
                if (table is null) throw new InvalidOperationException($"{excelFile} Read Fail!");
                Console.WriteLine($"Excel File Read, {Path.GetFileNameWithoutExtension(excelFile)}");
                allDatas.AddRange(table);
            }
            Console.WriteLine("Excel File Read End");
            #endregion

            foreach (var data in allDatas)
            {
                if (data.TableName.Contains(Const.ENUM_PREFIX))
                {
                    var sheet = Sheet.ConvertEnum(data);
                    if (TemporaryCode.EnumSheets.Exists(s=>s.Name == sheet.Name) is true) new ArgumentException($"Duplicate Sheet Name : {sheet.Name}");
                    TemporaryCode.EnumSheets.Add(sheet);
                    continue;
                }
                else if (data.TableName.Contains(Const.CONST_PREFIX))
                {
                    var sheet = Sheet.ConvertEnum(data);
                    if (data.TableName.Contains(Const.CONST_PREFIX))
                    {
                        if (TemporaryCode.ConstantSheets.Count > 1) new NullReferenceException("ConstantTable Count > 1");
                        continue;
                    }
                }
                else
                {
                    var sheet = Sheet.ConvertData(data);
                    if (TemporaryCode.DataSheets.Exists(s => s.Name == sheet.Name) is true) new ArgumentException($"Duplicate Sheet Name : {sheet.Name}");
                    TemporaryCode.DataSheets.Add(sheet);
                }
            }
        }
    }
}
