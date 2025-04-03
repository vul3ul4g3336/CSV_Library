
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Library
{
    internal class Program
    {
        static char[] buffer = new char[60];
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

        static void Main(string[] args)
        {
            #region
            //Student student = new Student();
            //student.Name = "Leo";
            //student.Id = "1";
            //student.Description = "I am a Student";
            //student.Score = "100";
            //Student student1 = new Student();
            //student1.Name = "A";
            //student1.Id = "12";
            //student1.Description = "測試測試測試測試測試測試測試測試測試測試測試測試";
            //student1.Score = "20";
            //string address = "C:\\Users\\TUF\\source\\repos\\CSV_Library\\CSV_Library\\bin\\Debug\\data.csv";
            //List<Student> students = new List<Student>() {
            //student,student1
            //};
            //CSV.Write(address, students);
            //CSV.Write(address, student);
            //CSV.Write(address, student1);
            //CSV.Read<Student>("C:\\Users\\TUF\\source\\repos\\CSV_Library\\CSV_Library\\bin\\Debug\\data.csv");
            //List<StudentData> studentsData = CSV.Read<StudentData>("C:\\Users\\TUF\\source\\repos\\CSV_Library\\CSV_Library\\bin\\Debug\\data.csv");

            //foreach (var data in studentsData)
            //{
            //    Console.WriteLine($"姓名: {data.Name}, 描述: {data.Description} , {data.Score}");
            //}
            //考慮資料夾路徑存在與否 檔案路徑存在與否 副檔名是否為CSV
            //Directory.Exists
            //File.Exists
            //List<Student> students = CSV.Read<Student>(@"C:\Users\TUF\source\repos\CSV_Library\CSV_Library\bin\Debug\data.csv");
            //foreach (Student s in students)
            //{
            //    Console.WriteLine($"{s.Name}  {s.Id} {s.Description} {s.Score}");
            //}
            #endregion


            var summary = BenchmarkRunner.Run<ReadLine_VS_Read>();


            //StreamWriter streamWriter = new StreamWriter();
            //streamWriter.WriteLine(,);


            //DataModel dataModel = new DataModel()
            //{
            //    id = "1",
            //    first_name = "Andrey",
            //    last_name = "Wyborn",
            //    email = "awyborn0@eepurl.com",
            //    gender = "Male",
            //    ip_address = "230.108.222.114"
            //};

            //DataModel dataModel2 = new DataModel()
            //{
            //    id = "2",
            //    first_name = "Leo",
            //    last_name = "AAA",
            //    email = "cccc@eepurl.com",
            //    gender = "Female",
            //    ip_address = "192.168.111.114"
            //};


            //List<DataModel> list = new List<DataModel>() { dataModel, dataModel2 };

            //StringBuilder stringBuilder = new StringBuilder(60);
            //StreamWriter streamWriter = new StreamWriter("data.csv", true, Encoding.UTF8);

            //foreach (var item in list)
            //{

            //    for (int j = 0; j < infos.Length; j++)
            //    {
            //        stringBuilder.Append(_getters[infos[j].Name](dataModel));//infos[j].GetValue(dataModel)
            //        if (j < infos.Length - 1) stringBuilder.Append(',');
            //    }
            //    stringBuilder.Append('\n');
            //    int length = stringBuilder.Length;
            //    stringBuilder.CopyTo(0, buffer, 0, length);

            //    streamWriter.Write(buffer, 0, length);
            //    stringBuilder.Clear();
            //}

            //streamWriter.Flush();
            //streamWriter.Close();


            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("HELLO");
            //stringBuilder.Append(",");
            //stringBuilder.Append("WROLD");
            //stringBuilder.Append(",");
            //stringBuilder.Append("!!!!!\r\n");
            //stringBuilder.Append("Hi");
            //stringBuilder.Append(",");
            //stringBuilder.Append("Leo");
            //stringBuilder.Append(",");
            //stringBuilder.Append("!!!!!\r\n");





            //StreamReader streamReader = new StreamReader(@"C:\Users\TUF\source\repos\Parallel_Processing\MockData_Read\MOCK_DATA_10000000.csv");

            //char[] buffer = new char[4096]; // 暫存用的 buffer，長度可以依據需求調整
            //int recordCount = 0;
            //int charsRead = 0;
            //int start = 0;

            //while (recordCount < 3 && (charsRead = streamReader.Read(buffer, 0, buffer.Length)) > 0)
            //{
            //    for (int i = 0; i < charsRead; i++)
            //    {
            //        if (buffer[i] == '\n' || (buffer[i] == '\r' && (i + 1 >= charsRead || buffer[i + 1] != '\n')))
            //        {
            //            // 抓出一行的資料
            //            //var lineSpan = new ReadOnlySpan<char>(buffer, start, i - start);
            //            //ProcessLine(lineSpan); // 你可以將這行當作你要做的處理
            //            string line = new string(buffer, start, i - start);
            //            Console.WriteLine(line.ToString());

            //            recordCount++;
            //            if (recordCount >= 3)
            //                break;

            //            start = i + 1;
            //        }
            //    }

            //    // 若有未處理的部分可以依需求續接下一批讀入的 buffer，但這裡我們只抓前三筆就好
            //}




            Console.ReadKey();
            //串流
            //迅雷影音 => 邊播邊下載 => 逐一下載每一個Byte
            //影片本身就是由連續的圖片所構成的(連續幀Frame) => 代表性GIF
            //200MB => 200000000 

            //比特慧星 => P2P(點對點) 下載方式，下載BT種子資源黨 （多點同時傳輸，平行運算）
            //P2P 點對點傳輸 => FOXY,PPS(現在愛奇藝)
            //新版WebRTC（維持P2P通訊原理）=> 視訊電話

            //影音串流(跳著播放)
        }
    }
}
