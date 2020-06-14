using System;
using System.Collections;

namespace c__workspace
{
    class Secure_Manage
    {
        private ArrayList sensitive_words = new ArrayList();
        public Secure_Manage(ArrayList word_list)
        {
            sensitive_words = word_list;
        }

        public Secure_Manage()
        {
            init();
        }

        private void init()
        {
            var words = Data_Process.read_file("Data_Stored//sensitive_words");
            var list = words.Split("\r\n");
            foreach (var item in list)
            {
                sensitive_words.Add(item);
            }
        }

        public void add_sens_word(string word)
        {
            if (!sensitive_words.Contains(word))
            {
                sensitive_words.Add(word);
            }
        }

        public bool match(string sentence)
        {
            foreach (var item in sensitive_words)
            {
                if (sentence.Contains((string)item))
                {
                    Console.WriteLine(sentence);
                    return true;
                }
            }
            return false;
        }

        public void show_list()
        {
            foreach(var item in sensitive_words)
            {
                Console.WriteLine((string)item);
            }
        }
    }
}