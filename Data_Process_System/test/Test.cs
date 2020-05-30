using System;
using System.Windows.Forms;

namespace c__workspace
{
    class Test
    {
        static void Main(string[] args)
        {
            // Test_Connect_To_MySQL.test_connect(); 

            // Test_Logging.test_logging();

            // Test_Spider.test_spider_write_file();
            // Test_Spider.test_spider();
            // Test_Spider.test_AngleSharp();
            // Test_Spider.test_do_crawl();
            // Test_Spider.test_keyword_search();
            // Test_Spider.test_get_comments();
            // Test_Spider.test_topic_track();

            // Test_Data_Process.test_get_post();
            // Test_Data_Process.test_parser_html();
            // Test_Data_Process.test_get_Json();
            // Test_Data_Process.test_read_from_csv();

            Test_Front.test_program_run();
        }

    }

    class Test_Data_Process
    {
        /// <summary> 测试解析html </summary>
        public static void test_parser_html()
        {
            var spider = new Spider();
            string url = "https://weibo.cn/comment/J1CBvFHhf?ckAll=1";
            var html_path = spider.get_file_name(url);
            Data_Process.parse_html(html_path);
        }

        /// <summary> 测试从网页获取博文信息 </summary>
        public static void test_get_post()
        {
            var spider = new Spider();
            string url = "https://weibo.cn/comment/J1CBvFHhf?ckAll=1";
            var html_path = spider.get_file_name(url);
            Data_Process.get_post(url,html_path);
        }

        /// <summary> 测试Jobject </summary>
        public static void test_get_Json()
        {
            var spider = new Spider();
            var keyword = "关键字";
            string url = $"https://m.weibo.cn/api/container/getIndex?type=all&queryVal={keyword}&featurecode=20000320&luicode=10000011&lfid=106003type%3D1&title={keyword}&containerid=100103type%3D1%26q%3D{keyword}";
            var html_path = spider.get_file_name(url);
            Data_Process.get_Json(html_path);

        }

        /// <summary> 测试读取csv文件 </summary>
        public static void test_read_from_csv()
        {
            var results = Data_Process.read_from_csv();
            foreach (Post post in results)
            {
                Console.WriteLine(post.view_attributes());
            }
        }
    }

    class Test_Logging
    {
        /// <summary> 测试Log </summary>
        public static void test_logging()
        {
            Logging.AddLog("hello,world");
        }
    }

    class Test_Spider
    {
        /// <summary> 测试写文件 </summary>
        public static void test_spider_write_file()
        {
            string data = null;
            string file_path = "Data_Stored/data/test.txt";
            Spider spider = new Spider();
            spider.write_to_file(file_path, data);
        }

        /// <summary> 测试爬取网页 </summary>
        public static void test_spider()
        {
            Spider spider = new Spider();
            string result = null;
            // string strBaseUrl = "https://weibo.cn/zhouyangqing912?refer_flag=1028035010_&is_hot=1";
            string strBaseUrl = "https://weibo.cn/comment/J1CBvFHhf?ckAll=1";
            result = spider.get_html_from_url(strBaseUrl);
            // result = spider.test_get_html(strBaseUrl);
            Console.WriteLine(result);
            // spider.parse_html(result);

        }

        /// <summary> 测试AngleSharp类 </summary>
        public static void test_AngleSharp()
        {
            var spider = new Spider();
            // string result = null;
            //var url = "https://www.weibo.cn/p/1005051862317094/home?from=page_100505&mod=TAB#place";
            var url = "https://weibo.cn/zhouyangqing912?refer_flag=1028035010_&is_hot=1";
            spider.using_AngleSharp(url);

        }

        /// <summary> 测试执行爬虫 </summary>
        public static void test_do_crawl()
        {
            var spider = new Spider();
            var url = "https://weibo.cn/zhouyangqing912";
            spider.do_crawl(url);
        }

        /// <summary> 测试关键字搜索 </summary>
        public static void test_keyword_search()
        {
            var spider = new Spider();
            var keyword = "猫";
            spider.search_for_keyword(keyword,3);
        }

        /// <summary> 测试获取微博评论 </summary>
        public static void test_get_comments()
        {
            var id = "4506444609948328";
            var page = 1;
            var spider = new Spider();
            spider.search_comments(id, page);
        }

        /// <summary> 测试热点话题追踪 </summary>
        public static void test_topic_track()
        {
            var topic = "";
            var spider = new Spider();
            spider.topic_track(topic);
        }
    }

    class Test_Connect_To_MySQL
    {
        /// <summary> 测试连接数据库 </summary>
        public static void test_connect()
        {
            // connect.insert("keyword", "10241");
            Connect_to_MySQL.select_by_keyword("keyword");
        }
    }

    class Test_Front
    {
        public static void test_program_run()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new WinForm());
        }
        
    }
}