using System.CodeDom;
using System.Data.Common;
using System.Xml.Linq;

namespace TableConvertor
{
    internal class DataCsGenerator : ACsGenerator
    {
        public override void CreateCode(CodeNamespace codeNamespace, params Sheet[] sheets)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.IO"));
            
            foreach(var sheet in sheets)
            {
                var listClassName = $"{sheet.Name}DataTable_List";
                var dataClassName = $"{sheet.Name}DataTable";

                var listClass = new CodeTypeDeclaration(listClassName);
                listClass.BaseTypes.Add("DataTableListBase");

                CodeMemberField nameField = new CodeMemberField();
                nameField.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                nameField.Name = "NAME";
                nameField.Type = new CodeTypeReference("TableNames");
                nameField.InitExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("TableNames"), sheet.Name);
                listClass.Members.Add(nameField);

                CodeMemberField fileNameField = new CodeMemberField();
                fileNameField.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                fileNameField.Name = "DATAFILENAME";
                fileNameField.Type = new CodeTypeReference(typeof(string));
                fileNameField.InitExpression = new CodePrimitiveExpression($"{sheet.Name}.bytes");
                listClass.Members.Add(fileNameField);

                CodeMemberMethod loadMethod = new CodeMemberMethod();
                loadMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                loadMethod.Name = "Load";
                loadMethod.ReturnType = new CodeTypeReference(typeof(void));
                loadMethod.Parameters.Add(new CodeParameterDeclarationExpression("BinaryReader", "reader"));

                CodeMethodInvokeExpression readDataCountInvoke =
                    new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("reader"), "ReadInt32");
                CodeVariableDeclarationStatement dataCountDeclaration = new CodeVariableDeclarationStatement(typeof(int), "dateCount", readDataCountInvoke);
                loadMethod.Statements.Add(dataCountDeclaration);

                {
                    CodeVariableDeclarationStatement iDeclaration = new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)), "i", new CodePrimitiveExpression(0));
                    CodeVariableReferenceExpression iReference = new CodeVariableReferenceExpression(iDeclaration.Name);
                    CodeIterationStatement loopStatement = new CodeIterationStatement();
                    loopStatement.InitStatement = iDeclaration;
                    CodeAssignStatement incrementStatement = new CodeAssignStatement();
                    incrementStatement.Left = iReference;
                    incrementStatement.Right = new CodeBinaryOperatorExpression(iReference, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1));
                    loopStatement.IncrementStatement = incrementStatement;
                    loopStatement.TestExpression = new CodeSnippetExpression("i < dataCount");


                    CodeVariableDeclarationStatement dataDeclaration = new CodeVariableDeclarationStatement(dataClassName, "data", new CodeObjectCreateExpression(dataClassName, new CodeExpression[] { }));
                    loopStatement.Statements.Add(dataDeclaration);

                    CodeExpression readerEx = new CodeVariableReferenceExpression("reader");
                    CodeMethodInvokeExpression loadInvoke =
                        new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression("data"), "Load", new CodeExpression[] { readerEx });
                    loopStatement.Statements.Add(loadInvoke);

                    CodeExpression serialNoEx = new CodeVariableReferenceExpression("serialNo");
                    CodeExpression dataEx = new CodeVariableReferenceExpression("data");
                    CodeMethodInvokeExpression addInvoke =
                      new CodeMethodInvokeExpression(
                      new CodeVariableReferenceExpression("DataList"), "Add", new CodeExpression[] { serialNoEx, dataEx });
                    loopStatement.Statements.Add(addInvoke);

                    loadMethod.Statements.Add(loopStatement);
                }

                listClass.Members.Add(loadMethod);

                codeNamespace.Types.Add(listClass);


                var dataClass = new CodeTypeDeclaration(dataClassName);
				var dataClassAttribute = new CodeAttributeDeclaration("System.Serializable");
				dataClass.CustomAttributes.Add(dataClassAttribute);
                dataClass.BaseTypes.Add("IDataTableBase");

                CodeMemberMethod loadDataMethod = new CodeMemberMethod();
                loadDataMethod.Attributes = MemberAttributes.Public;
                loadDataMethod.Name = "Load";
                loadDataMethod.ReturnType = new CodeTypeReference(typeof(void));
                loadDataMethod.Parameters.Add(new CodeParameterDeclarationExpression("BinaryReader", "reader"));

                foreach (var column in sheet.Columns)
                {
                    CodeMemberField columnField = new CodeMemberField();
                    columnField.Attributes = MemberAttributes.Public;
                    columnField.Name = column.Name;

                    if (column.Type!.Type is not null)
                        columnField.Type = new CodeTypeReference(column.Type!.Type);
                    else
                        columnField.Type = new CodeTypeReference(column.Type!.Name);

                    dataClass.Members.Add(columnField);


                    // 각 컬럼마다 리드코드
                }

                dataClass.Members.Add(loadDataMethod);
                codeNamespace.Types.Add(dataClass);
            }
        }
    }
}
