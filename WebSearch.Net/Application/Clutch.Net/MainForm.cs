using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WebSearch.Crawler.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using WebSearch.Model.Net;
using WebSearch.Common.Net;
using WebSearch.DataCenter.Net.Lucene;

namespace Clutch.Net
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private WebCrawler crawler = null;
        private string dataDir = "";

        private void btnStart_Click(object sender, EventArgs e)
        {
            // retrieve the params
            string seedSite = this.textBoxSeedSite.Text;
            string sitesDir = this.textBoxSitesFolder.Text;
            MessageBox.Show(sitesDir);
            this.dataDir = this.textBoxDataFolder.Text;
            string indexDir = this.textBoxIndexFolder.Text;
            int startID = (int)this.numStartID.Value;
            int crawlNum = (int)this.numCrawlSize.Value;
            bool dontLeave = this.checkBoxDontLeave.Checked;
            SupportedLanguage lang = SupportedLanguage.English;
            if (this.comboBoxLanguage.SelectedText == "Chinese")
                lang = SupportedLanguage.Chinese;

            // set the indexer
            LuceneIndexer.INDEX_STORE_PATH = indexDir;
            LuceneIndexer.MAX_DOCS_IN_RAM = 50;
            LuceneIndexer.LANGUAGE = lang;
            LuceneIndexer.Initialize();

            if (checkBoxSingleThread.Checked && textBoxUrlFile.Text != "")
            {
                StreamReader reader = new StreamReader(textBoxUrlFile.Text);
                string url = reader.ReadLine();
                StreamWriter writer = new StreamWriter("log.txt");
                this.crawler = new WebCrawler(WebProxies.Get("SJTU"));
                crawler.StartPageID = startID;
                while (url != null)
                {
                    WebPage page = this.crawler.Retrieve(url);
                    if (page != null)
                    {
                        // save the page content
                        File.WriteAllText(this.dataDir + page.ID + ".htm",
                            page.Url + "\n" + page.Html, page.Encoding);
                        // index the page
                        LuceneIndexer.Instance.AddDocument(LuceneWebCollection.ToDocument(page));
                        LuceneIndexer.Close();
                    }
                    else
                    {
                        writer.WriteLine(url);
                    }
                    url = reader.ReadLine();
                }
                reader.Close();
                writer.Close();
                return;
            }

            // start the initializing
            new Thread(delegate() { toolStripStatusLabel_Status.Text = "Initializing"; }).Start();
            
            // set the crawler
            this.crawler = new WebCrawler(seedSite, sitesDir);
            this.crawler.EnableProxy(WebProxies.Get("SJTU"));
            crawler.NewPageFound = new WebCrawler.__NewPageCallback(__NewPageEvent);
            crawler.WebCrawlerFinish = new WebCrawler.__WebCrawlerCallback(
                __CrawlerFinishEvent);
            crawler.StartPageID = startID;
            crawler.MaxCrawlPageNum = crawlNum;
            crawler.DontLeaveSite = dontLeave;

            // start running
            new Thread(delegate() { this.toolStripStatusLabel_Status.Text = "Running"; }).Start();
            this.btnOptimize.Enabled = false;
            this.btnStart.Enabled = false;
            this.btnPause.Enabled = true;
            this.btnStop.Enabled = true;

            if (!this.crawler.Start())
                MessageBox.Show("The seed page cannot be crawled, " +
                    "please re-select a seed page");
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (this.crawler != null && this.crawler.Pause())
            {
                this.btnPause.Enabled = false;
                this.btnResume.Enabled = true;
                this.toolStripStatusLabel_Status.Text = "Pause";
            }
            else
                this.toolStripStatusLabel_Status.Text = "Pause Failed";
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            if (this.crawler != null && this.crawler.Resume())
            {
                this.btnResume.Enabled = false;
                this.btnPause.Enabled = true;
                this.toolStripStatusLabel_Status.Text = "Running";
            }
            else
                this.toolStripStatusLabel_Status.Text = "Resume Failed";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.crawler != null)
            {
                this.crawler.Stop();
            }
            this.btnOptimize.Enabled = true;
        }

        private void __CrawlerFinishEvent()
        {
            MessageBox.Show("The Work is Done!");
            this.toolStripStatusLabel_Status.Text = "Finished";
            this.btnStop.Enabled = false;
            this.btnResume.Enabled = false;
            this.btnPause.Enabled = false;
            this.btnStart.Enabled = true;
        }

        private void __NewPageEvent(WebPage page)
        {
            int threads = Process.GetCurrentProcess().Threads.Count;
            if (threads > 50)
                this.crawler.SetTimer(1000);
            else if (threads > 25)
                this.crawler.SetTimer(400);
            else
                this.crawler.SetTimer(100);

            labelThreadNum.Text = threads.ToString();
            //if (threads == 0)
            //    this.statusStrip1.Update();

            this.labelCurrentPage.Text = page.ID.ToString();
            this.linkLabelCurrentPage.Text = page.Url;

            // save the page content
            File.WriteAllText(this.dataDir + page.ID + ".htm",
                page.Url + "\n" + page.Html, page.Encoding);

            // index the page
            LuceneIndexer.Instance.AddDocument(LuceneWebCollection.ToDocument(page));
            LuceneIndexer.Close();
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxSitesFolder.Text = folderBrowserDialog1.SelectedPath + "\\";
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxDataFolder.Text = folderBrowserDialog1.SelectedPath + "\\";
            }
        }

        private void btnBrowse3_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxIndexFolder.Text = folderBrowserDialog1.SelectedPath + "\\";
            }
        }

        private void btnOptimize_Click(object sender, EventArgs e)
        {
            LuceneIndexer.Instance.Optimize();
        }

        private void btnBrowse4_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxUrlFile.Text = openFileDialog1.FileName;
            }
        }
    }
}