using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoSaude.Manager
{
    class Cripto
    {

        private string raw;
        private int key;
        private Dictionary<string, string> replacement = new Dictionary<string, string>
        {
            {"A", "*"},
            {"E", "#"},
            {"I", "+"},
            {"O", "-"},
            {"U", "$"}
        };

        public Cripto(string raw)
        {
            this.raw = raw.ToUpper();
            this.key = 4;
        }

        public string Encrypted
        {
            get
            {
                string keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789";
                string build = "";
                foreach (char entry in this.raw)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (keys[i] == entry)
                        {
                            if (i + this.key >= keys.Length)
                            {
                                build += keys[this.key + (keys.Length - (this.key + i))];
                            }
                            else
                            {
                                build += keys[i + this.key];
                            }
                        }
                    }
                }
                foreach (var replace in this.replacement)
                {
                    build = build.Replace(replace.Key, replace.Value);
                }
                return build;
            }
        }

    }
}
