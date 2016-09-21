using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kuaidi100ex
{
    public delegate void SelectCompanyHandler(List<string> companyList);

    public delegate void WriteExpressStatusHandler(List<string> expressInfo);

    public delegate void ErrorResultHandler(string message);

    public class KuaiDiHundred
    {
        private static readonly string COMPANYxQUERYxAPI = "http://www.kuaidi100.com/autonumber/autoComNum?text={0}";
        private static readonly string EXPRESSxQUERYxAPI = "http://www.kuaidi100.com/query?type={0}&postid={1}";

        private string _number = string.Empty;
        private string _companyCode = string.Empty;

        public string CurrentOriginalString { get; private set; }

        public event SelectCompanyHandler OnSelectCompany = null;
        public event WriteExpressStatusHandler OnWriteExpressStatus = null;
        public event ErrorResultHandler OnErrorResult = null;

        public KuaiDiHundred(string number, string companyCode = null)
        {
            _number = number;
            _companyCode = companyCode;
        }

        public static bool Get(string url, out string text)
        {
            var request = WebRequest.Create(url);
            var response = (HttpWebResponse) request.GetResponse();
            var respStream = response.GetResponseStream();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                text = "";
                return false;
            }

            using (StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding("utf-8")))
            {
                text = reader.ReadToEnd();
            }
            return true;
        }

        public JObject GetJObject(string url)
        {
            var text = string.Empty;

            if (Get(url, out text))
            {
                this.CurrentOriginalString = text;
                return (JObject) JsonConvert.DeserializeObject(text);
            }
            else
            {
                this.CurrentOriginalString = "";
                return null;
            }
        }

        private void RaiseError(string message)
        {
            if (this.OnErrorResult == null)
            {
                throw new NotImplementedException("No handler for error result.");
            }
            this.OnErrorResult(message);
        }

        private string GetCompanyCode()
        {
            if (!string.IsNullOrEmpty(_companyCode)) return _companyCode;
            ;

            var url = string.Format(COMPANYxQUERYxAPI, _number);
            var companyCodeList = new List<string>();

            JObject jsonResult = this.GetJObject(url);

            if (jsonResult != null)
            {
                foreach (var companyInfo in jsonResult["auto"])
                {
                    companyCodeList.Add(companyInfo["comCode"].ToString());
                }
            }

            if (companyCodeList.Count == 0)
            {
                this.RaiseError("Cannot get express company information.");
                return "";
            }
            else if (companyCodeList.Count > 1)
            {
                if (this.OnSelectCompany == null)
                {
                    throw new NotImplementedException("No handler for process multi companies.");
                }

                OnSelectCompany(companyCodeList);
                return "";
            }

            return companyCodeList.Single();
        }

        private List<string> GetExpressInfo()
        {
            var url = string.Format(EXPRESSxQUERYxAPI, _companyCode, _number);
            var expressInfo = new List<string>();

            JObject jsonResult = this.GetJObject(url);

            if (jsonResult != null && jsonResult["status"].ToString().Equals("200"))
            {
                foreach (var info in jsonResult["data"])
                {
                    expressInfo.Add(string.Format("{0} {1}", info["time"], info["context"]));
                }
            }
            else
            {
                this.RaiseError(jsonResult["message"].ToString());
                return null;
            }

            return expressInfo;
        }

        public void QueryExpressStatus()
        {
            if (this.OnWriteExpressStatus == null)
            {
                throw new NotImplementedException("No handler for output result.");
            }

            _companyCode = this.GetCompanyCode();
            if (string.IsNullOrEmpty(_companyCode)) return;

            var expressInfo = this.GetExpressInfo();
            if (expressInfo != null)
            {
                this.OnWriteExpressStatus(expressInfo);
            }
        }
    }
}