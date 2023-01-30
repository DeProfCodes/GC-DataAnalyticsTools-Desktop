using Paket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data_Analytics_Tools.Helpers
{
    public class HttpRemoteDownload
    {

        public HttpRemoteDownload(string urlString, string descFilePath)      
        {

        }

        public bool DownloadFile()
        {
            var url = "\\\\102.67.142.10.domain.com\\C$\\folderwithstuff\\folderwithmorestuff\\";
            string fileName = System.IO.Path.GetFileName(url);
            string descFilePathAndName =
            System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            try
            {
                WebRequest myre = WebRequest.Create(url);
            }
            catch
            {
                return false;
            }
            try
            {
                byte[] fileData;
                using (WebClient client = new WebClient())
                {
                    fileData = client.DownloadData(url);
                }
                using (FileStream fs =
                new FileStream(descFilePathAndName, FileMode.OpenOrCreate))
                {
                    fs.Write(fileData, 0, fileData.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("download field", ex.InnerException);
            }
        }

    }

    public class FtpRemoteDownload 
    {
        public FtpRemoteDownload(string urlString, string descFilePath)
        {

        }
        /*
        public bool DownloadFile()
        {
            FtpWebRequest reqFTP;

            string fileName = System.IO.Path.GetFileName(this.UrlString);
            string descFilePathAndName =
            System.IO.Path.Combine(this.DescFilePath, fileName);

            try
            {

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(this.UrlString);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;


                using (FileStream outputStream = new FileStream(descFilePathAndName, FileMode.OpenOrCreate))
                using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
                using (Stream ftpStream = response.GetResponseStream())
                {
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
                return true;
            }

            catch (Exception ex)
            {
                throw new Exception("upload failed", ex.InnerException);
            }
        }
        */
    }
}
