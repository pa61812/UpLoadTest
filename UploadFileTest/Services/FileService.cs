using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Dapper;
using UploadFileTest.Models;

namespace UploadFileTest.Services
{
    public class FileService
    {
        //要連接資料庫所需要的 SqlConnection 物件
        public static string connectionStrings = WebConfigurationManager.ConnectionStrings["PracticeConnectionString"].ConnectionString;
        public static SqlConnection dataConnection = new SqlConnection(connectionStrings);

        public static bool InsertFile(FileUpload fileContent)
        {
            bool result = true;
            var files = fileContent.files;
          
            try
            {
                if (files.ElementAt(0) != null)
                {
                    for (int i = 0; i < files.Count(); i++)
                    {
                        var filepath = ConfigurationManager.AppSettings["UploadFileTempPath"];
                        filepath = CheckLocation(filepath);
                        filepath = Path.Combine(filepath, "FileService");
                        //資料夾若不在
                        if (!Directory.Exists(filepath))
                        {
                            //建立資料夾
                            Directory.CreateDirectory(filepath);
                        }
                        string path = Path.Combine(filepath, files.ElementAt(i).FileName);
                        files.ElementAt(i).SaveAs(path);
                        byte[] filebyte = null;
                        filebyte = UploadFile(path);
                        FileContent fileUpload = new FileContent
                        {
                            File_Content = filebyte,
                            File_ContentType = files.ElementAt(i).ContentType,
                            File_Name = files.ElementAt(i).FileName,
                            REASON = fileContent.REASON
                        };

                        string strsql = "Insert into  FileContent " +
                                         "values(@Reason,@File_Name,@File_Content,@File_ContentType)";

                        using (dataConnection)
                        {
                            dataConnection.Open();
                            //加上BeginTrans
                            using (var transaction = dataConnection.BeginTransaction())
                            {
                                try
                                {

                                
                                    //加上BeginTrans
                                    dataConnection.Execute(strsql, fileUpload, transaction);


                                    //正確就Commit
                                    transaction.Commit();
                                    dataConnection.Close();
                                }
                                catch (Exception)
                                {
                                    transaction.Rollback();
                                    dataConnection.Close();
                                    throw;
                                }
                            }
                        }
                        

                    }
                }



                return result;
            }
            catch (Exception ex)
            {
                result = false;
                return result;
                throw;
            }
        }



        public static byte[] UploadFile(string filePath)
        {
            try
            {
                // byte[] buffer = null;
                // buffer = File.ReadAllBytes(filePath);
                // return buffer;
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                Byte[] byteData = new Byte[file.Length];
                file.Read(byteData, 0, byteData.Length);
                file.Close();
                return byteData;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public static List<FileContent> FindFile(string fileId) 
        {

    
            string strMsgSelect = "select*  from FileContent ";
            using (var connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["PracticeConnectionString"].ConnectionString))
            {
                var anonymousList = connection.Query(strMsgSelect).ToList();
                var orderDetails = connection.Query<FileContent>(strMsgSelect).ToList();

                return orderDetails;
            }


        }

        public static Byte[] ToZip(Byte[] byteData, String filename)
        {
            using (var ms = new MemoryStream())
            {
                using (var archive =
                      new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    var zipEntry2 = archive.CreateEntry(filename,
                                CompressionLevel.Fastest);
                    using (var zipStream = zipEntry2.Open())
                    {
                        zipStream.Write(byteData, 0, byteData.Length);
                    }
                }
                FileStream fs = null;
                var filepath = ConfigurationManager.AppSettings["UploadFileTempPath"];
                filepath = CheckLocation(filepath);
                filepath = Path.Combine(filepath, "ServiceUpload");
                //資料夾若不在
                if (!Directory.Exists(filepath))
                {
                    //建立資料夾
                    Directory.CreateDirectory(filepath);
                }
                string output = DateTime.Now.ToString("yyyyMMdd") + "_Output.zip";
                filepath = Path.Combine(filepath, output);
                fs = File.Create(filepath, ms.ToArray().Length);
                fs.Write(ms.ToArray(), 0, ms.ToArray().Length);
                fs.Close();
                return ms.ToArray();
            }
        }



        /// <summary>
        /// 確認路徑是否最後為\
        /// </summary>
        /// <param name="str">要檢查的路徑</param>
        public static string CheckLocation(string str)
        {

            string _str = str.Substring(str.Length - 1, 1);
            if (_str != "\\")
            {
                str = str + "\\";
            }
            return str;

        }


    }
}