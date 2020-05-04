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
        public Boolean insert(string keyword,string html_name){
            string query = "insert into "+keyword_list+" ("+keyword_property+","+page_property+") values ('"+keyword+"','"+html_name+"')";
            MySqlConnection conn = new MySqlConnection(connect_config);
            MySqlCommand command = new MySqlCommand(query,conn);
            try{
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
                Logging.AddLog("insert successfully");
            } catch (Exception e) {
                // Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.ToString());
                Console.WriteLine("Insert error! " + keyword + " " + html_name);
            }


            return true;
        }

        // 数据库查询操作
        public Boolean select(){
            return true;
        }

        // 数据库更新操作
        public Boolean update(){
            return true;
        }

        // 数据库删除操作
        public Boolean delete(){
            return true;
        }
    }
}