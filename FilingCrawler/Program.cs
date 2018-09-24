namespace FilingCrawler
{
    using DataRepository;
    using IEXApiHandler;
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    class Program
    {
        const string downloadReportFolder = @"D:\\Projects\\SECReport";

        public static void Main(string[] args)
        {
            string baseUrl = "https://www.sec.gov";
            string cikTickerPath = @"cik_ticker.csv";
            string wikiSP500FilePath = @"sp500List.txt";
            string wikiSP500Content = File.ReadAllText(wikiSP500FilePath);

            var cikInfos = ImportCIKInfo(cikTickerPath);
            cikInfos = cikInfos.Where(x => Regex.Match(wikiSP500Content, x.CIK.ToString()).Success);

            //InsertCompanyData(cikInfos);
            //IEXApiSandbox.InsertIEXData(cikInfos, new List<IEXDataType> { IEXDataType.company, IEXDataType.stats });
            IEXApiSandbox.InsertChartData(cikInfos);
        }
        
        private static void InsertFilings(IEnumerable<CIKInfo>cikInfos)
        {
            string baseUrl = "https://www.sec.gov";
            List<SECFilingInfo> filings = new List<SECFilingInfo>();

            foreach (var cik in cikInfos)
            {
                SECFilingInfo filingInfo = new SECFilingInfo
                {
                    CompanyInfo = cik,
                    Filings = new List<Filing>()
                };

                Console.WriteLine("Downloading Ticker={0}, CIK={1}", cik.Ticker, cik.CIK);
                string downloadFile = GetSECFilling(cik, ReportType.Annual);

                string path = string.Format("{0}\\{1}_{2}", downloadReportFolder, cik.Ticker, cik.CIK);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                XElement xml = XElement.Load(downloadFile);

                //TODO: all entry and go through each entry to store info
                IEnumerable<XElement> entries = xml.Descendants().Where(x => x.Name.LocalName.Equals("entry"));
                Parallel.ForEach(entries, (entry) =>
                {
                    Filing filing = new Filing();
                    var href = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-href")).FirstOrDefault();
                    filing.fileNumber = entry.Descendants().Where(x => x.Name.LocalName.Equals("file-number")).FirstOrDefault().Value.Trim();
                    filing.fileNumberHref = entry.Descendants().Where(x => x.Name.LocalName.Equals("file-number-href")).FirstOrDefault().Value.Trim();
                    filing.FilingDate = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-date")).FirstOrDefault().Value.Trim();
                    filing.FilingHref = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-href")).FirstOrDefault().Value.Trim();
                    filing.FilingType = entry.Descendants().Where(x => x.Name.LocalName.Equals("filing-type")).FirstOrDefault().Value.Trim();

                    string filePath = string.Format("{0}\\{1}_{2}.htm", path, filing.FilingDate, filing.FilingType.Replace("\\", "").Replace("/", ""));
                    filing.DownloadReportPath = filePath;
                    filing.DownloadStatus = "Success";

                    if (File.Exists(filePath))
                    {
                        Console.WriteLine("File={0} already existed!!!", filePath);
                    }
                    else
                    {
                        //get content from filing_href
                        string content = DownloadContent(href.Value);

                        //search content from report
                        Regex regex = new Regex("Archives(.)*10(.){0,4}k(.){0,20}.htm\">", RegexOptions.Singleline);
                        Match result = regex.Match(content);

                        if (result.Value.Length < 10)
                        {
                            Console.WriteLine("UNABLE TO FIND FILING REPORT!!!");
                            filing.DownloadStatus = "Fail";
                        }
                        else
                        {
                            string reportHref = result.Value.Substring(0, result.Value.IndexOf("\">", 0));
                            string reportUrl = string.Format("{0}/{1}", baseUrl, reportHref);
                            filing.ReportOriginalUrl = reportUrl;
                            DownloadContentToFile(reportUrl, filePath);
                        }
                    }

                    lock (filingInfo)
                    {
                        filingInfo.Filings.Add(filing);
                    }
                });

                lock (filings)
                {
                    filings.Add(filingInfo);
                }
            }

            string json = JsonConvert.SerializeObject(filings, Newtonsoft.Json.Formatting.Indented);

            string summaryPath = string.Format("{0}\\{1}_summary.json", downloadReportFolder, DateTime.Now.ToString("yyyyMMdd"));

            File.WriteAllText(summaryPath, json);

            SECFilingCollection repo = new SECFilingCollection();

            Task.Run(async () =>
            {
                await repo.MongoInsert(filings);

            }).GetAwaiter().GetResult();
        }

        private static string GetSECFilling(CIKInfo cikInfo, ReportType reportType)
        {

            //string url = "https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany&CIK={cikInfo.CIK}&type=10-K&dateb=&owner=include&count=10";
            string rssUrl = string.Format("https://www.sec.gov/cgi-bin/browse-edgar?action=getcompany&CIK={0}&type=10-K%25&dateb=&owner=include&start=0&count=10&output=atom", cikInfo.CIK);

            string path = string.Format("{0}\\{1}_{2}", downloadReportFolder, cikInfo.Ticker, cikInfo.CIK);
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
                catch (Exception ex)
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
}
