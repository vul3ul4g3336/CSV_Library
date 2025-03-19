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
    public static class HeaderManager
    {
        public static Dictionary<string, int> dicts = null;
        public static void headerConfirm(string[] headers)
        {

            Dictionary<string, int> headerMap = new Dictionary<string, int>();
            for (int i = 0; i < headers.Length; i++)
            {
                headerMap.Add(headers[i], i);

            }
            dicts = headerMap;
        }
        public static void headerCheck<T>(T data, string filePath)
        {
            PropertyInfo[] dataproperties = data.GetType().GetProperties();
            string student_header = string.Join(",", dataproperties.Select(x => x.Name));
            if (!File.Exists(filePath))
            {
                StreamWriter newWriter = new StreamWriter(filePath, true, Encoding.Default);
                newWriter.WriteLine(student_header);
                newWriter.Flush();
                newWriter.Close();
            }
            StreamReader reader = new StreamReader(filePath, Encoding.Default);
            string header = reader.ReadLine().TrimEnd('\r');
            string fulldata = reader.ReadToEnd().TrimEnd('\r');
            reader.Close();
            if (header != student_header)
            {

                StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Default);
                streamWriter.WriteLine(student_header);
                streamWriter.WriteLine(header);
                streamWriter.WriteLine(fulldata.TrimEnd());
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

    }
}
