using System;
using System.Collections;
using System.IO;

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
        public static ArrayList read_file(string file_path)
        {
            ArrayList result = new ArrayList();
            try {
                StreamReader streamReader = new StreamReader(file_path);
                string line;
                while ((line = streamReader.ReadLine()) != null) {
                    result.Add(line);
                }
                streamReader.Close();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return result;

        }

        /// <summary> 解析HTML文件 </summary>
        /// <param name="html_path"> HTML文件路径，含文件名 </param> 
        /// <returns>  </returns>
        public void parse_html(string html_path)
        {
            
        }

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