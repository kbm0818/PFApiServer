namespace TableConvertor
{
    internal class Program
    {
        static int Main(string[] args)
        {
            GenerateOption.Init(args);

            TypeFactory.Init();
            DataReader.Load();

            Generator.Generate(eLanguageType.CS, eDataType.Enum, Const.ENUM_CS_FULL_FILE_NAME, TemporaryCode.EnumSheets.ToArray());
            Generator.Generate(eLanguageType.CS, eDataType.Constant, Const.CONST_CS_FULL_FILE_NAME, TemporaryCode.ConstantSheets.ToArray());
            Generator.Generate(eLanguageType.CS, eDataType.ConstantManager, Const.CONST_MANAGER_CS_FULL_FILE_NAME, TemporaryCode.ConstantSheets.ToArray());

            foreach (var sheet in TemporaryCode.DataSheets)
            {
                Generator.Generate(eLanguageType.CS, eDataType.Data, sheet.DataCsFullFileName, sheet);
            }

            Generator.Generate(eLanguageType.COMMON, eDataType.Bytes, string.Empty, TemporaryCode.DataSheets.ToArray());

            Generator.Generate(eLanguageType.COMMON, eDataType.Version, GenerateOption.FullOutputPath);

            return 0;
        }
    }
}
