using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace 信息内容安全实验
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public HashSet<string> keyword = new HashSet<string>();
        public HashSet<string> blog = new HashSet<string>();
        public  ArrayList website = new ArrayList();
        public Dictionary<string, List<string>> reduction = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> wbs = new Dictionary<string, List<string>>();
        public Dictionary<string, double> userdata = new Dictionary<string, double>();
        public Dictionary<string, string> weibos = new Dictionary<string, string>();
        public List<weibo> wb = new List<weibo>();
        public Dictionary<string, int> index = new Dictionary<string, int>();
        public Dictionary<string, int> index1 = new Dictionary<string, int>();

        public Form2 form2 = new Form2();

        public void keyword_match()
        {
            int i = -1;
            int flag = 0;
            StreamReader sr = new StreamReader("post.csv");
            while (!sr.EndOfStream)
            {
                
                string line = sr.ReadLine();
                if (flag == 0)
                {
                    flag++;
                    continue;
                }
                else flag++;
                string[] values = line.Split(',');

                weibo mb = new weibo();
                mb.wid = values[0];
                mb.theme = values[1];
                mb.article = values[2];
                mb.time = values[3];
                mb.url = values[4];
                //mb.like = int.Parse(values[5]);
                mb.like = Convert.ToInt32(values[5]);
                mb.comment = Convert.ToInt32(values[6]);
                mb.share = Convert.ToInt32(values[7]);

                wb.Add(mb);
                i++;
                if(!index.ContainsKey(values[0]))
                index.Add(values[0], i);
                if (!index1.ContainsKey(values[4]))
                    index1.Add(values[4], i);

                if (!weibos.ContainsKey(values[4]))
                weibos.Add(values[4], values[2]);

                if(!keyword.Contains(values[1]))
                {
                    reduction.Add(values[1], new List<string>());
                    
                }
                reduction[values[1]].Add(values[4]);
                keyword.Add(values[1]);
                if (!blog.Contains(values[2]))
                {
                    wbs.Add(values[2], new List<string>());
                }    
                wbs[values[2]].Add(values[4]);
                blog.Add(values[2]);
            }
            sr.Close();
            sr.Dispose();
            foreach (string s in reduction.Keys)
            {
                listBox1.Items.Add("关键字  " + s + "  出现的次数为：" + reduction[s].Count());
                
            }
        }
        public void user_match()
        {
            
            int i = 0;
            int j = 0;
            int flag = 0;
            StreamReader sr = new StreamReader("user.csv");
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (flag == 0)
                {
                    flag++;
                    continue;
                }
                else flag++;
                string[] values = line.Split(',');

                weibo mb = wb[index[values[0]]];
                mb.uid = values[1];
                mb.user = values[2];
                mb.sex = values[3];
                mb.userurl = values[4];
                mb.following = Convert.ToInt32(values[5]);
                mb.fans = Convert.ToInt32(values[6]);
                mb.weibos = Convert.ToInt32(values[7]);

                if (values[3].Equals("男"))
                    i++;
                else j++;
            }
            sr.Close();
            sr.Dispose();
            userdata.Add("男", i);
            userdata.Add("女", j);
        }
        public void find_website()
        {
            int flag = -1;
            foreach (string s in wbs.Keys)
            {
                
                if(ExecuteKMP(s, textBox1.Text) > 0)
                {
                    flag = 1;
                    foreach (string web in wbs[s])
                        listBox1.Items.Add(web);
                }                
            }
            if(flag < 0)
            {
                MessageBox.Show("包含该关键词的微博不存在", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
             /*   
            if (reduction.ContainsKey(textBox1.Text))
            {
                foreach (string s in reduction[textBox1.Text])
                    listBox1.Items.Add(s);
            }
            else
            {
                MessageBox.Show("包含该关键词的微博不存在", "错误提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
             */
        }   
        private void button1_Click(object sender, EventArgs e)
        {
            keyword_match();  
            user_match();
            pictureBox1.Image = GetBitmap("宋体", userdata);
            //e.Graphics.DrawImage(GetBitmap("tongjitu", userdata), new Point(0, 0));
            //paint();

        }
        public void paint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(GetBitmap("宋体", userdata), new Point(0, 0));
        }
        public static int[] Next(string pattern)
        {
            int[] next = new int[pattern.Length];
            next[0] = -1;
            if (pattern.Length < 2) //如果只有1个元素不用kmp效率会好一些
            {
                return next;
            }

            next[1] = 0;    //第二个元素的回溯函数值必然是0，可以证明：
            //1的前置序列集为{空集,L[0]}，L[0]的长度不小于1，所以淘汰，空集的长度为0，故回溯函数值为0
            int i = 2;  //正被计算next值的字符的索引
            int j = 0;  //计算next值所需要的中间变量，每一轮迭代初始时j总为next[i-1]
            while (i < pattern.Length)    //当i == pattern.Length时所有字符的next值都已计算完毕
            { //状态点
                if (pattern[i - 1] == pattern[j])   //迭代计算next值是从第三个元素开始的
                {   //如果L[i-1]等于L[j]，那么next[i] = j + 1
                    next[i++] = ++j;
                }
                else
                {   //如果不相等则检查next[i]的下一个可能值----next[j]
                    j = next[j];
                    if (j == -1)    //如果j == -1则表示next[i]的值是1
                    {   
                        next[i++] = ++j;
                    }
                }
            }
            return next;
        }

        public static int ExecuteKMP(string source, string pattern)
        {
            int[] next = Next(pattern);
            int i = 0;  //主串指针
            int j = 0;  //模式串指针
            //如果子串没有匹配完毕并且主串没有搜索完成
            while (j < pattern.Length && i < source.Length)
            {
                if (source[i] == pattern[j])    //i和j用于指示本轮迭代中要判断是否相等的主串字符和模式串字符
                {
                    i++;
                    j++;
                }
                else
                {
                    j = next[j];    //依照指示迭代回溯
                    if (j == -1)    //回溯有情况，这是第二种
                    {
                        i++;
                        j++;
                    }
                }
            }
            //如果j==pattern.Length则表示循环的退出是由于子串已经匹配完毕而不是主串用尽
            return j < pattern.Length ? -1 : i - j;
        }

        private Bitmap GetBitmap(string familyName, Dictionary<string, double> data)
        {
            int r = 70;
            Bitmap bitmap = new Bitmap(350, 300);
            Graphics graphics = Graphics.FromImage(bitmap);
            //用白色填充整个图片，因为默认是黑色
            graphics.Clear(Color.White);
            //抗锯齿
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //高质量的文字
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //像素均偏移0.5个单位，以消除锯齿
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            //第一个色块的原点位置
            PointF basePoint = new PointF(10, 20);
            //色块的大小
            SizeF theSize = new SizeF(45, 16);
            //第一个色块的说明文字的位置
            PointF textPoint = new PointF(basePoint.X + 50, basePoint.Y);
            foreach (var item in data)
            {        
                RectangleF baseRectangle = new RectangleF(basePoint, theSize);
                //画代表色块
                graphics.FillRectangle(new SolidBrush(getColor(item.Key.ToString())), baseRectangle);
                graphics.DrawString(item.Key.ToString(), new Font(familyName, 11), Brushes.Black, textPoint);
                basePoint.Y += 30;
                textPoint.Y += 30;
            }
            //扇形区所在边框的原点位置
            Point circlePoint = new Point(Convert.ToInt32(textPoint.X + 90), 35);
            //总比 初始值
            float totalRate = 0;
            //起始角度 Y周正方向
            float startAngle = 30;
            //当前比 初始值
            float currentRate = 0;
            //圆所在边框的大小
            Size cicleSize = new Size(r * 2, r * 2);
            //圆所在边框的位置
            Rectangle circleRectangle = new Rectangle(circlePoint, cicleSize);
            foreach (var item in data)
            {
                totalRate += float.Parse(item.Value.ToString());
            }
            foreach (var item in data)
            {
                currentRate = float.Parse(item.Value.ToString()) / totalRate * 360;
                graphics.DrawPie(Pens.White, circleRectangle, startAngle, currentRate);
                graphics.FillPie(new SolidBrush(getColor(item.Key.ToString())), circleRectangle, startAngle, currentRate);
                //至此 扇形图已经画完，下面是在扇形图上写上说明文字

                //当前圆的圆心 相对图片边框原点的坐标               
                PointF cPoint = new PointF(circlePoint.X + r, circlePoint.Y + r);
                //当前圆弧上的点
                //cos(弧度)=X轴坐标/r
                //弧度=角度*π/180
                double relativeCurrentX = r * Math.Cos((360 - startAngle - currentRate / 2) * Math.PI / 180);
                double relativecurrentY = r * Math.Sin((360 - startAngle - currentRate / 2) * Math.PI / 180);
                double currentX = relativeCurrentX + cPoint.X;
                double currentY = cPoint.Y - relativecurrentY;
                //内圆上弧上的 浮点型坐标
                PointF currentPoint = new PointF(float.Parse(currentX.ToString()), float.Parse(currentY.ToString()));
                //外圆弧上的点
                double largerR = r + 25;
                double relativeLargerX = largerR * Math.Cos((360 - startAngle - currentRate / 2) * Math.PI / 180);
                double relativeLargerY = largerR * Math.Sin((360 - startAngle - currentRate / 2) * Math.PI / 180);
                double largerX = relativeLargerX + cPoint.X;
                double largerY = cPoint.Y - relativeLargerY;
                //外圆上弧上的 浮点型坐标
                PointF largerPoint = new PointF(float.Parse(largerX.ToString()), float.Parse(largerY.ToString()));
                //将两个点连起来
                //graphics.DrawLine(Pens.Black, currentPoint, largerPoint);
                //外圆上 说明文字的位置
                PointF circleTextPoint = new PointF(float.Parse(largerX.ToString()), float.Parse(largerY.ToString()));
                //在外圆上的点的附近合适的位置 写上说明
                if (largerX >= 0 && largerY >= 0)//第1象限  实际第二象限
                {
                    //circleTextPoint.Y -= 15;                    
                    circleTextPoint.X -= 35;
                }
                if (largerX <= 0 && largerY >= 0)//第2象限  实际第三象限
                {
                    circleTextPoint.Y -= 15;                    
                    circleTextPoint.X -= 65;                
                }
                if (largerX <= 0 && largerY <= 0)//第3象限  实际第四象限                
                {
                    circleTextPoint.X -= 45;               
                    circleTextPoint.Y += 30;
                }
                if (largerX >= 0 && largerY <= 0)//第4象限  实际第一象限                
                {
                    circleTextPoint.X -= 15;
                    circleTextPoint.Y += 5;                
                }
                //象限差异解释：在数学中 二维坐标轴中 右上方 全为正，在计算机处理图像时，右下方全为正。相当于顺时针移了一个象限序号                              
                graphics.DrawString(item.Key.ToString() + " " + (currentRate / 360).ToString("p2"), new Font(familyName, 11), Brushes.Black, circleTextPoint);
                startAngle += currentRate;
            }
            return bitmap;

        }
        Color getColor(string scoreLevel)
        {
            Color c = Color.White;
            if (scoreLevel.Contains("男"))
                c = Color.FromArgb(57, 134, 155);
            if (scoreLevel.Contains("女"))
                c = Color.FromArgb(70, 161, 185);
            if (scoreLevel.Contains("一般"))
                c = Color.FromArgb(124, 187, 207);
            if (scoreLevel.Contains("不及格"))
                c = Color.FromArgb(181, 212, 224);
            return c;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            find_website();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int posindex = listBox1.IndexFromPoint(new Point(e.X, e.Y));
                listBox1.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < listBox1.Items.Count)
                {
                    listBox1.SelectedIndex = posindex;
                    contextMenuStrip1.Show(listBox1, new Point(e.X, e.Y));
                }
            }
            listBox1.Refresh();
        }
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(listBox1.SelectedItem);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form2.Owner = this;
        }

        private void 查看ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form2.textBox1.Clear();
            form2.textBox2.Clear();
            form2.textBox3.Clear();
            form2.textBox4.Clear();
            form2.textBox5.Clear();
            form2.textBox6.Clear();
            form2.textBox7.Clear();
            form2.textBox8.Clear();

            string s = listBox1.SelectedItem.ToString();
            weibo mb = wb[index1[s]];
            form2.textBox1.Text = mb.article;
            form2.textBox2.Text = mb.user;
            form2.textBox3.Text = mb.following.ToString();
            form2.textBox4.Text = mb.fans.ToString();
            form2.textBox5.Text = mb.weibos.ToString();
            form2.textBox6.Text = mb.like.ToString();
            form2.textBox7.Text = mb.comment.ToString();
            form2.textBox8.Text = mb.share.ToString();
            form2.textBox9.Text = mb.time;
            form2.ShowDialog();           
        }
    }
}
