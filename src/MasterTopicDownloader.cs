using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BaixarPDFXamarin
{
    public class MasterTopicDownloader
    {
        private readonly HtmlNode htmlRootNode = null;
        private readonly List<string> allLinks = new List<string>();

        public MasterTopicDownloader(HtmlNode htmlNode)
        {
            htmlRootNode = htmlNode;

            Console.WriteLine("\t{0}", htmlNode.SelectSingleNode("./a").InnerText);

            MatchCollection linkMatches = Regex.Matches(htmlRootNode.InnerHtml, "<a href=\"(/.*?)\">");

            foreach (Match linkMatch in linkMatches)
            {
                allLinks.Add(linkMatch.Groups[1].Value);
            }

        }

        public void Start()
        {
            var firstChild = htmlRootNode.SelectSingleNode("./a");

            string rootDir = @"C:\Users\Gabriel\Dropbox\Cursos\Xamarin\" + firstChild.InnerText;

            //Criando diretório raiz
            if (!System.IO.Directory.Exists(rootDir))
                System.IO.Directory.CreateDirectory(rootDir);

            var topicNodes = htmlRootNode.SelectNodes("./ul/li");

            int i = 1;

            foreach (var topicNode in topicNodes)
            {
                firstChild = topicNode.SelectSingleNode("./a");

                string childDir = rootDir + "/" + i + " - " + firstChild.InnerText;

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n-- {0}", firstChild.InnerText);
                Console.ForegroundColor = ConsoleColor.White;

                //Criando diretório de categorias
                if (!System.IO.Directory.Exists(childDir))
                    System.IO.Directory.CreateDirectory(childDir);

                string fileName = (allLinks.FindIndex(l => l == firstChild.Attributes["href"].Value) + 1) + " - " + firstChild.InnerText;
                fileName = Regex.Replace(fileName, @"[:\?\|]", "_").Replace("/", " ").Replace("\\", " ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Baixando: {0}", fileName);
                Console.ForegroundColor = ConsoleColor.Yellow;

                Download(
                            "http://developer.xamarin.com" + firstChild.Attributes["href"].Value + "offline.pdf",
                            childDir + "/" + fileName + ".pdf"
                        );

                var links = topicNode.SelectNodes("./ul/li");

                if (links != null)
                    DownloadRecursive(links, childDir, firstChild.InnerHtml);

                i++;
            }
        }

        private bool HasChild(HtmlNode htmlNode)
        {
            if (htmlNode.SelectNodes("./ul/li") != null)
                return true;
            else
                return false;
        }

        private void DownloadRecursive(HtmlNodeCollection htmlNodes, string path, string parent)
        {
            foreach (var htmlNode in htmlNodes)
            {

                var link = htmlNode.SelectSingleNode("./a");

                string fileName = string.Empty;

                if (allLinks.Exists(l => l == link.Attributes["href"].Value))
                    fileName = (allLinks.FindIndex(l => l == link.Attributes["href"].Value) + 1) + " - ";

                //Se houver filhos, baixa-los
                if (HasChild(htmlNode))
                {
                    string newParent = string.Empty;
                    if (parent != link.InnerText)
                        newParent = parent + "|" + link.InnerText;
                    else
                        newParent = parent;

                    fileName += link.InnerText;
                    fileName = Regex.Replace(fileName, @"[:\/\?\|]", "_");

                    DownloadRecursive(htmlNode.SelectNodes("./ul/li"), path, newParent);
                }
                //Senão, verificar nomenclatura
                else
                {
                    if (parent.Count(c => c == '|') >= 1)
                    {
                        fileName += parent.Split('|').Last() + " - " + link.InnerText;
                    }
                    else
                    {
                        fileName += link.InnerText;
                    }

                    fileName = Regex.Replace(fileName, @"[:\?\|]", "_").Replace("/", " ").Replace("\\", " ");

                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Baixando: {0}", fileName);
                Console.ForegroundColor = ConsoleColor.White;

                Download(
                            "http://developer.xamarin.com" + link.Attributes["href"].Value + "offline.pdf",
                            path + "/" + fileName + ".pdf"
                        );
            }
        }

        private void Download(string url, string path)
        {
            if (!System.IO.File.Exists(path))
            {
                url = WebUtility.HtmlDecode(url);
                path = WebUtility.HtmlDecode(path).Replace("'","");

                bool ok = false;
                do
                {
                    try
                    {

                        new WebClient().DownloadFile(url, path);

                        ok = true;
                    }
                    catch { }
                } while (!ok);
            }
        }
    }
}
