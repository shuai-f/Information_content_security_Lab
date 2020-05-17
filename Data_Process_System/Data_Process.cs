using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

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
                foreach (var item in list) //写入一行
                {
                    // Console.WriteLine(item);
                    // 判断是否为最后一行
                    if (list.Count == list.IndexOf(item) + 1)
                    {
                        sw.Write(item + "\r\n");
                        continue;
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
    }
}