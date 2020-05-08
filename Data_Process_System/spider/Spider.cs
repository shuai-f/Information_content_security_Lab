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


        /// <summary> 传入URL返回网页的html代码 </summary>
        /// <param name="Url"> Url </param>
        /// <returns>返回页面的源代码</returns>
        public string get_html_from_url(string Url)
        {
            Encoding encode = new UTF8Encoding();
            try
            {
                HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(Url);
                wRequest.AllowAutoRedirect = true; // 自动跳转
                //伪造浏览器数据，避免被防采集程序过滤
                wRequest.UserAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
                wRequest.Method = "GET"; //获取数据的方法
                wRequest.KeepAlive = true; //保持活性
                // 响应
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

        public void parse_html(string html)
        {
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase);
            
        }
    }
}