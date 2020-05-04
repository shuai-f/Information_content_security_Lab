using System;

namespace c__workspace
{
    class Test
    {
        static void Main(string[] args)
        {
            test_connect();
            test_logging();
        }

        static void test_logging() {
            Logging.AddLog("hello,world");
        }

        static void test_connect() {
            Connect_to_MySQL connect = new Connect_to_MySQL();
            connect.insert("keyword", "10241");
        }
    }
}