using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TableConvertor
{
    internal static class TypeFactory
    {
        static Dictionary<string, IType> _mapCreator = new Dictionary<string, IType>();

        public static void Init()
        {
            List<Type> valueTypes = new();
            foreach (var myType in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
             .Where(myType => myType.GetInterfaces().Contains(typeof(IType))
             && myType.GetCustomAttributes(typeof(ValueAttribute), false).Length > 0))
            {
                valueTypes.Add(myType);
            }

            List<Type> listTypes = new();
            foreach (var type in valueTypes)
            {
                listTypes.Add(typeof(TList<>).MakeGenericType(type));
            }

            List<Type> mapTypes = new();
            foreach (var type1 in valueTypes)
            {
                foreach (var type2 in valueTypes)
                {
                    mapTypes.Add(typeof(TMap<,>).MakeGenericType(type1, type2));
                }

                foreach (var type2 in listTypes)
                {
                    mapTypes.Add(typeof(TMap<,>).MakeGenericType(type1, type2));
                }
            }

            List<Type> totalTypes = new();
            totalTypes.AddRange(valueTypes);
            totalTypes.AddRange(listTypes);
            totalTypes.AddRange(mapTypes);

            //foreach(var type in totalTypes)
            //{
            //    Console.WriteLine($"{type.GetTypeInfo().ToString().Replace(type.Namespace!,"")}");
            //}

            MethodInfo method = typeof(TypeFactory).GetMethod("Register",
                    BindingFlags.NonPublic | BindingFlags.Static)!;
            foreach (var type in totalTypes)
            {
                method.MakeGenericMethod(type).Invoke(null, null);
            }
        }

        private static void Register<_T>() where _T : IType, new()
        {
            IType instance = new _T();
            _mapCreator.Add(instance.Name, instance);
        }

        public static IType? Get(string typeName)
        {
            IType? type;
            if (_mapCreator.TryGetValue(typeName, out type))
            {
                return type;

            }

            string tempName = typeName + "_t";
            if (_mapCreator.TryGetValue(tempName, out type))
            {
                return type;
            }

            char[] delimiterChars = { '<', '>' };

            string[] tempArray = typeName.Split(delimiterChars);
            if (tempArray.Length < 3)
            {
                return null;
            }

            tempName = tempArray[0] + "<" + tempArray[1] + "_t>" + tempArray[2];
            if (_mapCreator.TryGetValue(tempName, out type))
            {
                return type;
            }

            return null;
        }
    }
}
