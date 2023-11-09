namespace TableData
{
    public static class DataTableManager
    {
        public static DataTableLoader _dataTableLoader = new();

        static DataTableManager()
        {
            _dataTableLoader.ErrorHandler += ErrorCallHandler;
        }

        public static void Init(string path)
        {
            _dataTableLoader.Init(path);
        }

        public static void ErrorCallHandler(string fileName)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"{fileName} Load Error!!");
            Console.ResetColor();
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>>? GetEnumeratorOrNull<TKey, TValue>(TableNames tableName)
        {
            var test= _dataTableLoader.GetEnumeratorOrNull<TKey, TValue>(tableName);
            return default;
            //return _dataTableLoader.GetEnumeratorOrNull(tableName);
        }

        public static TValue? GetDataTable<TKey, TValue>(TKey key) where TKey : notnull
                                                                   where TValue : IDataTableBase
        {
            return _dataTableLoader.GetDataTable<TKey, TValue>(key);
        }

        public static void Load(string dstPath, string fileName, TableNames tableName, DataTableListBase datatableList)
        {
            _dataTableLoader.Load(dstPath, fileName, tableName, datatableList);
        }

        public static void AddDataTableList(TableNames tableName, DataTableListBase datatableList)
        {
            _dataTableLoader.AddDataTableList(tableName, datatableList);
        }
    }
}
