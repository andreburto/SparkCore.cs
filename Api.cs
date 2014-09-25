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

        // Use your Spark ID and password to get an access token
        public SparkCore.Json.oauth_token Authenticate(string id, string pw)
        {
            System.Collections.Specialized.NameValueCollection vals = new System.Collections.Specialized.NameValueCollection();
            string temp_url = api + "/oauth/token";
            vals.Add("grant_type", "password");
            vals.Add("username", id);
            vals.Add("password", pw);
            string json = PostRequest(temp_url, vals);

            if (json.Substring(0, 5) == "ERROR") { throw new Exception(json); }

            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SparkCore.Json.oauth_token));
            object objResponse = jsonSerializer.ReadObject(mem);
            SparkCore.Json.oauth_token jsonResponse = objResponse as SparkCore.Json.oauth_token;
            return jsonResponse;
        }

        // Call a defined function in the firmware
        public SparkCore.Json.function CallFunction(string did, string f, string at, string args)
        {
            System.Collections.Specialized.NameValueCollection vals = new System.Collections.Specialized.NameValueCollection();
            string temp_url = api + "/" + ver + "/devices/" + did + "/" + f;
            vals.Add("access_token", at);
            vals.Add("args", args);
            string json = PostRequest(temp_url, vals);

            if (json.Substring(0, 5) == "ERROR") { throw new Exception(json); }

            MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SparkCore.Json.function));
            object objResponse = jsonSerializer.ReadObject(mem);
            SparkCore.Json.function jsonResponse = objResponse as SparkCore.Json.function;
            return jsonResponse;            
        }

        // Get a variable
        public SparkCore.Json.variable GetVariable(string did, string at, string varname)
        {
            System.Collections.Specialized.NameValueCollection vals = new System.Collections.Specialized.NameValueCollection();
            string temp_url = api + "/" + ver + "/devices/" + did + "/" + varname + "?access_token=" + at;

            try
            {
                WebRequest wr = WebRequest.Create(temp_url);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebResponse res = (HttpWebResponse)wr.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) { throw new Exception("HTTP error"); }

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SparkCore.Json.variable));

                object objResponse = jsonSerializer.ReadObject(res.GetResponseStream());
                SparkCore.Json.variable jsonResponse = objResponse as SparkCore.Json.variable;
                return jsonResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Display access tokens
        public List<SparkCore.Json.access_token> ListTokens(string id, string pw)
        {
            string temp_url = api + "/" + ver + "/access_tokens";

            try
            {
                WebRequest wr = WebRequest.Create(temp_url);
                wr.Headers.Add("Authorization", "Basic " + encodeCreds(id, pw));
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebResponse res = (HttpWebResponse)wr.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) { throw new Exception("HTTP error"); }

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<SparkCore.Json.access_token>));
                
                object objResponse = jsonSerializer.ReadObject(res.GetResponseStream());
                List<SparkCore.Json.access_token> jsonResponse = objResponse as List<SparkCore.Json.access_token>;
                return jsonResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Delete an access token
        // STILL TIMES OUT -- needs work
        public SparkCore.Json.delete_token DeleteToken(string access_token, string id, string pw)
        {
            string temp_url = api + "/" + ver + "/access_tokens/" + access_token;

            try
            {
                WebRequest wr = WebRequest.Create(temp_url);
                wr.Headers.Add("Authorization", "Basic " + encodeCreds(id, pw));
                wr.Method = "DELETE";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                HttpWebResponse res = (HttpWebResponse)wr.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK) { throw new Exception("HTTP error"); }

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SparkCore.Json.delete_token));
                object objResponse = jsonSerializer.ReadObject(res.GetResponseStream());
                SparkCore.Json.delete_token jsonResponse = objResponse as SparkCore.Json.delete_token;
                return jsonResponse;            
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Use this for calls that require POST
        private string PostRequest(string url, System.Collections.Specialized.NameValueCollection data)
        {
            WebClient wc = new WebClient();

            // Only need this when you get an access token
            if (url.Contains("oauth")) { wc.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(creds)); }

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

        private string encodeCreds(string id, string pw)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(id + ":" + pw));
        }

        // Constructor
        public Api() { }
    }
}
