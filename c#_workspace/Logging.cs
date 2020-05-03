using System;
using System.IO;

namespace c__workspace
{

    class Logging
    {

        /// <summary> 记录日志，在日志文件中增加一条 </summary>
        /// <param name="msg"> Message </param> 
        /// <returns> void </returns>
        public static void AddLog(string msg)
        {
            string saveFolder = "Log";//日志文件保存路径
            string tishiMsg = "";
            try
            {
                string fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string filePath = Path.Combine(Environment.CurrentDirectory, saveFolder);
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileAbstractPath = filePath + "\\" + fileName + ".txt";
                FileStream fs = new FileStream(fileAbstractPath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入     
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                msg = time + "," + msg + System.Environment.NewLine;
 
                sw.Write(msg);
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
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                tishiMsg = "[" + datetime + "]写入日志出错：" + ex.Message;
            }
        }
 
        /// <summary> 向日志文件中添加空行 </summary>
        /// <param name="rows"> 空行数 </param> 
        /// <returns> void </returns>
        public static void AddLine(int rows)
        {
            string saveFolder = "Log";
            try
            {
                string fileName = DateTime.Now.ToString("yyyy-MM-dd");
                string filePath = Path.Combine(Environment.CurrentDirectory, saveFolder);
                if (Directory.Exists(filePath) == false)
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileAbstractPath = filePath + "\\" + fileName + ".txt";
                FileStream fs = new FileStream(fileAbstractPath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入    
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < rows; i++)
                {
                    sb.Append(System.Environment.NewLine);
                }
                string newline = sb.ToString();
                sw.Write(newline);
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
                string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string tishiMsg = "[" + datetime + "]写入日志出错：" + ex.Message;
            }
        } 
    }
}
