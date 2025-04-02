using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CSV_Library
{
    [MemoryDiagnoser]
    public class Span_VS_Span1
    {
        static PropertyInfo[] infos = typeof(DataModel).GetProperties();
        static int PropsCount = infos.Length;

        delegate void SetterDelegate(object target, object value);

        private static readonly SetterDelegate[] _setterDelegates =
    infos.Select(p => CreateSetter(p)).ToArray();
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
        public void ParseAndSetDirectly()
        {
            string input = "572,Christa,cornhill,ccornhillfv@theglobeandmail.com,Female,175.238.233.64";
            ReadOnlySpan<char> datas = input.AsSpan();

            DataModel dataModel = new DataModel();

            int start = 0;
            for (int i = 0; i < PropsCount; i++)
            {
                // 找逗號位置
                int commaIndex = datas.Slice(start).IndexOf(',');

                if (commaIndex == -1)
                {
                    // 最後一欄
                    _setterDelegates[i](dataModel, datas.Slice(start).ToString());
                    break;
                }
                else
                {
                    _setterDelegates[i](dataModel, datas.Slice(start, commaIndex).ToString());
                    start += commaIndex + 1;
                }
            }

            // 這裡就已經是完整填好的 DataModel
            List<DataModel> dataList = new List<DataModel> { dataModel };
        }
        [Benchmark]
        public void Span()
        {
            string input = "572,Christa,cornhill,ccornhillfv@theglobeandmail.com,Female,175.238.233.64";
            ReadOnlySpan<char> span = input.AsSpan();
            string[] datas = new string[PropsCount];
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
            List<DataModel> list = new List<DataModel>();

            DataModel dataModel1 = new DataModel();
            for (int i = 0; i < infos.Length; i++)
            {
                _setterDelegates[i](dataModel1, datas[i]);

                //object value = Convert.ChangeType(datas[i], infos[i].PropertyType);
                //infos[i].SetValue(t, value);
            }

            list.Add(dataModel1);
        }
    }
}