using System;

namespace c__workspace
{
    /// <summary>
    /// 解析html
    /// </summary>
    public class Belle
    {
            /// <summary> 标题 </summary>
            public string Title { get; set; }
            /// <summary> 图片地址 </summary>
            public string ImageUrl { get; set; }
    }

    class Post
    {
        public string id{ get; set; }
        public string subject{ get; set; }
        public string content{ get; set; }
        public string date{ get; set; }
        public string url { get; set; }
        public string good_count{ get; set; }
        public string comment_count{ get; set; }
        public string trans_count{ get; set; }
    }

}