using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SparkCore
{
    class Json
    {
        [DataContract]
        public class oauth_token
        {
            [DataMember(Name = "access_token")]
            public string access_token { get; set; }
            [DataMember(Name = "token_type")]
            public string token_type { get; set; }
            [DataMember(Name = "expires_in")]
            public Int64 expires_in { get; set; }
        }

        [DataContract]
        public class access_token
        {
            [DataMember(Name = "token")]
            public string token { get; set; }
            [DataMember(Name = "expires_in")]
            public Int64 expires_in { get; set; }
            [DataMember(Name = "client")]
            public string client { get; set; }
        }

        [DataContract]
        public class delete_token
        {
            [DataMember(Name = "ok")]
            public bool ok { get; set; }
        }

        [DataContract]
        public class function
        {
            [DataMember(Name = "id")]
            public string id { get; set; }
            [DataMember(Name = "name")]
            public string name { get; set; }
            [DataMember(Name = "connected")]
            public bool connected { get; set; }
            [DataMember(Name = "return_value")]
            public Int64 return_value { get; set; }
        }

        [DataContract]
        public class core_info
        {
            [DataMember(Name = "last_app")]
            public string last_app { get; set; }
            [DataMember(Name = "last_heard")]
            public string last_heard { get; set; }
            [DataMember(Name = "connected")]
            public bool connected { get; set; }
            [DataMember(Name = "devideID")]
            public string deviceID { get; set; }
        }

        [DataContract]
        public class variable
        {
            [DataMember(Name = "cmd")]
            public string cmd { get; set; }
            [DataMember(Name = "name")]
            public string name { get; set; }
            [DataMember(Name = "result")]
            public object result { get; set; }
            [DataMember(Name = "coreInfo")]
            public core_info coeInfo { get; set; }
        }
    }
}
