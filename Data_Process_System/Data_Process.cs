using System;
using System.Collections;
using System.IO;

namespace c__workspace
{
    class test_data_process
    {
        // public static void main(string[] args){
            
        // }
    }

    class Data_Process
    {
        public void package_store(string package)
        {

        }

        /// <summary> 读文件，按行读取 </summary>
        /// <param name="file_path"> 文件路径（含文件名） </param> 
        /// <returns> 返回读取列表 </returns>
        public ArrayList read_file(string file_path)
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

    }
}