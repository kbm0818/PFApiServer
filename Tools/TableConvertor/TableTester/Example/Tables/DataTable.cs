using System;
using System.Collections;
using System.Collections.Generic;
using TableTester.Example.Tables;

namespace TableData
{
	public interface IDataTableBase{ }

	public class DataTableListBase 
    {
		public Dictionary<object, object> DataList = new Dictionary<object, object>();

		public virtual void Load(BinaryReader reader)
		{
		}

		public TValue? GetDataTable<TKey, TValue>(object key) where TValue : IDataTableBase
        {
			if (DataList.TryGetValue(key, out object? dataTableBase) == false)
			{
				return default;
			}

			if (dataTableBase is not TValue ret)
			{
				return default;
			}

			return ret;
		}
	}

	public class DataTableLoader
	{
		protected Dictionary<TableNames, DataTableListBase> _datatableList = new();
		public delegate void FileErrorHandler(string dataFileName);
		public event FileErrorHandler? ErrorHandler;

		public void Init(string dstPath)
		{
			if (!Load(dstPath, AILevelDataTable_List.DATAFILENAME, AILevelDataTable_List.NAME, new AILevelDataTable_List())) ErrorHandler?.Invoke(AILevelDataTable_List.DATAFILENAME);
			if (!Load(dstPath, AILevelDataTable_List2.DATAFILENAME, AILevelDataTable_List2.NAME, new AILevelDataTable_List2())) ErrorHandler?.Invoke(AILevelDataTable_List2.DATAFILENAME);
		}

		public IEnumerable<KeyValuePair<TKey, TValue>>? GetEnumeratorOrNull<TKey, TValue>(TableNames tableName)
		{
			if (_datatableList.TryGetValue(tableName, out DataTableListBase? dataTableListBase) is false)
				return default;

			//foreach(var e in dataTableListBase.DataList)
			//{
			//	if((KeyValuePair<int, IDataTableBase>)e)
			//	{
			//		int c = 10;
			//	}
			//}

            var temp = dataTableListBase.DataList.Select(p => p);
            if (temp is not IEnumerable<KeyValuePair<TKey, TValue>> ret)
                return default;

            return ret;

            //if (dataTableListBase.DataList is not IEnumerable<KeyValuePair<TKey, TValue>> ret)
            //	return default;

            //return ret;
        }

        public IEnumerable<KeyValuePair<ITRange<T1>, T2>>? GetEnumeratorOrNullByRange<T1, T2>(TableNames tableName, T1 key) where T1 : IComparable
                                                                                                                        where T2 : IDataTableBase
        {
            if (_datatableList.TryGetValue(tableName, out DataTableListBase? dataTableListBase) is false)
                return default;

            if (dataTableListBase.DataList.Where(p => ((ITRange<T1>)p.Key).CompareTo(key) is 0) is not IEnumerable<KeyValuePair<ITRange<T1>, T2>> ret)
                return default;

            return ret;
        }


        public TValue? GetDataTable<TKey, TValue>(TKey key) where TKey : notnull 
														    where TValue : IDataTableBase
		{
			if (Enum.TryParse(typeof(TValue).Name, out TableNames tableName) is false)
				return default;

			if (_datatableList.TryGetValue(tableName, out DataTableListBase? dataTableListBase) is false)
				return default;

			return dataTableListBase!.GetDataTable<TKey, TValue>(key);
		}

        public void AddDataTableList(TableNames tableName, DataTableListBase datatableList)
		{
			if (true == _datatableList.ContainsKey(tableName))
			{
				_datatableList[tableName] = datatableList;
			}
			else
			{
				_datatableList.Add(tableName, datatableList);
			}
		}

		public bool Load(string dstPath, string fileName, TableNames tableName, DataTableListBase datatableList)
		{
			try
			{
				string dataPath = $"{dstPath}/{fileName}";
                if (!File.Exists(dataPath))
				{
					return false;
				}

				using (BinaryReader reader = new(File.Open(dataPath, FileMode.Open)))
				{
					datatableList.Load(reader);
				}

				AddDataTableList(tableName, datatableList);

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}