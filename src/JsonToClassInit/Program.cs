using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace JsonToClassInit
{
    public class Program
    {
        static Dictionary<Type, Func<object, PropertyInfo, bool, string>> _handler = new Dictionary<Type, Func<object, PropertyInfo, bool, string>>
                                                                                   {
                                                                                       {
                                                                                           typeof(string), Program.GeneratorInitStringForString
                                                                                       },
                                                                                       {
                                                                                           typeof(int), Program.GeneratorInitStringForInt
                                                                                       },
                                                                                       {
                                                                                           typeof(decimal), Program.GeneratorInitStringForDecimal
                                                                                       },
                                                                                       {
                                                                                           typeof(bool), Program.GeneratorInitStringForBool
                                                                                       }
                                                                                   };

        static void Main(string[] args)
        {
            var jsonStr = "{\"Name\":\"JamisLiao\",\"Age\":36,\"Weight\":101.23,\"IsAdult\":true,\"NickNames\":[\"123\",\"456\"],\"Detail\":[{\"Address\":\"TestAddress\",\"Email\":\"jamisliao@gmail.com\"},{\"Address\":\"Test2Address\",\"Email\":\"jamisliao2@gmail.com\"}]}";
            var sample = JsonConvert.DeserializeObject<Sample>(jsonStr);

            var result = GetClassInitString(sample);
            Console.WriteLine(result);
            Console.ReadLine();
        }

        public static string GetClassInitString(object obj)
        {
            var strBuilder = new StringBuilder();
            var type = obj.GetType();
            var typeName = type.Name;
            var propertys = type.GetProperties();
            var classTypes = propertys.Where(p => p.PropertyType.IsClass && p.PropertyType != typeof(string)).ToList();

            strBuilder.AppendLine($"new {typeName}");
            strBuilder.AppendLine("{");
            foreach (var property in propertys)
            {

                if (Program._handler.ContainsKey(property.PropertyType))
                {
                    strBuilder.AppendLine(Program._handler[property.PropertyType].Invoke(obj, property, false));
                }
                else if(property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    strBuilder.AppendLine($"{property.Name} = {GetClassInitString(property.GetValue(obj, null))}");
                }
                else
                {
                    strBuilder.AppendLine(GeneratorInitStringForString(obj, property, false));
                }
            }

            strBuilder.AppendLine("}");

            return strBuilder.ToString();
        }

        public static string GeneratorInitStringForString(object obj, PropertyInfo propertyInfo, bool isCollection)
        {
            if (isCollection)
            {
                var values = propertyInfo.GetValue(obj, null) as IEnumerable<string>;
                var tmp = string.Join(", ", values.Select(p => { return $"\"{p}\""; }));
                var collectionResult = $"new List<{propertyInfo.PropertyType.GetGenericArguments().First().ToString().Replace("System.", "").ToLower()}>{{ {tmp} }},";
                return collectionResult;
            }

            var result = $"{propertyInfo.Name} = \"{propertyInfo.GetValue(obj, null)}\",";
            return result;
        }

        public static string GeneratorInitStringForInt(object obj, PropertyInfo propertyInfo, bool isCollection)
        {
            var result = $"{propertyInfo.Name} = {propertyInfo.GetValue(obj, null)},";
            return result;
        }

        public static string GeneratorInitStringForDecimal(object obj, PropertyInfo propertyInfo, bool isCollection)
        {
            var result = $"{propertyInfo.Name} = {propertyInfo.GetValue(obj, null)}m,";

            return result;
        }

        public static string GeneratorInitStringForBool(object obj, PropertyInfo propertyInfo, bool isCollection)
        {
            var result = $"{propertyInfo.Name} = {propertyInfo.GetValue(obj, null).ToString().ToLower()},";

            return result;
        }
    }
}