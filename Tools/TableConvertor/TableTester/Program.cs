using TableData;
using TableTester.Example.Tables;

namespace TableTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //DataTableManager.Init(@".\");
            DataTableListBase listBase1 = new();
            listBase1.DataList.Add((int)-1, new AILevelDataTable() { SerialNo = -1 });
            listBase1.DataList.Add((int)0, new AILevelDataTable() { SerialNo = 0 });
            listBase1.DataList.Add((int)1, new AILevelDataTable() { SerialNo = 1 });
            listBase1.DataList.Add((int)2, new AILevelDataTable() { SerialNo = 2 });

            DataTableManager._dataTableLoader.AddDataTableList(AILevelDataTable_List.NAME, listBase1);

            var temp = DataTableManager.GetDataTable<int, AILevelDataTable>(1);
            //var temp2 = DataTableManager.GetEnumeratorOrNull<int, AILevelDataTable>();
            var result = DataTableManager._dataTableLoader.GetEnumeratorOrNullByRange<int, AILevelDataTable>(TableNames.AILevelDataTable, 1);

            //(int, DateTime) test = (1, DateTime.UtcNow);
            //var tempType = test.GetType();


            //var b1 = test.GetType().GetGenericArguments()[0].IsAssignableTo(typeof(IComparable));
            //var b2 = test.GetType().GetGenericArguments()[1].IsAssignableTo(typeof(IComparable));
            ////var b2 = test.GetType().GetGenericParameterConstraints()[1];//.IsSubclassOf(typeof(System.Runtime.CompilerServices.ITuple));


            int a = 10;
        }
    }
}