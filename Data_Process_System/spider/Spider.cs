using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        // public string get_root_dir()
        // {
        //     return this.root_dir;
        // }

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
                FileStream fs = new FileStream(file_path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                Logging.AddLog("Wtite to file successfully.");
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
            string[] splits = url.Split("/");
            string temp = splits[splits.Length - 1];
            while ((temp.IndexOfAny(Path.GetInvalidFileNameChars()))>0) // 违法字符判断
            {
                var index = temp.IndexOfAny(Path.GetInvalidFileNameChars());
                char invalid_char = temp[index];
                temp = temp.Replace(invalid_char,'-');
            }
            if (temp.Length > 200) //太长，做hash取值
            {
                temp = "" + temp.GetHashCode();
            }
            file_name += temp + ".html";
            return file_name;
        }

        /// <summary> 传入URL返回网页的html代码 </summary>
        /// <param name="Url"> Url </param>
        /// <returns>返回页面的源代码, 失败返回空</returns>
        public string get_html_from_url(string Url)
        {
            Encoding encode = new UTF8Encoding();
            try
            {
                Console.WriteLine(Url);
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

                if (wRespond.StatusCode != HttpStatusCode.OK) // 判断响应状态码
                {
                    Console.WriteLine("不 ok");
                    Logging.AddLog("[Warning]:Crawl page failed!");
                    return null;
                }

                // 写入文件
                string file_path = get_file_name(Url);
                string[] splits = Url.Split("/");
                // new Connect_to_MySQL().insert(splits[splits.Length - 1], Url);
                if (!Url.Contains("show?id="))
                    write_to_file(file_path, content);
                
                reader.Close();
                reader.Dispose();
                Logging.AddLog("Crawl page successfully！");
                return content;
            }
            catch (Exception ex)
            {
                Logging.AddLog($"[Exception]:{ex.Message}");
                if (ex.Message.Contains("403")) // 服务器拒绝响应
                {
                    
                }
                Console.WriteLine(ex.Message);
                // Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

        /// <summary>
        /// 辅助：延时函数
        /// </summary>
        /// <param name="second"> 延时秒数 </param>
        public void delay(int second)
        {
            Thread.Sleep(second * 1000);
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
                Connect_to_MySQL.insert(html_path, cur_url);
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

        /// <summary> 关键字搜索 </summary>
        /// <param name="keyword"> keyword </param> 
        /// <param name="n"> page_num </param> 
        /// <returns>  </returns>
        public void search_for_keyword(string keyword,int n)
        {

            // construct url by keyword
            // https://s.weibo.com/weibo/keyword
            var queue = new Queue();
            for (int i = 0; i < n; i++)
            {
                string url = $"https://m.weibo.cn/api/container/getIndex?type=wb&queryVal={keyword}&containerid=100103type=2%26q%3D{keyword}&page={i}";
                queue.Enqueue(url);
            }
            while(queue.Count != 0)
            {
                string cur_url = (string)queue.Dequeue();
                while (get_html_from_url(cur_url) == null) 
                {
                    delay(10);
                }
                Connect_to_MySQL.insert(keyword, cur_url);
                var html_path = get_file_name(cur_url); // 获取报文存储路径
                var jo = Data_Process.get_Json(html_path);
                Data_Process.post_stored(jo);

            }
        }

        /// <summary> 获取评论 </summary>
        /// <param name="id"> 博文id </param> 
        /// <param name="page"> 页数 </param>
        /// <returns>  </returns>
        public void search_comments(string id,int page)
        {
            string path = $"Data_Stored/comments/{id}.txt";
            string data = "";
            var queue = new Queue();
            for (int i = 0; i < page; i++)
            {
                var url = $"https://m.weibo.cn/api/comments/show?id={id}&page={i}";
                queue.Enqueue(url);
            }
            while (queue.Count != 0)
            {
                string cur_url = (string)queue.Dequeue();
                var json_text = get_html_from_url(cur_url);
                if (json_text == null) { // 不 ok
                    break;
                }
                // Console.WriteLine(json_text);
                JObject jObject = (JObject)JsonConvert.DeserializeObject(json_text);
                if (jObject["ok"].ToString().Equals("0")) // 无数据
                {
                    break;
                }
                foreach (var item in jObject["data"]["data"])
                {
                    var text = Data_Process.content_handler(item["text"].ToString()) + "\n";
                    data += text;
                }
            }
            write_to_file(path, data);

        }

        /// <summary> 根据爬取userid用户微博 </summary>
        /// <param name="userid"> userid </param> 
        /// <returns>  </returns>
        public void search_for_userid(string userid)
        {
            var queue = new Queue();
            string url = $"https://m.weibo.cn/api/container/getIndex?type=uid&value={userid}";
            queue.Enqueue(url);
            while(queue.Count != 0)
            {
                string cur_url = (string)queue.Dequeue();
                Console.WriteLine(cur_url);
                get_html_from_url(cur_url);
                Connect_to_MySQL.insert(userid, cur_url);
                var html_path = get_file_name(cur_url); // 获取报文存储路径
                var jo = Data_Process.get_Json(html_path);
            }
        }

        /// <summary> 热点话题追踪 </summary>
        /// <param name="topic"> Topic </param> 
        /// <returns>  </returns>
        public void topic_track(string topic)
        {
            var topic_list = new ArrayList();

            //var url = "https://m.weibo.cn/api/container/getIndex?containerid=100103type%3D38%26q%3D%E8%B5%84%E7%94%9F%E5%A0%82%26t%3D0&page_type=searchall";
            var url = "https://m.weibo.cn/api/container/getIndex?containerid=106003type%3D25%26t%3D3%26disable_hot%3D1%26filter_type%3Drealtimehot&title=%E5%BE%AE%E5%8D%9A%E7%83%AD%E6%90%9C&extparam=cate%3D10103%26pos%3D0_0%26mi_cid%3D100103%26filter_type%3Drealtimehot%26c_type%3D30%26display_time%3D1590247150&luicode=10000011&lfid=231583";
            if (topic.Contains("话题"))
            {
                url = "https://m.weibo.cn/api/container/getIndex?containerid=106003type%3D25%26t%3D3%26disable_hot%3D1%26filter_type%3Dtopicscene&title=%E8%AF%9D%E9%A2%98%E6%A6%9C&extparam=lon%3D%26lat%3D&luicode=10000011&lfid=106003type%3D25%26t%3D3%26disable_hot%3D1%26filter_type%3Drealtimehot";
            }
            if (get_html_from_url(url) == null)
            {
                delay(10);
            }
            var jo = Data_Process.get_Json(get_file_name(url));
            if (topic.Contains("话题"))
            {
                foreach (var item in jo["data"]["cards"]) // 实时热点
                {
                    // Console.WriteLine(item["desc"].ToString());
                    topic_list.Add(item["card_group"][0]["scheme"].ToString());
                }
            }
            else
            {
                foreach (var item in jo["data"]["cards"][0]["card_group"]) // 实时热点
                {
                    Console.WriteLine(item["desc"].ToString());
                    topic_list.Add(item["scheme"].ToString());
                }
            }
            int count = 0;
            foreach (string item in topic_list)
            {
                if (count >= 4)
                {
                    count = 0;
                    delay(200);
                }
                var splits = item.Split("?");
                url = "https://m.weibo.cn/api/container/getIndex?" + splits[1];
                // url = $"https://m.weibo.cn/api/container/getIndex?231522type=1&t=10&q={item}";
                if (get_html_from_url(url) == null)
                {
                    delay(10);
                    break;
                }
                jo = Data_Process.get_Json(get_file_name(url));
                Console.WriteLine(get_file_name(url));
                Data_Process.topic_stored(jo);
                count++;
            }
        }

        public void movie_track()
        {
            var start_url = "https://m.weibo.cn/api/feed/trendtop?containerid=102803_ctg1_3288_-_ctg1_3288";
            get_html_from_url(start_url);
            var jo = Data_Process.get_Json(get_file_name(start_url));
            write_to_file("Data_Stored//json//test.json", jo.ToString());
            foreach (var blog in jo["data"]["statuses"])
            {
                var post = new Post();
                post.id = blog["id"].ToString();
                post.url = "https://m.weibo.cn/detail/" + post.id;
                post.date = blog["created_at"].ToString();
                post.subject = 
                post.content = Data_Process.content_handler(blog["text"].ToString());
                if (post.content.Contains("...全文"))
                {
                    post.content = post.content.Split("...全文")[0];
                }
                post.trans_count = blog["reposts_count"].ToString();
                post.comment_count = blog["comments_count"].ToString();
                post.good_count = blog["attitudes_count"].ToString();
            }

        }
        
        private void login()
        {
            Encoding encode = new UTF8Encoding();
            string url = "https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=https://m.weibo.cn/";
            string postData = $"loginUser=15765513201&loginPassword=f21213";

            //1.获取登录Cookie
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";// POST OR GET， 如果是GET, 则没有第二步传参，直接第三步，获取服务端返回的数据
            req.AllowAutoRedirect = true;//服务端重定向。一般设置false
            req.ContentType = "application/x-www-form-urlencoded";//数据一般设置这个值，除非是文件上传

            byte[] postBytes = Encoding.ASCII.GetBytes(postData);
            req.ContentLength = postBytes.Length;
            Stream postDataStream = req.GetRequestStream();
            postDataStream.Write(postBytes, 0, postBytes.Length);
            postDataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream respStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(respStream, encode);
            string result = reader.ReadToEnd();
            write_to_file(get_file_name(url), result)                                     ;
            // this.cookie_str = resp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
            Console.WriteLine(resp.Headers.ToString());
            
        }

        /// <summary> 发布一条评论 </summary>
        /// <param name="post_id"> 博文ID </param>
        /// <param name="comment"> 评论内容 </param>
        public void post_comments(string post_id, string comment)
        {
            Encoding encode = new UTF8Encoding();
            try
            {
                // login();
                var Url = "https://m.weibo.cn/api/comments/create";
                // 参数
                var param = $"content={comment}&mid={post_id}";
                byte[] bytes = Encoding.ASCII.GetBytes(param);


                HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(Url);
                wRequest.AllowAutoRedirect = true; // 自动跳转
                //伪造浏览器数据，避免被防采集程序过滤
                wRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
                wRequest.Method = "POST"; //获取数据的方法
                wRequest.ContentType = "application/x-www-form-urlencoded"; // 数据提交方式
                wRequest.ContentLength = bytes.Length;
                wRequest.Headers.Add("Cooklie", this.cookie_str);
                wRequest.CookieContainer = new CookieContainer();
                wRequest.CookieContainer.SetCookies(wRequest.RequestUri, this.cookie_str);

                Console.WriteLine(wRequest.Method + " " + wRequest.RequestUri + " " + wRequest.ProtocolVersion);
                Console.WriteLine("Accept:"+wRequest.Accept);
                Console.WriteLine("Host:"+wRequest.Host);
                Console.WriteLine(wRequest.Headers.ToString());
                
                using (Stream reqStream = wRequest.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    Console.WriteLine(param);
                }
                // 响应
                HttpWebResponse wRespond = (HttpWebResponse)wRequest.GetResponse();

                // 获取输入流
                Stream respStream = wRespond.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, encode);
                string result = reader.ReadToEnd();
                write_to_file(get_file_name(Url), result);
                var jo = Data_Process.get_Json(get_file_name(Url));
                Console.WriteLine(jo.ToString());
            }
            catch (Exception ex)
            {
                Logging.AddLog($"[Exception]:{ex.Message}");
                if (ex.Message.Contains("403")) // 服务器拒绝响应
                {
                    
                }
                Console.WriteLine(ex.Message);
                // Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary> 解析html网页匹配链接 </summary>
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
    
    }
}