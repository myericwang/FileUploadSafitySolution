using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileEncrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            /** DEMO用測試資料 **/

            // 上傳檔案的基本資訊
            string workspace = @"C:\Users\user\source\repos\FileEncrypt";
            string fileName = "google";
            string fileType = "png";

            // 模擬要進出DB的資料
            Dictionary<string, DB_FileInfo> FileInfos = new Dictionary<string, DB_FileInfo>();
            FileInfos.Add(fileName, new DB_FileInfo()
            {
                // 將檔名換成GUID後存入fileServer與DB，下載時再用Guid換回檔名。
                SID = "9bfa08c3-dda8-4ebe-958b-58e9c475b665",
                FileName = fileName,
                FileType = fileType,
                FilePath = $"{workspace}/EncryptFile",
                ModifyTime = DateTime.Now
            });


            /** 上傳並轉換檔案編碼 **/

            // 將檔案轉為btye，依Guid當檔名並以txt儲存
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] bytes = wc.DownloadData($"{workspace}/SourceFile/{fileName}.{fileType}");
            var dataBytes = JsonConvert.SerializeObject(bytes);

            StreamWriter sw = new StreamWriter($"{FileInfos[fileName].FilePath}/{FileInfos[fileName].SID}.txt");
            sw.WriteLine(dataBytes);
            sw.Close();


            /** 還原回檔案 **/

            // 用檔名對應到Guid          
            StreamReader sr = new StreamReader($"{FileInfos[fileName].FilePath}/{FileInfos[fileName].SID}.txt");

            // 將txt轉為byte陣列
            String line;
            StringBuilder data = new StringBuilder();
            line = sr.ReadLine();
            while (line != null)
            {
                data.Append(line);
                line = sr.ReadLine();
            }
            sr.Close();

            // 將byte轉回檔案並放置目標空間(如是Web可提供使用者直接下載)
            byte[] ansBytes = JsonConvert.DeserializeObject<byte[]>(data.ToString());
            File.WriteAllBytes($"{workspace}/AnsFile/{FileInfos[fileName].FileName}.{FileInfos[fileName].FileType}", ansBytes);
        }
    }



    public class DB_FileInfo
    {
        public string SID { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FilePath { get; set; }

        public DateTime ModifyTime { get; set; }
    }
}
