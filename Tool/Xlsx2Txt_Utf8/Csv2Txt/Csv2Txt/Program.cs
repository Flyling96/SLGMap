using System;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using System.Diagnostics;


//功能目标，将当前目录下的所有xlsx的EXCEL文件导出为csv和txt格式
namespace Csv2Txt
{
    class Program
    {
        static void Main(string[] args)
        {
            string InputFilePath = System.Environment.CurrentDirectory;
            //获取应用程序运行目录
            DirectoryInfo dir = new DirectoryInfo(InputFilePath);
            FileInfo[] filename = dir.GetFiles("*.csv");

            foreach (var item in filename)
            {
                string newFileNameTXT = string.Empty;
                //保存到txt
                newFileNameTXT = InputFilePath + @"\" + item.Name.Replace("csv", "txt");
                Console.WriteLine(newFileNameTXT);
                if (File.Exists(newFileNameTXT))
                {
                    File.Delete(newFileNameTXT);
                }

                StreamReader sr = new StreamReader(InputFilePath + @"\" + item.Name, Encoding.Default, false);
                string contenttxt = sr.ReadToEnd();
                sr.Close();
                FileStream fs = new FileStream(newFileNameTXT, FileMode.CreateNew);
                fs.Close();
                StreamWriter sw = new StreamWriter(newFileNameTXT, false, Encoding.UTF8);
                sw.Write(contenttxt);
                sw.Flush();
            }
        }
    }
}
