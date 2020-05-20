using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using AngleSharp;
using AngleSharp.Html.Parser;

namespace c__workspace
{
    class Data_Process
    {
        static string[] list = { "ID", "主题", "博文", "日期", "URL", "点赞数", "评论数", "转发数" };
        static string file_name = "Data_Stored//post.csv";
        public void package_store(string package)
        {
            
        }

        /// <summary> 读文件，按行读取 </summary>
        /// <param name="file_path"> 文件路径（含文件名） </param> 
        /// <returns> 返回读取列表 </returns>
        public static string read_file(string file_path)
        {
            string value = null;
            try {
                StreamReader streamReader = new StreamReader(file_path);
                value = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return value;
        }

        /// <summary> 解析HTML文件，获取博文链接列表 </summary>
        /// <param name="html_path"> HTML文件路径，含文件名 </param> 
        /// <returns> 博文链接列表 </returns>
        public static ArrayList parse_html(string html_path)
        {
            var page = read_file(html_path);
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);

            var parser = context.GetService<IHtmlParser>();
            // var document = await context.OpenAsync(req => req.Content(page));
            var document = parser.ParseDocument(page);
            var elements = document.QuerySelectorAll("a[href]");
            var post_url_list = new ArrayList();
            foreach (var item in elements)
            {
                var url = item.GetAttribute("href");
                if (url.Contains(@"/comment/") && !url.Contains("https"))
                {
                    url = "https://weibo.cn" + url;
                    post_url_list.Add(url);
                    Console.WriteLine(url);
                }
            }
            return post_url_list;
            // Spider spider = new Spider();
            // spider.parse_html(page);
        }

        private static ArrayList post_to_arraylist(Post post)
        {
            var list = new ArrayList();
            list.Add(post.id);
            list.Add(post.subject);
            list.Add(post.content);
            list.Add(post.date);
            list.Add(post.url);
            list.Add(post.good_count);
            list.Add(post.comment_count);
            list.Add(post.trans_count);
            return list;
        }
        
        /// <summary> 获取博文信息 </summary>
        /// <param name="html_path"> html路径 </param> 
        /// <returns>  </returns>
        public static void get_post(string url,string html_path)
        {
            var page = read_file(html_path);
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var parser = context.GetService<IHtmlParser>();
            var post = new Post();
            post.url = url;
            post.id = html_path;
            // var document = await context.OpenAsync(req => req.Content(page));
            var document = parser.ParseDocument(page);
            post.content = document.QuerySelector("span.ctt").TextContent;
            post.date = document.QuerySelector("span.ct").TextContent;
            var elements = document.QuerySelectorAll("div");
            var flag = true;
            foreach (var item in elements)
            {
                string content = item.TextContent;
                if (content.Contains("转发") && content.Contains("评论") && content.Contains("赞"))
                {
                    var regex_str = @"\d+";
                    Regex regex = new Regex(regex_str);
                    MatchCollection matchs = regex.Matches(content);
                    post.trans_count = matchs[0].Value;
                    post.comment_count = matchs[1].Value;
                    post.good_count = matchs[2].Value;
                    flag = false;
                    break;
                }
            }
            if (flag)
                return;
            post.subject = "需对正文做字符串匹配，暂时理解为关键字";
            if (store_to_csv(post_to_arraylist(post)))
            {
                Console.WriteLine("stored successfully");
            }
        }

        /// <summary> 初始化csv文件 </summary>
        /// <param name="file_name"> 文件名 </param> 
        /// <returns>  </returns>
        private static void init_csv(string file_name)
        {
            try
            {
                File.Create(file_name).Close();
                var sw = new StreamWriter(new FileStream(file_name,FileMode.Append),System.Text.Encoding.UTF8);
                foreach (var item in list)
                {
                    if (item.Equals(list[list.Length - 1 ])) 
                    {
                        sw.Write(item + "\r\n");
                        continue;
                    }
                    sw.Write(item + ",");
                }
                sw.Close();
                sw.Dispose();
            }
            catch (System.Exception)
            {
                throw;
            }
            
        }

        /// <summary> 博文存储为csv文件 </summary>
        /// <param name="list"> 写入参数Arraylist<String> </param> 
        /// <returns>  </returns>
        public static Boolean store_to_csv(ArrayList list)
        {
            try
            {
                if (!File.Exists(file_name))
                {
                    init_csv(file_name);
                }
                var sw = new StreamWriter(new FileStream(file_name,FileMode.Append),System.Text.Encoding.UTF8);
                if (list.Count != Data_Process.list.Length) // 参数检查
                {
                    Console.WriteLine("写入csv文件:参数不对");
                    Console.WriteLine(list.ToString());
                    return false;
                }
                for (int i = 0; i < list.Count;i++) //写入一行
                {
                    var item = list[i];
                    item = ((string)item).Replace(",", "，");
                    // 判断是否为最后一行
                    if (i == list.Count-1)
                    {
                        sw.Write(item + "\r\n");
                    }
                    sw.Write(item + ",");
                }
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    
        /// <summary> 读取csv文件 </summary>
        /// <param name="">  </param> 
        /// <returns> post_list </returns>
        public static ArrayList read_from_csv()
        {
            var list = new ArrayList();
            FileStream fs = new FileStream(file_name, FileMode.Open, FileAccess.Read);
 
            //StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);  
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每行记录中的各字段内容  
            string[] aryLine = null;
            //标示是否是读取的第一行  
            bool IsFirst = true;
            var strLine = "";
            //逐行读取CSV中的数据  
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst)
                {
                    IsFirst = false;
                }
                else
                {
                    var post = new Post();
                    int i = 0;
                    post.id = aryLine[i++];
                    post.subject = aryLine[i++];
                    post.content = aryLine[i++];
                    post.date = aryLine[i++];
                    post.url = aryLine[i++];
                    post.good_count = aryLine[i++];
                    post.comment_count = aryLine[i++];
                    post.trans_count = aryLine[i++];
                    list.Add(post);
                }
 
            }
 
            sr.Close();
            fs.Close();
            return list;
        }

        /// <summary> 时间处理 </summary>
        /// <param name="original_time"> 解析出的时间 </param> 
        /// <returns> 处理后的博文内容 </returns>
        public static string time_handler(string original_time)
        {
            var current_time = original_time;
            string[] units = { "年", "月", "天", "时", "分钟", "秒" };

            foreach(var unit in units)
            {
                if (!original_time.Contains(unit))
                    continue;
                var splits = original_time.Split(unit); // 11分钟前
                // Console.WriteLine(original_time);
                int timespan = -(int.Parse(splits[0]));
                switch (unit)
                {
                    case "天":
                        current_time = DateTime.Now.AddYears(timespan).ToString();
                        break;
                    case "月":
                        current_time = DateTime.Now.AddMonths(timespan).ToString();
                        break;
                    case "日":
                        current_time = DateTime.Now.AddDays(timespan).ToString();
                        break;
                    case "时":
                        current_time = DateTime.Now.AddHours(timespan).ToString();
                        break;
                    case "分钟":
                        current_time = DateTime.Now.AddMinutes(timespan).ToString();
                        break;
                    case "秒":
                        current_time = DateTime.Now.AddSeconds(timespan).ToString();
                        break;
                }
                break;
            }
            return current_time;
        }
        
        /// <summary> 文章处理 </summary>
        /// <param name="original_content"> 解析出的博文内容 </param> 
        /// <returns> 处理后的博文内容 </returns>
        public static string content_handler(string original_content)
        {
            var current_content = original_content;
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(original_content);
            var value = document.QuerySelector("body");
            current_content = value.TextContent;
            return current_content;
        }

        /// <summary> 读取Json文件 </summary>
        /// <param name="file_path"> 文件存储路径 </param> 
        /// <returns> JObject </returns>
        public static JObject get_Json(string file_path)
        {
			StreamReader sr = new StreamReader(file_path);
            //存储json文本
			string json_text = "";
			while (!sr.EndOfStream) {
				json_text += sr.ReadLine();
			}
			JObject jo = (JObject)JsonConvert.DeserializeObject(json_text);
            // new Spider().write_to_file("Data_Stored//json//json_test.json", jo.ToString());
            var item = jo["data"];
            int count = 0;
            foreach (var temp in item["cards"][0]["card_group"])
            {
                if (!temp["card_type"].ToString().Contains("9")) 
                {
                    continue;
                }
                var post = new Post();
                var blog = temp["mblog"];
                post.content = content_handler(blog["text"].ToString()); // content
                post.trans_count = blog["reposts_count"].ToString();
                post.comment_count = blog["comments_count"].ToString();
                post.good_count = blog["attitudes_count"].ToString();
                post.date = time_handler(blog["created_at"].ToString());
                post.id = blog["user"]["id"].ToString();
                post.subject = blog["source"].ToString(); // 需要修改
                post.url = temp["scheme"].ToString(); // url
                // Console.WriteLine(post.view_attributes());
                if (store_to_csv(post_to_arraylist(post)))
                {
                    // Console.WriteLine("stored successfully");
                }
                count++;
            }
            Console.WriteLine(count);
            return jo;
        }
    }
}