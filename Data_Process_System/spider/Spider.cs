using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace c__workspace
{
    class Spider
    {
        private string root_dir = "spider\\data\\"; // 数据存储根目录

        /// <summary> 写入文件 </summary>
        /// <param name="file_path"> 文件存储路径 </param> 
        /// <param name="data"> 报文/数据 </param> 
        /// <returns> 成功返回True </returns>
        public Boolean write_to_file(string file_path, string data)
        {
            try
            {
                // string fileName = DateTime.Now.ToString("yyyy-MM-dd");
                if (File.Exists(file_path) == false)
                {
                    File.Create(file_path);
                }
                FileStream fs = new FileStream(file_path, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(data);
                //清空缓冲区               
                sw.Flush();
                //关闭流               
                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        /// <summary> 爬取网页 </summary>
        /// <param name="url"> url </param> 
        /// <returns>  </returns>
        public void crawl_page(string url)
        {
            string data = null;
            string file_path = url+ ".html";
            write_to_file(file_path, data);
            return;
        }

        /// <summary> 传入URL返回网页的html代码 </summary>
        /// <param name="Url"> Url </param>
        /// <returns>返回页面的源代码</returns>
        public string GetUrltoHtml(string Url)
        {
            Encoding encode = new UTF8Encoding();
            try
            {
                HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(Url);
                //伪造浏览器数据，避免被防采集程序过滤
                wRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215; CrazyCoder.cn;www.aligong.com)";

                HttpWebResponse wRespond = (HttpWebResponse)wRequest.GetResponse();
                
                // 获取输入流
                Stream respStream = wRespond.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, encode);
                string content = reader.ReadToEnd();
                Console.WriteLine(content);

                // 写入文件
                string[] splits = Url.Split("/");
                string file_path = this.root_dir + splits[splits.Length - 1] + ".html";
                new Connect_to_MySQL().insert(splits[splits.Length - 1], Url);
                write_to_file(file_path, content);
                
                reader.Close();
                reader.Dispose();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }
}