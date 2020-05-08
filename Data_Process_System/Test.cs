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
            test_spider();

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
            string file_path = "spider/data/test.txt";
            Spider spider = new Spider();
            spider.write_to_file(file_path, data);
        }

        static void test_spider()
        {
            Spider spider = new Spider();
            string result = spider.get_html_from_url("https://tieba.baidu.com");
            // Console.WriteLine(result);
        }
    }
}