using HtmlAgilityPack;
using System;

namespace BaixarPDFXamarin
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "Baixar PDF Xamarin";

            var html = new HtmlWeb().Load("http://developer.xamarin.com/guides/cross-platform/getting_started/");

            var htmlNodes = html.DocumentNode.SelectNodes("//li[contains(@class,'product')]");

            foreach (HtmlNode htmlNode in htmlNodes)
            {
                new MasterTopicDownloader(htmlNode).Start();
            }

        }                
    }
}
