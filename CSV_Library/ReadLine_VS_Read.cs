using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Library
{
    [MemoryDiagnoser]
    public class ReadLine_VS_Read
    {
        [Benchmark]
        public void Read()
        {
            StreamReader streamReader = new StreamReader(@"C:\Users\TUF\source\repos\Parallel_Processing\MockData_Read\MOCK_DATA_10000000.csv");

            char[] buffer = new char[100]; // 暫存用的 buffer，長度可以依據需求調整
            int recordCount = 0;
            int charsRead = 0;
            int start = 0;

            while (recordCount < 1000000 && (charsRead = streamReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < charsRead; i++)
                {
                    if (buffer[i] == '\n' || (buffer[i] == '\r' && (i + 1 >= charsRead || buffer[i + 1] != '\n')))
                    {
                        // 抓出一行的資料
                        var lineSpan = new ReadOnlySpan<char>(buffer, start, i - start);
                        //ProcessLine(lineSpan); // 你可以將這行當作你要做的處理


                        recordCount++;
                        if (recordCount >= 1000000)
                            break;

                        start = i + 1;
                    }
                }
                // 若有未處理的部分可以依需求續接下一批讀入的 buffer，但這裡我們只抓前三筆就好
            }
            streamReader.Close();

        }


        [Benchmark]
        public void ReadLine()
        {
            StreamReader streamReader = new StreamReader(@"C:\Users\TUF\source\repos\Parallel_Processing\MockData_Read\MOCK_DATA_10000000.csv");

            for (int i = 0; i < 1000000; i++)
            {
                string line = streamReader.ReadLine();
                ReadOnlySpan<char> datas = line.AsSpan();
            }


            streamReader.Close();
        }
    }
}
