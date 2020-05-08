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
        public string get_root_dir()
        {
            return this.root_dir;
        }

        /// <summary> 写入文件 </summary>
        /// <param name="file_path"> 文件存储路径 </param> 
        /// <param name="data"> 报文/数据 </param> 
        /// <returns> 成功返回True </returns>
        public Boolean write_to_file(string file_path, string data)
        {
            try
            {
                // string fileName = DateTime.Now.ToString("yyyy-MM-dd");
                // if (File.Exists(file_path) == false)
                // {
                //     File.Create(file_path);
                // }
                FileStream fs = new FileStream(file_path, FileMode.Create);
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

        /// <summary> 辅助函数：获取写入文件文件名 </summary>
        /// <param name="url"> url </param> 
        /// <returns> file_name </returns>
        public string get_file_name(string url)
        {
            string file_name = this.root_dir; //数据存储根目录
            string[] splits;
            if (url.Contains("uid")) {
                splits = url.Split("value=");
                file_name += splits[splits.Length - 1] + ".html";
                return file_name;
            }
            splits = url.Split("/");
            file_name += splits[splits.Length - 1] + ".html";
            return file_name;
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
                wRequest.Method = "POST"; //获取数据的方法
                wRequest.KeepAlive = true; //保持活性
                // 响应
                HttpWebResponse wRespond = (HttpWebResponse)wRequest.GetResponse();
                
                // 获取输入流
                Stream respStream = wRespond.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, encode);
                string content = reader.ReadToEnd();
                Console.WriteLine(content);

                // 写入文件
                string file_path = get_file_name(Url);
                string[] splits = Url.Split("/");
                new Connect_to_MySQL().insert(splits[splits.Length - 1], Url);
                write_to_file(file_path, content);
                
                reader.Close();
                reader.Dispose();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return "";
        }

        public string test_get_html(string url) {
            HttpWebRequest webReq;
            HttpWebResponse webResp = null;
            string Response = "";
            webReq = (HttpWebRequest)WebRequest.Create(url);

            webReq.AllowAutoRedirect = true; // 自动跳转
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Method = "GET";
            webReq.KeepAlive = true;
            webResp = (HttpWebResponse)webReq.GetResponse();

            if (webResp.StatusCode == HttpStatusCode.OK)
            {
                StreamReader loResponseStream = new StreamReader(webResp.GetResponseStream(), Encoding.UTF8);
                Response = loResponseStream.ReadToEnd();
                // 写入文件
                string file_path = get_file_name(url);
                string[] splits = url.Split("/");
                new Connect_to_MySQL().insert(splits[splits.Length - 1], url);
                write_to_file(file_path, Response);
            }

            webResp.Close();
            webResp = null;
            webReq = null;
            return Response;
        }

        /// <summary> 解析html网页 </summary>
        /// <param name="html"> 爬取的网页 </param> 
        /// <returns> void </returns>
        public void parse_html(string html)
        {
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase);
            string regexString = @"<\s*a\s+[^>]*href\s*=\s*[""'](?<HREF>[^""']*)[""'][^>]*>(?<IHTML>[\s\S]+?)<\s*/\s*a\s*>";
            Regex regex = new Regex(regexString, RegexOptions.IgnoreCase);
            MatchCollection matchs = regex.Matches(html);
            foreach (Match match in matchs)
            {
                string match_string = match.Groups["HREF"].Value.ToLower();
                Console.WriteLine(match_string);
            }
        }
    }
}