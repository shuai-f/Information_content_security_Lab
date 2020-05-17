using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;

using AngleSharp;
using AngleSharp.Html.Parser;

namespace c__workspace
{
    class Spider
    {
        // 数据存储根目录
        private string root_dir = "Data_Stored\\data\\"; 
        // Cookie--需自己更新,配置文件存储于Data_Stored
        private string cookie_str = "";

        //构造器
        public Spider()
        {
            this.cookie_str = read_cookie("Data_Stored//Cookie");
        }
        public string get_root_dir()
        {
            return this.root_dir;
        }

        /// <summary> 辅助：读取cookie，按行读取 </summary>
        /// <param name="file_path"> 文件路径（含文件名） </param> 
        /// <returns> 返回读取列表 </returns>
        public string read_cookie(string file_path)
        {
            string result = null;
            try {
                StreamReader streamReader = new StreamReader(file_path);
                string line;
                while ((line = streamReader.ReadLine()) != null) {
                    result = line;
                }
                streamReader.Close();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return result;

        }

        /// <summary> 辅助：写入文件 </summary>
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

        /// <summary> 辅助：获取写入文件文件名 </summary>
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
            string temp = splits[splits.Length - 1].Replace('?','-');
            file_name += temp + ".html";
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
                wRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8"; // 数据提交方式
                wRequest.Headers.Add("cookie", this.cookie_str); // Cookie设置
                // 响应
                HttpWebResponse wRespond = (HttpWebResponse)wRequest.GetResponse();
                
                // 获取输入流
                Stream respStream = wRespond.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, encode);
                string content = reader.ReadToEnd();
                // Console.WriteLine(content);

                if (wRespond.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("不 ok");
                }

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

        /// <summary> 执行爬虫 </summary>
        /// <param name="start_url"> 起始url </param> 
        /// <returns>  </returns>
        public void do_crawl(string start_url)
        {
            var queue = new Queue();
            queue.Enqueue(start_url);
            while(queue.Count != 0)
            {
                string cur_url = (string)queue.Dequeue();
                Console.WriteLine(cur_url);
                get_html_from_url(cur_url);
                var html_path = get_file_name(cur_url);
                if (cur_url.Equals(start_url))
                {
                    var list = Data_Process.parse_html(html_path);
                    foreach (string item in list)
                    {
                        if (queue.Contains(item))
                            continue;
                        queue.Enqueue(item);
                    }
                }
                Data_Process.get_post(cur_url, html_path);
            }

        }



        /// <summary> 解析html网页 </summary>
        /// <param name="html"> 爬取的网页 </param> 
        /// <returns> void </returns>
        public void parse_html(string html)
        {
            html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase);
            string regexString = @"<\s*a\s+[^>]*href\s*=\s*[""'](?<HREF>[^""']*)[""'][^>]*>(?<IHTML>[\s\S]+?)<\s*/\s*a\s*>";
            // string regexString = "<[Aa]\\s*href\\s*=\\s*'.+?'>.+?</[Aa]>";
            Regex regex = new Regex(regexString);
            MatchCollection matchs = regex.Matches(html);
            foreach (Match match in matchs)
            {
                string match_string = match.Groups["HREF"].Value.ToLower();
                if (match_string.Contains("comment"))
                {
                    if (!match_string.Contains("https"))
                    {
                        match_string = "https://weibo.cn" + match_string;
                        Console.WriteLine(match_string);
                    }
                }
                Console.WriteLine(match_string);
            }
        }
    
        /// <summary> Using AngleSharp </summary>
        /// <param name="Url"> 请求网页url </param> 
        /// <returns>  </returns>
        public void using_AngleSharp(string Url) 
        {
            
            //Use the default configuration for AngleSharp
            var config = Configuration.Default.WithDefaultLoader();

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            //Just get the DOM representation
            // var document = await context.OpenAsync(req => req.Content(url));
            var document = context.OpenAsync(Url);
            // Console.WriteLine(document.Result.DocumentElement.OuterHtml);
            // 写入文件
            string file_path = get_file_name(Url);
            string[] splits = Url.Split("/");
            new Connect_to_MySQL().insert(splits[splits.Length - 1], Url);
            write_to_file(file_path, document.Result.DocumentElement.OuterHtml);

            var cells = document.Result.QuerySelectorAll(".panel-body li");

            var list = new ArrayList();
            foreach (var item in cells)
            {
                var belle = new Belle
                {
                    Title = item.QuerySelector("img").GetAttribute("title"),
                    ImageUrl = item.QuerySelector("img").GetAttribute("src")
                };
                list.Add(belle);
            }
            Console.WriteLine(list.Count);
            foreach (var element in list)
            {
                Console.WriteLine(((Belle)element).ImageUrl);
            }

            // Serialize it back to the console
            // Console.WriteLine(document.DocumentElement.OuterHtml);

            // write_to_file(this.root_dir + "AngleSharpTest.html", document.);

        }
    }
}