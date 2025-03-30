using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CSV_Library
{
    [MemoryDiagnoser]
    public class Split_VS_Span
    {

        static PropertyInfo[] infos = typeof(DataModel).GetProperties();
        delegate void SetterDelegate(object target, object value);
        static readonly Dictionary<string, SetterDelegate> _setters = typeof(DataModel)
            .GetProperties()
            .ToDictionary(
                prop => prop.Name,
                prop => CreateSetter(prop)
            );


        private static SetterDelegate CreateSetter(PropertyInfo property)
        {
            var targetType = typeof(object);
            var valueType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");
            var valueParam = Expression.Parameter(valueType, "value");

            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var castValue = Expression.Convert(valueParam, property.PropertyType);

            var propertySetter = Expression.Call(castTarget, property.GetSetMethod(), castValue);

            var lambda = Expression.Lambda<SetterDelegate>(propertySetter, targetParam, valueParam);
            return lambda.Compile();
        }



        [Benchmark]
        public void Split()
        {
            string line = "1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114";
            string[] datas = line.Split(',');

            List<DataModel> list = new List<DataModel>();
            DataModel dataModel = new DataModel();

            for (int i = 0; i < infos.Length; i++)
            {
                //object value = Convert.ChangeType(datas[i], infos[i].PropertyType);
                // infos[i].SetValue(dataModel, datas[i]);
                _setters[infos[i].Name](dataModel, datas[i]);
            }

            list.Add(dataModel);
        }

        [Benchmark]
        public void Span()
        {

            string line = "1,Andrey,Wyborn,awyborn0@eepurl.com,Male,230.108.222.114";
            ReadOnlySpan<char> span = line.AsSpan();
            string[] datas = new string[6];
            List<DataModel> list = new List<DataModel>();
            int start = 0;
            int field = 0;
            while (true)
            {
                int commaIndex = span.Slice(start).IndexOf(',');
                if (commaIndex == -1)
                {
                    datas[field++] = span.Slice(start).ToString();
                    break;
                }
                else
                {
                    datas[field++] = span.Slice(start, commaIndex).ToString();
                    start += commaIndex + 1;
                }
            }

            DataModel dataModel = new DataModel();
            for (int i = 0; i < infos.Length; i++)
            {
                //object value = Convert.ChangeType(datas[i], infos[i].PropertyType);
                //infos[i].SetValue(dataModel, datas[i]);
                _setters[infos[i].Name](dataModel, datas[i]);
            }

            list.Add(dataModel);
        }
    }
}