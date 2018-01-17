namespace SECCrawler
{
    public class Filing
    {
        public Filing() { }

        public string AccessionNunber { get; set; }
        public string fileNumber { get; set; }
        public string fileNumberHref { get; set; }
        public string FilingDate { get; set; }
        public string FilingHref { get; set; }
        public string FilingType { get; set; }
        public string DownloadReportPath { get; set; }
        public string DownloadStatus { get; set; }
    }
}