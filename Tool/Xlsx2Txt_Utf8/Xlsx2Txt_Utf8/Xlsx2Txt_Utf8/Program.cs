using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
//添加引用--》引用管理器--》COM类型库中找到Microsoft Excel  xx  Object Library
//xx看自己的excel版本
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using System.Diagnostics;


//功能目标，将当前目录下的所有xlsx的EXCEL文件导出为csv和txt格式
namespace Xlsx2Txt_Utf8
{
    class Program
    {
        private static void QuitExcel()
        {
            Process[] excels = Process.GetProcessesByName("EXCEL");
            foreach (var item in excels)
            {
                item.Kill();
            }
        }
        static void Main(string[] args)
        {
            QuitExcel();
            string InputFilePath = System.Environment.CurrentDirectory;  //excel文件目录
            string OutputFilePath_csv = InputFilePath + @"\csv";//excel导出csv目录
            string OutputFilePath_txt = InputFilePath + @"\txt";//excel导出txt目录
            string FileContent = string.Empty;//临时存储文件内容，读取excel内容，存放到csv和txt文件
                                              //创建导出csv和txt文件的目录

            if (Directory.Exists(OutputFilePath_csv))
            {
                Console.WriteLine(OutputFilePath_csv + "存在了");
                Directory.Delete(OutputFilePath_csv, true);
                Directory.CreateDirectory(OutputFilePath_csv);
            }
            else
            {
                Directory.CreateDirectory(OutputFilePath_csv);
            }

            if (Directory.Exists(OutputFilePath_txt))
            {
                Console.WriteLine(OutputFilePath_txt + "存在了");
                Directory.Delete(OutputFilePath_txt, true);
                Directory.CreateDirectory(OutputFilePath_txt);
            }
            else
            {
                Directory.CreateDirectory(OutputFilePath_txt);
            }



            #region  在当前目录创建excel测试用文件
            //// 创建文件名
            // string[] s = new string[10];
            // for (int i = 0; i < 10; i++)
            // {
            //     s[i] = InputFilePath + @"\" + "test" + i + ".xlsx";
            // }
            // //检查目录下是否存在这些文件，如果存在则删除
            // foreach (var item in s)
            // {
            //     if (Directory.Exists(item))
            //     {
            //         Console.WriteLine(item);
            //     }
            //     else
            //     {
            //         File.Delete(item);
            //     }
            // }

            // //在当前目录创建excel测试文件
            // object Nothing = Missing.Value;
            // Application excelApp = new Application();
            // excelApp.Visible = false;
            // foreach (var item in s)
            // {
            //     Workbook excelDoc = excelApp.Workbooks.Add(Nothing);
            //     Worksheet excelSheet = excelDoc.Sheets[1];
            //     excelSheet.Cells[1, 1] = "ID";
            //     excelSheet.Cells[1, 2] = "Name";
            //     excelSheet.Cells[1, 3] = "IP";
            //     excelSheet.Cells[1, 4] = "Desc";
            //     excelSheet.SaveAs(item, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //     excelDoc.Close(false, Type.Missing, Type.Missing);
            // }
            // excelApp.Quit();
            #endregion

            #region 读取当前目录下的所有xlsx文件
            //获取应用程序运行目录
            DirectoryInfo dir = new DirectoryInfo(InputFilePath);
            FileInfo[] filename = dir.GetFiles("*.xlsx");
            Application App = new Application();
            object nothing = Missing.Value;

            foreach (var item in filename)
            {
                string newFileNameCSV = string.Empty;
                //保存到csv
                Workbook AppBook = App.Workbooks.Open(item.FullName, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing);
                Worksheet AppSheet = AppBook.Worksheets[1];
                newFileNameCSV = OutputFilePath_csv + @"\" + item.Name.Replace("xlsx", "csv");
                AppSheet.SaveAs(newFileNameCSV, XlFileFormat.xlCSV, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                AppBook.Close(false, Type.Missing, Type.Missing);
                AppBook = null;
                Console.WriteLine(item);
            }
            foreach (var item in filename)
            {
                string newFileNameTXT = string.Empty;
                //保存到txt
                Workbook AppBook = App.Workbooks.Open(item.FullName, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing);
                Worksheet AppSheet = AppBook.Worksheets[1];
                newFileNameTXT = OutputFilePath_txt + @"\" + item.Name.Replace("xlsx", "txt");
                Console.WriteLine(newFileNameTXT);
                AppBook.SaveAs(newFileNameTXT, XlFileFormat.xlCSV, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
                AppBook.Close(false, Type.Missing, Type.Missing);
                AppBook = null;

                //将txt文件的编码格式修改为UTF8
                StreamReader sr = new StreamReader(newFileNameTXT, Encoding.Default, false);
                string contenttxt = sr.ReadToEnd();
                sr.Close();
                StreamWriter sw = new StreamWriter(newFileNameTXT, false, Encoding.UTF8);
                sw.Write(contenttxt);
                sw.Close();

                //Console.WriteLine(contenttxt);
                Console.WriteLine(newFileNameTXT);
            }
            App.Quit();





            #endregion
            Console.ReadKey();




        }
    }
}
