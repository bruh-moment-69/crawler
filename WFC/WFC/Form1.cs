using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;


namespace WFC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Parse(string prnt)
        {
            WebClient webClient = new WebClient();
            string html = webClient.DownloadString(prnt);
            string bodyString = Regex.Replace(html, "<(.*)<body(.+?)>", " ", RegexOptions.Singleline);
            bodyString = Regex.Replace(bodyString, "</body>(.*)>", " ", RegexOptions.Singleline);

            string text = Regex.Replace(bodyString, "<script(.+?)</script>", " ", RegexOptions.Singleline);
            text = Regex.Replace(text, "<style(.+?)(</style>)", " ", RegexOptions.Singleline);
            text = Regex.Replace(text, "<.+?>", " ", RegexOptions.Singleline);
            text = Regex.Replace(text, @"\W+", " ", RegexOptions.Singleline);
            text = Regex.Replace(text, @"\d+", " ", RegexOptions.Singleline);
            text = Regex.Replace(text, @"\s+", " ", RegexOptions.Singleline);

            Regex linkPattern = new Regex(@"href=""[^""]+""", RegexOptions.Singleline);
            MatchCollection matchLinks = linkPattern.Matches(bodyString); //html на bodyString?

            List<string> linksList = new List<string>();
            foreach (Match m in matchLinks)
            {
                string link = Regex.Replace(m.ToString(), "href=\"", "");
                link = Regex.Replace(link, "\"", "");

                if (link.EndsWith(".css") | link.EndsWith(".png"))
                    continue;
                else
                    if (link.StartsWith("#"))
                    continue;
                else
                        if (link.StartsWith("/"))
                    linksList.Add(prnt + link);
                else
                    if (link.StartsWith("http") | link.StartsWith("https"))
                    linksList.Add(link);
            }

            text = text.ToLower();
            richTextBox1.Text = text;

            string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = words.GroupBy(x => x)
                              .Where(x => x.Count() > 1)
                              .Select(x => new { Word = x.Key, Frequency = x.Count() });
            int nnn = 0;
            foreach (var item in result)
            {
                richTextBox2.AppendText("   || Слово: " + item.Word+" Количество повторов: "+ item.Frequency);
            }
            foreach (var item in result)
            {
                if (item.Frequency>nnn)
                {
                    nnn = item.Frequency;
                    richTextBox3.Text = ("Самое часто встречаемое слово: " + item.Word + " Количество повторов: " + item.Frequency);
                }
            }

            foreach (string item in linksList)
            {
                treeView1.SelectedNode.Nodes.Add(item);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            treeView1.Nodes.Add(url);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string url = treeView1.SelectedNode.Text;
            try
            {
                Parse(url);
            }
            catch { }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
