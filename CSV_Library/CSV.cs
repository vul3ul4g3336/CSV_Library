using FastSerialization;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Library
{
    public class CSV
    {

        static PropertyInfo[] infos = null; //typeof(DataModel).GetProperties();
        delegate void SetterDelegate(object target, object value);
        delegate object GetterDelegate(object target);
        static Dictionary<string, SetterDelegate> _setters = null;
        static Dictionary<string, GetterDelegate> _getters = null;
        private static SetterDelegate[] _setterDelegates = null;



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


        private static GetterDelegate CreateGetter(PropertyInfo property)
        {
            var targetParam = Expression.Parameter(typeof(object), "target");
            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var propertyGetter = Expression.Property(castTarget, property);
            var castResult = Expression.Convert(propertyGetter, typeof(object));

            var lambda = Expression.Lambda<GetterDelegate>(castResult, targetParam);
            return lambda.Compile();
        }
        public static void Write<T>(string filePath, T data) where T : class, new()
        {
            PathCorrectly.CreateFile(filePath);
            Type dataType = data.GetType();
            HeaderManager.headerCheck(data, filePath);
            csvWritter(filePath, data);

        }
        public static void Write<T>(string filePath, List<T> datas) where T : class, new()
        {
            //PathCorrectly.CreateFile(filePath);
            //HeaderManager.headerCheck(datas[0], filePath);
            if (infos == null || _getters == null)
            {
                infos = typeof(T).GetProperties();
                _getters = typeof(T)
            .GetProperties()
            .ToDictionary(
                prop => prop.Name,
                prop => CreateGetter(prop)
            );

            }

            #region 使用Buffer
            char[] buffer = new char[100];
            StringBuilder stringBuilder = new StringBuilder(100);
            using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.Default))
            {

                for (int i = 0; i < datas.Count; i++)
                {

                    for (int j = 0; j < infos.Length; j++)
                    {
                        stringBuilder.Append(_getters[infos[j].Name](datas[i]));//infos[j].GetValue(dataModel)
                        if (j < infos.Length - 1) stringBuilder.Append(',');
                    }
                    stringBuilder.Append('\n');
                    int length = stringBuilder.Length;
                    stringBuilder.CopyTo(0, buffer, 0, length);
                    writer.Write(buffer, 0, length);
                    stringBuilder.Clear();
                }

            }
            #endregion




        }
        private static void csvWritter<T>(string filePath, T data)
        {


            using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.Default))
            {
                string message = string.Empty;
                PropertyInfo[] properties = data.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    message += property.GetValue(data) + ",";
                }
                message = message.TrimEnd(',');
                writer.WriteLine(message);
            }
        }
        public static List<T> Read<T>(string filePath, int startLine, int takeCount) where T : class, new()
        {
            if (!PathCorrectly.CorrectlyOrNot(filePath))
            {
                return null;
            }

            if (infos == null)
            {
                infos = typeof(T).GetProperties();
                //_setters =
                //typeof(T)
                //.GetProperties()
                //.ToDictionary(
                //    prop => prop.Name,
                //    prop => CreateSetter(prop)
                //);
                _setterDelegates =
                infos.Select(p => CreateSetter(p)).ToArray();
            }


            List<T> list = new List<T>();
            StreamReader reader = new StreamReader(filePath, Encoding.Default);
            int count = 0;
            while (!reader.EndOfStream)
            {
                count++;
                if (count < startLine)
                {
                    reader.ReadLine(); // skip
                    continue;
                }
                if (count > startLine + takeCount)
                    break;

                T t = new T();
                ReadOnlySpan<char> datas = reader.ReadLine().AsSpan();
                int start = 0;
                for (int i = 0; i < infos.Length; i++)
                {
                    // 找逗號位置
                    int commaIndex = datas.Slice(start).IndexOf(',');

                    if (commaIndex == -1)
                    {
                        // 最後一欄
                        _setterDelegates[i](t, datas.Slice(start).ToString());
                        break;
                    }
                    else
                    {
                        _setterDelegates[i](t, datas.Slice(start, commaIndex).ToString());
                        start += commaIndex + 1;
                    }
                }


                list.Add(t);
            }
            reader.Close();
            GC.Collect();
            return list;

        }

    }

}

