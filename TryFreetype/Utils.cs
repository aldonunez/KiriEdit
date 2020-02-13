using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TryFreetype
{
    public static class Utils
    {
        public static void PrintProperties(object obj, string indent)
        {
            var stack = new Stack<object>();
            stack.Push(obj);
            PrintProperties(obj, indent, stack);
        }

        private static void PrintProperties(object obj, string indent, Stack<object> stack)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                object value;

                try
                {
                    value = prop.GetValue(obj);
                }
                catch
                {
                    continue;
                }

                Console.Write("{0}{1} = ", indent, prop.Name);

                if (prop.Name == "Glyph")
                {
                    Console.WriteLine();
                    continue;
                }

                foreach (object ancestor in stack)
                {
                    if (object.ReferenceEquals(value, ancestor))
                    {
                        Console.WriteLine();
                        goto NextProp;
                    }
                }

                bool useToString = true;

                if (value != null)
                {
                    MethodInfo method = value.GetType().GetMethod("ToString", new Type[0]);

                    if (method.DeclaringType == typeof(object))
                    {
                        useToString = false;
                    }
                    else if (method.DeclaringType == typeof(ValueType))
                    {
                        var fields = value.GetType().GetFields();
                        if (fields.Length == 0)
                            useToString = false;
                    }
                }

                if (useToString)
                {
                    Console.WriteLine("{0}", value);
                }
                else
                {
                    Console.WriteLine();
                    stack.Push(value);
                    PrintProperties(value, indent + "  ", stack);
                    stack.Pop();
                }

            NextProp:
                ;
            }
        }
    }
}
