using System;
using System.Collections;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace c__workspace
{
    /* 类用于连接数据库 
        报文存储：没有原始报文
        关键字：keyword:html_name

        */
    class Connect_to_MySQL
    {
        private string connect_config = "server=localhost;User Id=root;password=123456;Database=information_content_security;port=3306";
        private string keyword_property = "keyword";
        private string page_property = "html_name";
        private string keyword_list = "keywordlist";

        // 数据库插入操作
        public Boolean insert(string keyword, string html_name)
        {
            string query = "insert into " + keyword_list + " (" + keyword_property + "," + page_property + ") values ('" + keyword + "','" + html_name + "')";
            MySqlConnection conn = new MySqlConnection(connect_config);
            MySqlCommand command = new MySqlCommand(query, conn);
            try
            {
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
                Logging.AddLog("insert successfully.");
            }
            catch (Exception e)
            {
                // Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.WriteLine("Insert error! " + keyword + " " + html_name);
            }


            return true;
        }

        // 数据库查询操作
        public ArrayList select_by_keyword(string keyword)
        {
            string query = "select " + page_property + " from " + keyword_list + " where " + keyword_property + " = '" + keyword + "'";
            Console.WriteLine(query);
            ArrayList list = new ArrayList();
            MySqlConnection conn = new MySqlConnection(connect_config);
            MySqlCommand command = new MySqlCommand(query, conn);
            try
            {
                conn.Open();
                MySqlDataReader myReader;
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    Console.WriteLine(myReader.GetString(0));
                    list.Add(myReader.GetString(0));
                }
                myReader.Close();
                conn.Close();
                Logging.AddLog("select successfully.");
            }
            catch (Exception e)
            {
                // Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                Console.WriteLine("Select error! " + keyword);
            }
            return list;
        }

        // 数据库更新操作
        public Boolean update()
        {
            return true;
        }

        // 数据库删除操作
        public Boolean delete()
        {
            return true;
        }
    }
}