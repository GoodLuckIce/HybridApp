using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HybridCommon.Context;

namespace HybridCommon.StaticResourceHelper
{
    public class HtmlFileManage
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="filename">下载后的存放地址</param>
        public void DownloadFile(string url, string filename)
        {
            long numBytesToRead = 0;
            long numBytesRead = 0;
            try
            {
                var Myrq = (HttpWebRequest)WebRequest.Create(url);
                Myrq.ServicePoint.Expect100Continue = false;
                Myrq.KeepAlive = false;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
                    {
                        return true;
                    };
                }
                var myrp = (HttpWebResponse)Myrq.GetResponse();

                var st = myrp.GetResponseStream();
                var stLength = st.Length;

                var bytes = new byte[stLength + 20];
                numBytesToRead = stLength;
                do
                {
                    // Read may return anything from 0 to 10.
                    int n = st.Read(bytes, (int)numBytesRead, 10);
                    numBytesRead += n;
                    numBytesToRead -= n;
                } while (numBytesToRead > 0);
                st.Close();

                var filedirectory = filename.Substring(0, filename.LastIndexOf("/"));
                if (!Directory.Exists(filedirectory))
                {
                    Directory.CreateDirectory(filedirectory);
                }
                var so = new FileStream(filename, FileMode.Create);
                so.Write(bytes, 0, (int)stLength);
                so.Close();
            }
            catch (Exception e)
            {
                DownloadFile(url, filename);
            }

        }
        public static void Init()
        {
            var fileList = GetHtmlFileList(DeviceInfo.HtmlFolder);
        }

        public static Stream GetResponseData(string filePath)
        {
            if (!File.Exists(DeviceInfo.HtmlFolder + filePath))
            {
                new HtmlFileManage().DownloadFile(Config.HtmlServerHost + filePath, DeviceInfo.HtmlFolder + filePath);
            }
            var file = File.OpenRead(DeviceInfo.HtmlFolder + filePath);
            return file;
        }
        

        public static List<string> GetHtmlFileList(string path)
        {
            var fileList = new List<string>();
            var htmlFolderInfo = new DirectoryInfo(path);
            if (htmlFolderInfo.Exists)
            {
                foreach (var fileSystemInfo in htmlFolderInfo.GetFileSystemInfos())
                {
                    if (fileSystemInfo is DirectoryInfo)
                    {
                        var temp = GetHtmlFileList(fileSystemInfo.FullName);
                        fileList.AddRange(temp);
                    }
                    else if (fileSystemInfo is FileInfo)
                    {
                        fileList.Add(fileSystemInfo.FullName.Replace(DeviceInfo.HtmlFolder, ""));
                    }
                }
            }

            return fileList;
        }
    }
}
