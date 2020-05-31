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
        
        public string view_attributes()
        {
            string result = $"id={id};\nsubject={subject};\ncontent={content};\n"
                + $"date={date};\nurl={url};\ngood={good_count};\ncomments={comment_count};\ntrans={trans_count};\n";
            return result;
        }
    }

        class User
    {
        public string post_id{ get; set; }
        public string user_id{ get; set; }
        public string user_name{ get; set; }
        public string gender{ get; set; }
        public string profile_url { get; set; }
        public string follow_count{ get; set; }
        public string followers_count{ get; set; }
        public string statuses_count{ get; set; }
        
        public string view_attributes()
        {
            string result = $"id={post_id};\nsubject={user_id};\ncontent={user_name};\n"
                + $"date={gender};\nurl={profile_url};\ngood={follow_count};\ncomments={followers_count};\ntrans={statuses_count};\n";
            return result;
        }
    }

}