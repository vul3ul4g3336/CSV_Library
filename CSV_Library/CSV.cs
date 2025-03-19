using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Library
{
    public class CSV
    {

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

            PropertyInfo[] properties = typeof(T).GetProperties();


            for (int i = 0; i < datas.Count; i++)
            {

                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.Default))
                {
                    string message = string.Empty;
                    foreach (PropertyInfo property in properties)
                    {
                        message += property.GetValue(datas[i]) + ",";
                    }
                    message = message.TrimEnd(',');
                    writer.WriteLine(message);
                }

            }
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
        public static List<T> Read<T>(string filePath) where T : class, new()
        {
            //HW:嘗試完成資料對位，將StudentData讀出
            //Tips:可以利用Dictionary儲存Header去做對應


            //"C:\Users\TUF\source\repos\CSV_Library\CSV_Library\bin\Debug\data.csv"
            //資料夾路徑:"C:\Users\TUF\source\repos\CSV_Library\CSV_Library\bin\Debug"
            //檔案完整路徑:"C:\Users\TUF\source\repos\CSV_Library\CSV_Library\bin\Debug\data.csv"
            //檔案名稱與類型:data.csv
            if (!PathCorrectly.CorrectlyOrNot(filePath))
            {
                return null;
            }


            StreamReader reader = new StreamReader(filePath, Encoding.Default);
            List<T> list = new List<T>();
            HeaderManager.headerConfirm(reader.ReadLine().Split(',')); // 讀標頭
            while (!reader.EndOfStream)
            {
                T t = new T();
                string[] inputs = reader.ReadLine().Split(','); // 4
                PropertyInfo[] infos = t.GetType().GetProperties(); //10
                for (int i = 0; i < infos.Length; i++) // 2
                {
                    if (HeaderManager.dicts.ContainsKey(infos[i].Name))
                    {
                        infos[i].SetValue(t, Convert.ChangeType(inputs[HeaderManager.dicts[infos[i].Name]], infos[i].PropertyType));
                    }

                }
                list.Add(t);
            }
            reader.Close();
            return list;
        }


        //        不進入非使用者程式碼 '記帳.Models.RecordModel..ctor'
        //逐步執行: 不進入屬性 '記帳.Models.RecordModel.set_Date'。 若要逐步執行屬性或運算子，請前往[工具] -> [選項] -> [偵錯]，然後取消選取[不進入屬性和運算子(僅限受控)]。
        //逐步執行: 不進入屬性 '記帳.Models.RecordModel.set_Price'。 若要逐步執行屬性或運算子，請前往[工具] -> [選項] -> [偵錯]，然後取消選取[不進入屬性和運算子(僅限受控)]。
        //逐步執行: 不進入屬性 '記帳.Models.RecordModel.set_Type'。 若要逐步執行屬性或運算子，請前往[工具
    }
}
