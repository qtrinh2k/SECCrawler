using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;

namespace SECCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            string cikTickerPath = @"C:\Users\Matrix-PC\source\repos\SimpleAngular\SECCrawler\cik_ticker.csv";

            var cikInfos = ImportCIKInfo(cikTickerPath);

            foreach(var cik in cikInfos)
            {
                Console.WriteLine("Downloading Ticker={0}, CIK={1}", cik.Ticker, cik.CIK);
                GetSECFilling(cik, ReportType.Annual);
            }

        }

        private static void GetSECFilling(CIKInfo cikInfo, ReportType reportType)
        {

            //string url = "https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany&CIK={cikInfo.CIK}&type=10-K&dateb=&owner=include&count=10";
            string rssUrl = string.Format("https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany&CIK={0}&type=10-K%25&dateb=&owner=include&start=0&count=10&output=atom", cikInfo.CIK);

            using (WebClient client = new WebClient())
            {
                try
                {
                    Console.WriteLine(rssUrl);
                    client.DownloadFile(rssUrl, string.Format("c:\\temp\\SEC\\{0}_{1}.txt", cikInfo.Ticker, cikInfo.CIK));
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error for {0}-{1}", cikInfo.Ticker, cikInfo.CIK);
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static IEnumerable<CIKInfo> ImportCIKInfo(string cikTickerPath)
        {
            if (!File.Exists(cikTickerPath))
                throw new FileNotFoundException(cikTickerPath);

            IEnumerable<CIKInfo> cikInfos = File.ReadAllLines(cikTickerPath)
                .Skip(1)
                .Select(x => x.Split('|'))
                .Select(x => new CIKInfo
                {
                    CIK = long.Parse(x[0]),
                    Ticker = x[1],
                    Name = x[2],
                    Exchange = x[3],
                    SIC = x[4],
                    Business = x[5],
                    Incorportated = x[6],
                    IRS = x[7]
                });
            return cikInfos;
        }
    }
    
    //CIK|Ticker|Name|Exchange|SIC|Business|Incorporated|IRS
    public class CIKInfo
    {
        public long CIK { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Exchange { get; set; }
        public string SIC { get; set; }
        public string Business { get; set; }
        public string Incorportated { get; set; }
        public string IRS { get; set; }

    }
}
