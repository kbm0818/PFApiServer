using System.CodeDom;
using System.Data;

namespace TableConvertor
{
    internal class ConstantManagerCsGenerator : ACsGenerator
    {
        public override void CreateCode(CodeNamespace codeNamespace, params Sheet[] sheets)
        {
            var targetClass = new CodeTypeDeclaration("className");

            //foreach (DataRow row in table.Rows)
            //{
            //    CodeMemberField mem = new CodeMemberField();
            //    mem.Name = row[table.Columns[0]].ToString();
            //    targetClass.Members.Add(mem);
            //}

            codeNamespace.Types.Add(targetClass);
        }
    }
}
