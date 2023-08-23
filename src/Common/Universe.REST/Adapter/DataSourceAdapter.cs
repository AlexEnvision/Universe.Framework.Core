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
using System.Net;
using System.Text;
using System.Threading;
using Universe.Helpers.Extensions;
using Universe.REST.Adapter.Base;
using Universe.REST.Models;
using Universe.REST.Models.Base;

namespace Universe.REST.Adapter
{
    /// <summary>
    /// Адаптер источника данных
    /// </summary>
    public class DataSourceAdapter: RestClientAdapter
    {       
        private readonly Uri _baseAddress;

        private const int MaxTryingNum = 5;

        public DataSourceAdapter(string baseAdress)
        {
            IsAnonymous = true;
            _baseAddress = new Uri(baseAdress);
        }

        public DataSourceAdapter(string baseAdress, string authorizationToken)
        {
            AuthType = "Bearer";
            _baseAddress = new Uri(baseAdress);

            ChangeToken(authorizationToken);
        }

        public DataSourceAdapter(string baseAdress, string login, string password)
        {
            AuthType = "Basic";
            GetRequestAuthType = GetRequestAuthType.Link;

            string authorizationToken = login + ":" + password;
            authorizationToken = Convert.ToBase64String(Encoding.Default.GetBytes(authorizationToken));
            _baseAddress = new Uri(baseAdress);

            ChangeToken(authorizationToken);
            AuthAccount(login, password);
        }

        public BaseResponce GetData(string endPointUrl, params Argument[] arguments)
        {
            var fullAddress = StringExtension.CombineUrl(this._baseAddress.AbsolutePath, endPointUrl);
            return this.CreateGetRequest(fullAddress, arguments);
        }

        public string ExchangeData(string requestData)
        {
            var result = CreatePostRequest(requestData, _baseAddress.AbsoluteUri);
            return result;
        }

        public string TryExchangeData(string requestData, int tryingNum = 0)
        {
            try
            {
                var result = ExchangeData(requestData);
                return result;
            }
            catch (WebException)
            {
                if (tryingNum > MaxTryingNum)
                    throw;

                tryingNum++;
                Thread.Sleep(1000 * tryingNum * tryingNum);
                return TryExchangeData(requestData, tryingNum);
            }
            catch (Exception ex)
            {
                if (tryingNum > MaxTryingNum)
                    throw new Exception(ex.Message, ex);

                tryingNum++;
                Thread.Sleep(1000 * tryingNum * tryingNum);
                return TryExchangeData(requestData, tryingNum);
            }
        }
    }
}