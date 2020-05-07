using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace c__workspace
{
    class Spider
    {
        private string root_dir = "data/"; // 数据存储根目录

        /// <summary> 写入文件 </summary>
        /// <param name="file_path"> 文件存储路径 </param> 
        /// <param name="data"> 报文/数据 </param> 
        /// <returns> 成功返回True </returns>
        public Boolean write_to_file(string file_path, byte[] data)
        {
            try
            {
                string fileName = DateTime.Now.ToString("yyyy-MM-dd");
                // string filePath = Path.Combine(Environment.CurrentDirectory, this.root_dir);
                if (File.Exists(file_path) == false)
                {
                    File.Create(file_path);
                }
                FileStream fs = new FileStream(file_path, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(data);
                //清空缓冲区               
                sw.Flush();
                //关闭流               
                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        /// <summary> 爬取网页 </summary>
        /// <param name="url"> url </param> 
        /// <returns>  </returns>
        public void crawl_page(string url)
        {
            byte[] data = null;
            return;
        }
    }
}