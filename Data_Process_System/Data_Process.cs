using System;
using System.Collections;
using System.IO;
using AngleSharp;
using AngleSharp.Html.Parser;

namespace c__workspace
{
    class Data_Process
    {
        static string[] list = { "ID", "主题", "博文", "日期", "URL", "点赞数", "评论数", "转发数" };
            
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

        /// <summary> 解析HTML文件 </summary>
        /// <param name="html_path"> HTML文件路径，含文件名 </param> 
        /// <returns>  </returns>
        public static void parse_html(string html_path)
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
                if (url.Contains("comment") && !url.Contains("https"))
                {
                    url = "https://weibo.cn" + url;
                    post_url_list.Add(url);
                    Console.WriteLine(url);
                    Console.WriteLine(item.TextContent);
                }
            }
            Spider spider = new Spider();
            // spider.parse_html(page);
        }

        /// <summary> 初始化csv文件 </summary>
        /// <param name="file_name"> 文件名 </param> 
        /// <returns>  </returns>
        private static void init_csv(string file_name)
        {
            File.Create(file_name);
            var sw = new StreamWriter(new FileStream(file_name,FileMode.Append));
            foreach (var item in list)
            {
                if (item.Equals(list[list.Length - 1 ])) 
                {
                    sw.Write(item + "\r\n");
                    continue;
                }
                sw.Write(item + ",");
            }
        }

        /// <summary> 博文存储为csv文件 </summary>
        /// <param name="list"> 写入参数Arraylist<Arraylist<>> </param> 
        /// <returns>  </returns>
        public static Boolean store_to_csv(ArrayList list)
        {
            string file_name = "Data_Stored//post.csv";
            if (!File.Exists(file_name))
            {
                File.Create(file_name).Close();
                init_csv(file_name);
            }
            var sw = new StreamWriter(new FileStream(file_name,FileMode.Append));
            foreach (var item_list in list)
            {
                if (((ArrayList)item_list).Count != Data_Process.list.Length)
                {
                    Console.WriteLine("写入csv文件:参数不对");
                    Console.WriteLine(((ArrayList)item_list).ToString());
                    return false;
                }
                foreach (var item in (ArrayList)item_list)
                {
                    if (list.Count == list.IndexOf(item) + 1)
                    {
                        sw.Write(item + "\r\n");
                        continue;
                    }
                    sw.Write(item + ",");
                }
            }
            return true;
        }
    }
}