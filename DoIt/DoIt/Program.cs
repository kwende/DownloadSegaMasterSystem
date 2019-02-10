using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoIt
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient wc = new WebClient();

            HtmlDocument doc = new HtmlDocument();
            string str = wc.DownloadString("https://edgeemu.net/browse-sms.htm");
            doc.LoadHtml(str);

            HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//div[@id='content']/p[1]/a/@href");
            
            foreach(HtmlNode link in links)
            {
                string page = link.Attributes["href"].Value;

                string pageContent = wc.DownloadString($"https://edgeemu.net/{page}");
                Console.WriteLine($"From page {page}..."); 

                doc = new HtmlDocument();
                doc.LoadHtml(pageContent); 

                HtmlNodeCollection coll = doc.DocumentNode.SelectNodes("//tr/td/a");

                foreach (HtmlNode node in coll)
                {
                    string path = node.Attributes["href"].Value;

                    string pageUrl = $"https://edgeemu.net/{path}";

                    HtmlDocument doc2 = new HtmlDocument();
                    doc2.LoadHtml(wc.DownloadString(pageUrl));

                    HtmlNodeCollection coll2 = doc2.DocumentNode.SelectNodes("//div[@class='content']/table/tr/td[1]/a/@href");

                    foreach (HtmlNode node2 in coll2)
                    {
                        string part = node2.Attributes["href"].Value;
                        string name = node2.InnerHtml;

                        Console.WriteLine($"\tDownloading {name}..."); 

                        string downloadUrl = $"https://edgeemu.net/{part}";

                        string archive = $"E:/sega/{name}"; 
                        wc.DownloadFile(downloadUrl, archive);

                        ZipFile.ExtractToDirectory(archive, "E:/sega");

                        using (var md5 = MD5.Create())
                        {
                            using (var stream = File.OpenRead(archive))
                            {
                                Console.WriteLine($"\t\t{BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant()}"); 
                            }
                        }

                        File.Delete(archive); 
                    }

                }
            }

            
        }
    }
}
