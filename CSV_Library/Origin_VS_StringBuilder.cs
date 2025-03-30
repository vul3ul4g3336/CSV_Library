using BenchmarkDotNet.Attributes;
using Iced.Intel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CSV_Library
{
    [MemoryDiagnoser]
    public class Origin_VS_StringBuilder
    {
        static PropertyInfo[] infos = typeof(DataModel).GetProperties();
        delegate object GetterDelegate(object target);

        static readonly Dictionary<string, GetterDelegate> _getters = typeof(DataModel)
            .GetProperties()
            .ToDictionary(
                prop => prop.Name,
                prop => CreateGetter(prop)
            );

        private static GetterDelegate CreateGetter(PropertyInfo property)
        {
            var targetParam = Expression.Parameter(typeof(object), "target");
            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var propertyGetter = Expression.Property(castTarget, property);
            var castResult = Expression.Convert(propertyGetter, typeof(object));

            var lambda = Expression.Lambda<GetterDelegate>(castResult, targetParam);
            return lambda.Compile();
        }

        [Benchmark]
        public void Origin()
        {
            for (int i = 0; i < 2_000_000; i++)
            {
                DataModel dataModel = new DataModel()
                {
                    id = "1",
                    first_name = "Andrey",
                    last_name = "Wyborn",
                    email = "awyborn0@eepurl.com",
                    gender = "Male",
                    ip_address = "230.108.222.114"
                };

                string message = string.Empty;
                for (int j = 0; j < infos.Length; j++)
                {
                    message += infos[j].GetValue(dataModel) + ",";
                }
                message = message.TrimEnd(',');
            }

        }
        [Benchmark]
        public void StringBuilder()
        {
            StringBuilder stringBuilder = new StringBuilder(90);

            for (int i = 0; i < 2_000_000; i++)
            {

                DataModel dataModel = new DataModel()
                {
                    id = "1",
                    first_name = "Andrey",
                    last_name = "Wyborn",
                    email = "awyborn0@eepurl.com",
                    gender = "Male",
                    ip_address = "230.108.222.114"
                };

                for (int j = 0; j < infos.Length; j++)
                {
                    stringBuilder.Append(_getters[infos[j].Name](dataModel));//infos[j].GetValue(dataModel)
                    if (j < infos.Length - 1) stringBuilder.Append(',');
                }
                string message = stringBuilder.ToString();
                stringBuilder.Clear();
            }

        }
    }
}