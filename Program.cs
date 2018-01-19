﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace SECCrawler
{
    class Program
    {
        public static void Main(string[] args)
        {
            string baseUrl = "https://www.sec.gov";

            string cikTickerPath = @"C:\Users\Matrix-PC\source\repos\SECCrawler\cik_ticker.csv";
            string wikiSP500FilePath = @"C:\Users\Matrix-PC\source\repos\SECCrawler\sp500List.txt";
            string wikiSP500Content = File.ReadAllText(wikiSP500FilePath);

            List<SECFilingInfo> filings = new List<SECFilingInfo>();

            var cikInfos = ImportCIKInfo(cikTickerPath);
            cikInfos = cikInfos.Where(x => Regex.Match(wikiSP500Content, x.CIK.ToString()).Success);
            //test only
            //cikInfos = cikInfos.Where(x => x.CIK == 1500217);
            foreach (var cik in cikInfos.Take(3))
            {
                SECFilingInfo filingInfo = new SECFilingInfo
                {
                    CompanyInfo = cik,
                    Filings = new List<Filing>()
                };

                Console.WriteLine("Downloading Ticker={0}, CIK={1}", cik.Ticker, cik.CIK);
                string downloadFile = GetSECFilling(cik, ReportType.Annual);

                string path = string.Format("c:\\temp\\SEC\\{0}_{1}", cik.Ticker, cik.CIK);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                XElement xml = XElement.Load(downloadFile);

                //TODO: all entry and go through each entry to store info
                IEnumerable<XElement> entries = xml.Descendants().Where(x => x.Name.LocalName.Equals("entry"));
                foreach(XElement entry in entries)
                {
                    Filing filing = new Filing();
                    var href = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-href")).FirstOrDefault();
                    filing.fileNumber = entry.Descendants().Where(x => x.Name.LocalName.Equals("file-number")).FirstOrDefault().Value.Trim();
                    filing.fileNumberHref = entry.Descendants().Where(x => x.Name.LocalName.Equals("file-number-href")).FirstOrDefault().Value.Trim();
                    filing.FilingDate = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-date")).FirstOrDefault().Value.Trim();
                    filing.FilingHref = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-href")).FirstOrDefault().Value.Trim();
                    filing.FilingType = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-type")).FirstOrDefault().Value.Trim();
                     
                    //get content from filing_href
                    string content = DownloadContent(href.Value);

                    //search content from report
                    Regex regex = new Regex("Archives(.)*10(.){0,4}k(.){0,20}.htm\">", RegexOptions.Singleline);
                    Match result = regex.Match(content);

                    if (result.Value.Length < 10)
                    {
                        Console.WriteLine("UNABLE TO FIND FILING REPORT!!!");
                        filing.DownloadStatus = "Fail";
                        continue;
                    }

                    string reportHref = result.Value.Substring(0, result.Value.IndexOf("\">", 0));
                    string reportUrl = string.Format("{0}/{1}", baseUrl, reportHref);
                    string filePath = string.Format("{0}\\{1}", path, reportHref.Split('/').LastOrDefault());
                    DownloadContentToFile(reportUrl, filePath);

                    filing.DownloadReportPath = filePath;
                    filing.DownloadStatus = "Success";

                    filingInfo.Filings.Add(filing);
                }

                filings.Add(filingInfo);
            }

            string json = JsonConvert.SerializeObject(filings, Newtonsoft.Json.Formatting.Indented);

            string summaryPath = string.Format("c:\\temp\\SEC\\{0}_summary.json", DateTime.Now.ToString("yyyyMMdd"));

            File.WriteAllText(summaryPath, json);

            DataRepository repo = new DataRepository();
            repo.MongoInsert(filings);
            //Task.Run(async () =>
            //{
            //    string jsonContent = File.ReadAllText(@"C:\temp\SEC\20180117_summary.json");
            //    await repo.MongoInsertJsonAsync(jsonContent);
            //}).GetAwaiter().GetResult();

        }

        private static string GetSECFilling(CIKInfo cikInfo, ReportType reportType)
        {

            //string url = "https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany&CIK={cikInfo.CIK}&type=10-K&dateb=&owner=include&count=10";
            string rssUrl = string.Format("https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany&CIK={0}&type=10-K%25&dateb=&owner=include&start=0&count=10&output=atom", cikInfo.CIK);

            string path = string.Format("c:\\temp\\SEC\\{0}_{1}", cikInfo.Ticker, cikInfo.CIK);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string summaryFile = string.Format("{0}\\{1}_{2}.txt", path, cikInfo.Ticker, cikInfo.CIK);

            using (WebClient client = new WebClient())
            {
                try
                {
                    Console.WriteLine(rssUrl);
                    client.DownloadFile(rssUrl, summaryFile);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error for {0}-{1}", cikInfo.Ticker, cikInfo.CIK);
                    Console.WriteLine(ex.Message);
                }
            }

            return summaryFile;
        }

        private static void DownloadContentToFile(string url, string filePath)
        {
            Console.WriteLine(url);

            if (File.Exists(filePath))
            {
                Console.WriteLine("FilePath={0} already exist.", filePath);
                return;
            }

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, filePath);
            }
        }

        private static string DownloadContent(string url)
        {
            Console.WriteLine(url);
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
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

        //TODO
        private IEnumerable<CIKInfo> ParseWikiSP500(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            File.ReadAllLines(filePath)
                .Skip(5)
                .Where(x => x.StartsWith("|-"))
                .Select(x => x.Split(new[] { "||" }, StringSplitOptions.None))
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

            return null;
        }
    }
    
    //CIK|Ticker|Name|Exchange|SIC|Business|Incorporated|IRS

}
