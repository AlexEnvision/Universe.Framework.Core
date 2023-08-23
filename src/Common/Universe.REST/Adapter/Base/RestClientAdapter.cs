//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Universe.Helpers.Extensions;
using Universe.REST.Models;
using Universe.REST.Models.Base;

namespace Universe.REST.Adapter.Base
{
    public class RestClientAdapter
    {
        private string _authToken;

        private string _login;

        private string _password;

        private string _certName;

        private string _certPath;

        protected bool IsAnonymous { get; set; }

        protected string AuthType { get; set; }

        protected GetRequestAuthType GetRequestAuthType { get; set; }

        public bool IsTextMode { get; set; }

        public bool AllowAutoRedirect { get; set; }

        protected RestClientAdapter()
        {
            if (string.IsNullOrEmpty(AuthType))
                AuthType = "Bearer";

            IsTextMode = false;
            AllowAutoRedirect = false;
        }

        protected void ChangeToken(string authToken)
        {
            _authToken = authToken;
        }

        protected void AuthAccount(string login, string password)
        {
            _login = login;
            _password = password;
        }

        public void SetCertificate(string certName, string certPath)
        {
            _certName = certName;
            _certPath = certPath;
        }

        public string CreatePostRequest(string content, string endPointUrl)
        {
            return IsAnonymous ? CreateAnonymousPostRequestInt(content, endPointUrl) : CreatePostRequestInt(content, endPointUrl);
        }

        private string CreateAnonymousPostRequestInt(string content, string endPointUrl)
        {
            var swServiceConnecting = Stopwatch.StartNew();
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endPointUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = IsTextMode ? "text/plain" : "application/json";
                httpWebRequest.Accept = IsTextMode ? "*/*" : "application/json";
                httpWebRequest.Timeout = 300000;

                httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;

                var contentInner = new StringContent(
                    content,
                    Encoding.UTF8,
                    IsTextMode ? "text/plain" : "application/json");

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var str = contentInner.ReadAsStringAsync().Result;
                    streamWriter.Write(str);
                    streamWriter.Close();
                }
                using (StreamReader streamReader = new StreamReader((httpWebRequest.GetResponse() as HttpWebResponse).GetResponseStream()))
                {
                    string end = streamReader.ReadToEnd();
                    swServiceConnecting.Stop();
                    return end;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
            finally
            {
                swServiceConnecting.Stop();
            }
        }

        private string CreatePostRequestInt(string content, string endPointUrl)
        {
            var swServiceConnecting = Stopwatch.StartNew();
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endPointUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = IsTextMode ? "text/plain" : "application/json";
                httpWebRequest.Accept = IsTextMode ? "*/*" : "application/json";
                httpWebRequest.Timeout = 300000;

                httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;

                httpWebRequest.Credentials = new NetworkCredential(_login, _password);

                // Обход ошибки сертификата
                ServicePointManager.ServerCertificateValidationCallback += ((sender, certificate, chain, sslPolicyErrors) => true);

                var contentInner = new StringContent(
                    content,
                    Encoding.UTF8,
                    IsTextMode ? "text/plain" : "application/json");

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var str = contentInner.ReadAsStringAsync().Result;
                    streamWriter.Write(str);
                    streamWriter.Close();
                }
                using (StreamReader streamReader = new StreamReader((httpWebRequest.GetResponse() as HttpWebResponse).GetResponseStream()))
                {
                    string end = streamReader.ReadToEnd();
                    swServiceConnecting.Stop();
                    return end;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
            finally
            {
                swServiceConnecting.Stop();
            }
        }

        public BaseResponce CreateGetRequest(string endPointUrl, params Argument[] arguments)
        {
            return IsAnonymous 
                ? CreateAnonymousGetRequestInt(endPointUrl, arguments)
                : CreateGetRequestInt(endPointUrl, arguments);
        }

        private BaseResponce CreateAnonymousGetRequestInt(string endPointUrl, params Argument[] arguments)
        {
            var reqUrl = endPointUrl;
            if (arguments.Length > 0)
            {
                var sb = new StringBuilder();
                sb.Append(reqUrl);

                var argumentCol = arguments.ToList();
                var fstArg = argumentCol.First();

                sb.Append($"?{fstArg.ArgumentName}={fstArg.ArgumentValue}");

                // создаём объект HttpWebRequest через статический метод Create класса WebRequest, явно приводим результат к HttpWebRequest. 
                //В параметрах указываем страницу, которая указана в API
                if (arguments.Length > 1)
                {
                    for (var index = 1; index < arguments.Length; index++)
                    {
                        var argument = arguments[index];
                        sb.Append($"&{argument.ArgumentName}={argument.ArgumentValue}");
                    }
                }

                reqUrl = sb.ToString();
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqUrl);

            request.AllowAutoRedirect = AllowAutoRedirect;

            // получаем ответ сервера
            var response = (HttpWebResponse)request.GetResponse();

            // и полностью считываем его в строку
            string result = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            string location = response.Headers["Location"]?.F() ?? string.Empty;

            return new BaseResponce
            {
                ServiceAnswer = result,
                IsSuccessful = true,
                Location = location
            };
        }

        private BaseResponce CreateGetRequestInt(string endPointUrl, params Argument[] arguments)
        {
            switch (GetRequestAuthType)
            {
                case GetRequestAuthType.Link: return CreateGetRequestLinkAuth(endPointUrl, arguments);
                case GetRequestAuthType.Header: return CreateGetRequestHeaderAuth(endPointUrl, arguments);

                default:
                    return CreateGetRequestHeaderAuth(endPointUrl, arguments);
            }
        }

        private BaseResponce CreateGetRequestLinkAuth(string endPointUrl, Argument[] arguments)
        {
            // создаём объект HttpWebRequest через статический метод Create класса WebRequest, явно приводим результат к HttpWebRequest. 
            //В параметрах указываем страницу, которая указана в API
            var reqUrl = $@"{endPointUrl}?api_key={_authToken}";
            var sb = new StringBuilder();
            sb.Append(reqUrl);
            foreach (var argument in arguments)
            {
                sb.Append($"&{argument.ArgumentName}={argument.ArgumentValue}");
            }

            reqUrl = sb.ToString();

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(reqUrl);

            request.AllowAutoRedirect = AllowAutoRedirect;

            // получаем ответ сервера
            var response = (HttpWebResponse) request.GetResponse();

            // и полностью считываем его в строку
            string result = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            string location = response.Headers["Location"]?.F() ?? string.Empty;

            return new BaseResponce
            {
                ServiceAnswer = result,
                IsSuccessful = true,
                Location = location
            };
        }

        private BaseResponce CreateGetRequestHeaderAuth(string endPointUrl, params Argument[] arguments)
        {
            // создаём объект HttpWebRequest через статический метод Create класса WebRequest, явно приводим результат к HttpWebRequest. 
            //В параметрах указываем страницу, которая указана в API
            var reqUrl = $@"{endPointUrl}";
            var sb = new StringBuilder();
            sb.Append(reqUrl);
            foreach (var argument in arguments)
            {
                sb.Append($"&{argument.ArgumentName}={argument.ArgumentValue}");
            }
            reqUrl = sb.ToString();

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(reqUrl);
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers["Authorization"] = $"{this.AuthType} {this._authToken}";

            httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;

            // получаем ответ сервера
            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            // и полностью считываем его в строку
            string result = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            string location = response.Headers["Location"]?.F() ?? string.Empty;

            return new BaseResponce
            {
                ServiceAnswer = result,
                IsSuccessful = true,
                Location = location
            };
        }

        /// <summary>
        ///     Callback used to validate the certificate in an SSL conversation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        private bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = cert.Subject.Contains(_certName);
            return result;
        }
    }
}