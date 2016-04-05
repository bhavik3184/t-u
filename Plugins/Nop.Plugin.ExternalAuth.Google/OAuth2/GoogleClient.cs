using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Nop.Plugin.ExternalAuth.Google.OAuth2
{
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Google", Justification = "Brand name")]
    public class GoogleClient : OAuth2Client
    {
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";

        private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";

        private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";

        private readonly string[] UriRfc3986CharsToEscape = new string[] { "!", "*", "'", "(", ")" };

        private readonly string appId;

        private readonly string appSecret;

        private readonly string[] scope;

        public GoogleClient(string appId, string appSecret)
            : this(appId, appSecret, "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email")
        {
        }
 
       
        public GoogleClient(string appId, string appSecret, params string[] scope)
            : base("google") {
          
            this.appId = appId;
            this.appSecret = appSecret;
            this.scope = scope;
        }

        private string HttpPost(string URI, string Parameters)
        {
            WebRequest req = WebRequest.Create(URI);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            using (var stream = req.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            var res = (HttpWebResponse)req.GetResponse();
            using (var stream = res.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd().Trim();
                }
            }           
        }

        private string NormalizeHexEncoding(string url)
        {
            var chars = url.ToCharArray();
            for (int i = 0; i < chars.Length - 2; i++)
            {
                if (chars[i] == '%')
                {
                    chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
                    chars[i + 2] = char.ToUpperInvariant(chars[i + 2]);
                    i += 2;
                }
            }
            return new string(chars);
        }

        private string EscapeUriDataStringRfc3986(string value)
        {
            StringBuilder builder = new StringBuilder(Uri.EscapeDataString(value));
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                builder.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }
            return builder.ToString();
        }

        private string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            if (!args.Any<KeyValuePair<string, string>>())
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(args.Count<KeyValuePair<string, string>>() * 10);
            foreach (KeyValuePair<string, string> pair in args)
            {
                builder.Append(EscapeUriDataStringRfc3986(pair.Key));
                builder.Append('=');
                builder.Append(EscapeUriDataStringRfc3986(pair.Value));
                builder.Append('&');
            }
            builder.Length--;
            return builder.ToString();
        }

        private void AppendQueryArgs(UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if ((args != null) && (args.Count<KeyValuePair<string, string>>() > 0))
            {
                StringBuilder builder2 = new StringBuilder(50 + (args.Count<KeyValuePair<string, string>>() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    builder2.Append(builder.Query.Substring(1));
                    builder2.Append('&');
                }
                builder2.Append(CreateQueryString(args));
                builder.Query = builder2.ToString();
            }
        }        

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var builder = new UriBuilder(AuthorizationEndpoint);
            var args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", appId);
            args.Add("redirect_uri", NormalizeHexEncoding(returnUrl.AbsoluteUri));
            args.Add("scope", string.Join(" ", this.scope));
            AppendQueryArgs(builder, args);
            return builder.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var userData = new Dictionary<string, string>();
            using (WebClient client = new WebClient())
            {
                using (Stream stream = client.OpenRead(UserInfoEndpoint + "?access_token=" + EscapeUriDataStringRfc3986(accessToken)))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        JObject jObject = JObject.Parse(reader.ReadToEnd());
                        userData.Add("id", (string)jObject["id"]);
                        userData.Add("username", (string)jObject["email"]);
                        userData.Add("name", (string)jObject["given_name"] + " " + (string)jObject["family_name"]);
                    }
                }
            }

            return userData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("code", authorizationCode);
            args.Add("client_id", appId);
            args.Add("client_secret", appSecret);
            args.Add("redirect_uri", NormalizeHexEncoding(returnUrl.AbsoluteUri));
            args.Add("grant_type", "authorization_code");
            string query = "?" + CreateQueryString(args);
            string data = HttpPost(TokenEndpoint, query);
            if (string.IsNullOrEmpty(data))
                return null;
            JObject jObject = JObject.Parse(data);
            return (string)jObject["access_token"];                  
        }
    }
}
