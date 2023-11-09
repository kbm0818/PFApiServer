using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableConvertor
{
    internal static class Const
    {
        public static readonly string ENUM_PREFIX = "ENUM_";
        public static readonly string CONST_PREFIX = "CONST_";

        public static readonly string EXECEL_EXTENSION = "xlsx";

        public static readonly string FILE_CRYPTO_KEY = "79628bb8-afed-4133-8c86-9dbf458efc4c";

        public static readonly string ENUM_CS_FILE_NAME = "Enum.cs";
        public static readonly string CONST_CS_FILE_NAME = "Const.cs";
        public static readonly string CONST_MANAGER_CS_FILE_NAME = "ConstManager.cs";
        public static readonly string BYTES_FILE_EXTENSION = ".bytes";
        public static readonly string DATA_CS_FILE_EXTENSION = "Data.cs";
        public static readonly string VERSION_FILE_NAME = "Version.bytes";

        public static readonly string ENUM_CS_FULL_FILE_NAME;
        public static readonly string CONST_CS_FULL_FILE_NAME;
        public static readonly string CONST_MANAGER_CS_FULL_FILE_NAME;
        public static readonly string BYTES_FULL_FILE_NAME;
        public static readonly string DATA_CS_FULL_FILE_NAME;
        public static readonly string VERSION_FULL_FILE_NAME;


        static Const()
        {
            ENUM_CS_FULL_FILE_NAME = $"{GenerateOption.FullOutputPath}{ENUM_CS_FILE_NAME}";
            CONST_CS_FULL_FILE_NAME = $"{GenerateOption.FullOutputPath}{CONST_CS_FILE_NAME}";
            CONST_MANAGER_CS_FULL_FILE_NAME = $"{GenerateOption.FullOutputPath}{CONST_MANAGER_CS_FILE_NAME}";
            BYTES_FULL_FILE_NAME = $"{GenerateOption.FullOutputPath}{{0}}{BYTES_FILE_EXTENSION}";
            DATA_CS_FULL_FILE_NAME = $"{GenerateOption.FullOutputPath}{{0}}{DATA_CS_FILE_EXTENSION}";
            VERSION_FULL_FILE_NAME = $"{GenerateOption.FullOutputPath}{VERSION_FILE_NAME}";
        }
    }

    internal class GenerateOption
    {
        public static string NameSpace { get; set; } = "Table";
        public static string FullOutputPath { get; protected set; } = string.Empty;
        public static string FullSourcePath { get; protected set; } = string.Empty;
        
        public static void Init(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException($"args is empty");

            Parse(args);

            if(FullOutputPath == string.Empty)
                throw new ArgumentException($"Need Directory Path, use /o or /out");

            if (FullSourcePath == string.Empty)
                throw new ArgumentException($"Need Source Path, use /s or /src");
        }

        protected static void Parse(string[] args)
        {
            Console.WriteLine($"Parsing args...");

            var options = new Mono.Options.OptionSet();

            options.Add("o|out=", "Output Path", s => FullOutputPath = Path.GetFullPath(s));
            options.Add("s|src=", "Source File Path", s => FullSourcePath = Path.GetFullPath(s));

            options.Parse(args);
        }
    }
}
