
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Library
{
    internal class Program
    {
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


            var summary = BenchmarkRunner.Run<Origin_VS_StringBuilder>();





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
