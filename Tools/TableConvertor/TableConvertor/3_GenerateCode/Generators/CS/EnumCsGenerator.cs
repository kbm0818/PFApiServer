using System.CodeDom;
using System.Data;
using System.Reflection;

namespace TableConvertor
{
    internal class EnumCsGenerator : ACsGenerator
    {
        public override void CreateCode(CodeNamespace codeNamespace, params Sheet[] sheets)
        {
            var targetClass = new CodeTypeDeclaration();
            targetClass.IsEnum = true;
            targetClass.TypeAttributes = TypeAttributes.Public;

            try
            {
                foreach (var sheet in sheets)
                {
                    foreach (var ls in sheet.Rows)
                    {
                        CodeMemberField mem = new CodeMemberField();
                        mem.Name = ls[0];
                        if (int.TryParse(ls[1], out int value) is true)
                        {
                            mem.InitExpression = new CodePrimitiveExpression(value);
                        }
                        targetClass.Members.Add(mem);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Enum Cs Create Fail, Exception Message:{e.Message}");
            }

            codeNamespace.Types.Add(targetClass);
        }
    }
}
