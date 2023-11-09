using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;

namespace TableConvertor
{
    internal abstract class ACsGenerator : IGenerator
    {
        private CodeCompileUnit targetUnit = new CodeCompileUnit();

        public void Generate(string fileName, params Sheet[] sheets)
        {
            if(sheets == null) throw new ArgumentNullException($"Generate Fail, sheets is null, fileName:{fileName}");

            CodeNamespace nameSpace = new CodeNamespace(GenerateOption.NameSpace);
            targetUnit.Namespaces.Add(nameSpace);

            CreateCode(nameSpace, sheets);
            GenerateCode(fileName);
        }

        private void GenerateCode(string fileName)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = false,
                VerbatimOrder = true,
                BracingStyle = "C",
                IndentString = "\t"
            };

            using (StreamWriter sourceWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }
        }

        public abstract void CreateCode(CodeNamespace codeNamespace, params Sheet[] sheets);
    }
}
