using System;

namespace c__workspace
{
    class Test
    {
        static void Main(string[] args)
        {
            // test_connect();
            // test_logging();
            // test_spider_write_file();
            // test_spider();
            // test_AngleSharp();
            // test_parser_html();
            test_do_crawl();
            // test_get_post();

        }

        static void test_logging()
        {
            Logging.AddLog("hello,world");
        }

        static void test_connect()
        {
            Connect_to_MySQL connect = new Connect_to_MySQL();
            // connect.insert("keyword", "10241");
            connect.select_by_keyword("keyword");
        }

        static void test_spider_write_file()
        {
            string data = null;
            string file_path = "Data_Stored/data/test.txt";
            Spider spider = new Spider();
            spider.write_to_file(file_path, data);
        }

        static void test_spider()
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

        static void test_AngleSharp()
        {
            var spider = new Spider();
            // string result = null;
            //var url = "https://www.weibo.cn/p/1005051862317094/home?from=page_100505&mod=TAB#place";
            var url = "https://weibo.cn/zhouyangqing912?refer_flag=1028035010_&is_hot=1";
            spider.using_AngleSharp(url);

        }

        static void test_parser_html()
        {
            var spider = new Spider();
            string url = "https://weibo.cn/comment/J1CBvFHhf?ckAll=1";
            var html_path = spider.get_file_name(url);
            Data_Process.parse_html(html_path);
        }

        static void test_get_post()
        {
            var spider = new Spider();
            string url = "https://weibo.cn/comment/J1CBvFHhf?ckAll=1";
            var html_path = spider.get_file_name(url);
            Data_Process.get_post(url,html_path);
        }

        static void test_do_crawl()
        {
            var spider = new Spider();
            var url = "https://weibo.cn/zhouyangqing912";
            spider.do_crawl(url);
        }
    }
}