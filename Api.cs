using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SparkCore
{
    class Api
    {
        private const string api = "https://api.spark.io";
        private const string ver = "v1";
        private const string creds = "spark:spark";

        public SparkCore.Json.oauth_token Authenticate(string id, string pw)
        {
            System.Collections.Specialized.NameValueCollection vals = new System.Collections.Specialized.NameValueCollection();
            string temp_url = api + "/oauth/token";
            vals.Add("grant_type", "password");
            vals.Add("username", id);
            vals.Add("password", pw);
            string json = PostRequest(temp_url, vals);

            if (json.Substring(0, 5) == "ERROR") { throw new Exception(json); }

            MemoryStream mem = new MemoryStream(Encoding.ASCII.GetBytes(json));
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SparkCore.Json.oauth_token));
            object objResponse = jsonSerializer.ReadObject(mem);
            SparkCore.Json.oauth_token jsonResponse = objResponse as SparkCore.Json.oauth_token;
            return jsonResponse;
        }

        public SparkCore.Json.function CallFunction(string did, string f, string at, string args)
        {
            System.Collections.Specialized.NameValueCollection vals = new System.Collections.Specialized.NameValueCollection();
            string temp_url = api + "/" + ver + "/devices/" + did + "/" + f;
            vals.Add("access_token", at);
            vals.Add("args", args);
            string json = PostRequest(temp_url, vals);

            if (json.Substring(0, 5) == "ERROR") { throw new Exception(json); }

            MemoryStream mem = new MemoryStream(Encoding.ASCII.GetBytes(json));
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SparkCore.Json.function));
            object objResponse = jsonSerializer.ReadObject(mem);
            SparkCore.Json.function jsonResponse = objResponse as SparkCore.Json.function;
            return jsonResponse;            
        }

        private string PostRequest(string url, System.Collections.Specialized.NameValueCollection data)
        {
            WebClient wc = new WebClient();

            // Only need this when you get an access token
            if (url.Contains("oauth")) { wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(creds)); }

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                byte[] retval = wc.UploadValues(url, "POST", data);
                return Encoding.ASCII.GetString(retval);
            }
            catch (Exception e)
            {
                string error = "ERROR: " + e.Message;
                return error;
            }
        }

        // Constructor
        public Api() { }
    }
}
