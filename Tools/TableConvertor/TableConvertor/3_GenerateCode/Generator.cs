namespace TableConvertor
{
    public enum eLanguageType
    {
        COMMON,
        CS,
        MAX
    }

    public enum eDataType
    {
        Data,
        Enum,
        Constant,
        ConstantManager,
        Bytes,
        Version,
        MAX
    }

    internal static partial class Generator
    {
        private static Dictionary<eLanguageType, Dictionary<eDataType, Type>> _generators = new();

        static Generator()
        {
            _generators[eLanguageType.CS] = new();
            _generators[eLanguageType.CS].Add(eDataType.Data, typeof(DataCsGenerator));
            _generators[eLanguageType.CS].Add(eDataType.Enum, typeof(EnumCsGenerator));
            _generators[eLanguageType.CS].Add(eDataType.Constant, typeof(ConstantCsGenerator));
            _generators[eLanguageType.CS].Add(eDataType.ConstantManager, typeof(ConstantManagerCsGenerator));

            _generators[eLanguageType.COMMON] = new();
            _generators[eLanguageType.COMMON].Add(eDataType.Bytes, typeof(BytesGenerator));
            _generators[eLanguageType.COMMON].Add(eDataType.Version, typeof(VersionGenerator));
        }

        public static bool Generate(eLanguageType languageType, eDataType dataType, string fileName, params Sheet[] sheets)
        {
            if (_generators.TryGetValue(languageType, out var generateTypes) is false)
            {
                Console.WriteLine($"Not Support LanguageType : {languageType}");
                return false;
            }

            if (generateTypes.TryGetValue(dataType, out var type) is false)
            {
                Console.WriteLine($"Not Support DataType : {dataType}");
                return false;
            }

            IGenerator? c = Activator.CreateInstance(type) as IGenerator;
            if(c is null) throw new ArgumentNullException("Generate Fail");
            c!.Generate(fileName, sheets);

            return true;
        }
    }
}
