using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IEXApiHandler
{
    public class HttpClientHandler
    {
        public async Task<string> GetString(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            return response;
        }
        public async Task<string> GetStringEx(string url)
        {
            HttpClient client = new HttpClient();
            var response = await Task.Run(() => client.GetStringAsync(url));
            return response;
        }

        public async Task<JObject> GetJson(string url)
        {
            var json = await GetString(url);
            JObject jobject = JObject.Parse(json);
            return jobject;
        }


        public async Task<JArray> GetJsonArray(string url)
        {
            var json = await GetString(url);
            var array = JArray.Parse(json);
            return array;
        }

    }
}
