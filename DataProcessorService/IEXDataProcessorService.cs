using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using IEXApiWrapper.IEXClient;
using IEXApiWrapper.IEXData.Stock;
using System.Net;

namespace DataProcessorService
{
    public partial class IEXDataProcessorService : ServiceBase
    {
        Timer iexProcessDataTimer = new Timer();
        BatchReportResponse reportResponse = new BatchReportResponse();

        public IEXDataProcessorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            iexProcessDataTimer.Elapsed += new ElapsedEventHandler(OnElapsedTimer);
            iexProcessDataTimer.Interval = 1000;
            iexProcessDataTimer.Start();
        }

        private void OnElapsedTimer(object sender, ElapsedEventArgs e)
        {
            //get data from iex server
            var data = reportResponse.GetBatchReport(new string[] { "ipar", "ge", "docu" }, new string[] { "quote", "news", "chart" }, ChartOption._1m, 1);

        }

        protected override void OnStop()
        {
            iexProcessDataTimer.Stop();
            iexProcessDataTimer.Dispose();
        }
    }
}
