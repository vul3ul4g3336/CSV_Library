using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Library
{
    public static class PathCorrectly
    {
        public static bool CorrectlyOrNot(string filePath)
        {
            string[] path = filePath.Split('.');
            if (path[path.Length - 1] != "csv")
            {
                throw new Exception("請提供有效的csv檔案，再進行操作");

            }
            if (!File.Exists(filePath))
            {
                throw new Exception("請提供有效的csv檔案路徑位址");

            }
            return true;
        }
        public static void CreateFile(string filePath)
        {
            string[] csvConfirm = filePath.Split('.');
            if (csvConfirm[csvConfirm.Length - 1] != "csv") //確認是否csv
            {
                throw new Exception("請提供有效的csv檔案，再進行操作");

            }

            string[] file = filePath.Split('\\');
            string[] result = file.Take(file.Length - 1).ToArray();
            string directoryPath = string.Join("\\", result);

            if (!Directory.Exists(directoryPath)) //確認是否有此位址
            {
                Directory.CreateDirectory(directoryPath);
            }

        }
    }
}
