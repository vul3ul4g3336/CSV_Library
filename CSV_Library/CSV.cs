﻿using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
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

        //static PropertyInfo[] props = typeof(DataModel).GetProperties();
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
            //if (infos == null || _getters == null)
            //{
            //    infos = typeof(T).GetProperties();
            //    _getters = typeof(T)
            //.GetProperties()
            //.ToDictionary(
            //    prop => prop.Name,
            //    prop => CreateGetter(prop)
            //);

            //}
            //StringBuilder stringBuilder = new StringBuilder(90);

            //using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.Default))
            //{

            //    for (int i = 0; i < datas.Count; i++)
            //    {

            //        for (int j = 0; j < infos.Length; j++)
            //        {
            //            var value = _getters[infos[j].Name](datas[i]);
            //            stringBuilder.Append(value.ToString());

            //            // ✅ 正確使用 j 判斷逗號
            //            if (j < infos.Length - 1)
            //                stringBuilder.Append(',');

            //        }
            //        writer.WriteLine(stringBuilder.ToString());
            //        stringBuilder.Clear();
            //    }

            //}
            #region 不用字典
            char[] buffer = new char[60];
            var properties = typeof(T).GetProperties();
            var getters = properties.Select(CreateGetter).ToArray();
            StringBuilder sb = new StringBuilder(90);
            using (StreamWriter writer = new StreamWriter(filePath, append: true, Encoding.Default))
            {

                foreach (var data in datas)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = getters[i](data);
                        sb.Append(value);
                        if (i < properties.Length - 1)
                            sb.Append(',');
                    }

                    writer.WriteLine(sb.ToString());

                    sb.Clear();
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
                _setters =
                typeof(T)
                .GetProperties()
                .ToDictionary(
                    prop => prop.Name,
                    prop => CreateSetter(prop)
                );
            }
            List<T> list = new List<T>();
            int fieldsCount = typeof(T).GetProperties().Length;
            StreamReader reader = new StreamReader(filePath, Encoding.Default);
            /* HeaderManager.headerConfirm(reader.ReadLine().Split(','));*/ // 讀標頭
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
                string line = reader.ReadLine();
                ReadOnlySpan<char> span = line.AsSpan();
                #region
                //string[] datas = new string[fieldsCount];
                //int start = 0;
                //int field = 0;
                //while (true)
                //{
                //    int commaIndex = span.Slice(start).IndexOf(',');
                //    if (commaIndex == -1)
                //    {
                //        datas[field++] = span.Slice(start).ToString();
                //        break;
                //    }
                //    else
                //    {
                //        datas[field++] = span.Slice(start, commaIndex).ToString();
                //        start += commaIndex + 1;
                //    }
                //}
                #endregion
                int start = 0;
                int field = 0;
                while (true)
                {
                    // 找逗號位置
                    int commaIndex = span.Slice(start).IndexOf(',');

                    if (commaIndex == -1)
                    {
                        // 最後一欄
                        _setterDelegates[field++](t, span.Slice(start).ToString());
                        break;
                    }
                    else
                    {
                        _setterDelegates[field++](t, span.Slice(start, commaIndex).ToString());
                        start += commaIndex + 1;
                    }
                }


                //PropertyInfo[] infos = t.GetType().GetProperties();

                //for (int i = 0; i < infos.Length; i++)
                //{
                //    _setters[infos[i].Name](t, _setterDelegates[i]);
                //    //object value = Convert.ChangeType(datas[i], infos[i].PropertyType);
                //    //infos[i].SetValue(t, value);
                //}

                list.Add(t);
            }
            reader.Close();
            GC.Collect();
            return list;
        }
        //if (!PathCorrectly.CorrectlyOrNot(filePath))
        //{
        //    return null;
        //}


        //StreamReader reader = new StreamReader(filePath, Encoding.Default);
        //List<T> list = new List<T>();
        //HeaderManager.headerConfirm(reader.ReadLine().Split(',')); // 讀標頭
        //while (!reader.EndOfStream)
        //{
        //    T t = new T();
        //    string[] inputs = reader.ReadLine().Split(','); // 4
        //    PropertyInfo[] infos = t.GetType().GetProperties(); //10
        //    for (int i = 0; i < infos.Length; i++) // 2
        //    {
        //        infos[i].SetValue(t, inputs[i]);
        //    }
        //    list.Add(t);
        //}
        //reader.Close();
        //return list;
    }


    //        不進入非使用者程式碼 '記帳.Models.RecordModel..ctor'
    //逐步執行: 不進入屬性 '記帳.Models.RecordModel.set_Date'。 若要逐步執行屬性或運算子，請前往[工具] -> [選項] -> [偵錯]，然後取消選取[不進入屬性和運算子(僅限受控)]。
    //逐步執行: 不進入屬性 '記帳.Models.RecordModel.set_Price'。 若要逐步執行屬性或運算子，請前往[工具] -> [選項] -> [偵錯]，然後取消選取[不進入屬性和運算子(僅限受控)]。
    //逐步執行: 不進入屬性 '記帳.Models.RecordModel.set_Type'。 若要逐步執行屬性或運算子，請前往[工具
}

