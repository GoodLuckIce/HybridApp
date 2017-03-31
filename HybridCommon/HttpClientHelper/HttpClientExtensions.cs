using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace HybridCommon.HttpClientHelper
{
    public class HttpClientExtensions
    {
        /// <summary>
        /// Host地址
        /// </summary>
        private string BaseAddress { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        private string Version { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        private string ModuleId { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        private string SecretKey { get; set; }

        /// <summary>
        /// 附近参数可以为null
        /// </summary>
        private IDictionary<string, string> AdditionalParm { get; set; }


        public HttpClientExtensions(string baseAddress, string version, string moduleId, string secretKey, IDictionary<string, string> additionalParm = null)
        {
            BaseAddress = baseAddress;
            Version = version;
            ModuleId = moduleId;
            SecretKey = secretKey;
            AdditionalParm = additionalParm;
        }


        public ResultObj DoGet(string apiUrl, object o)
        {
            var result = DoToMap(apiUrl, o, false);
            return result;
            //return CheckException(result);
        }

        public ResultObj DoGet(string apiUrl, IDictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            if (AdditionalParm != null)
            {
                //附加参数
                foreach (var item in AdditionalParm)
                {
                    if (!parameters.Keys.Select(p => p.ToLower()).Contains(item.Key.ToLower()))
                    {
                        parameters.Add(item);
                    }
                }
            }
            var result = DefaultClient.DoGet(MapUrl(apiUrl), parameters, ModuleId, SecretKey);
            return result;
        }

        public ResultObj DoPost(string apiUrl, object o)
        {
            var result = DoToMap(apiUrl, o, true);
            return result;
        }

        public ResultObj DoPost(string apiUrl, IDictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            if (AdditionalParm != null)
            {
                //附加参数
                foreach (var item in AdditionalParm)
                {
                    if (!parameters.Keys.Select(p => p.ToLower()).Contains(item.Key.ToLower()))
                    {
                        parameters.Add(item);
                    }
                }
            }
            var result = DefaultClient.DoPost(MapUrl(apiUrl), parameters, ModuleId, SecretKey);
            return result;
        }

        protected ResultObj DoPostFile(string apiUrl, List<FileContent> files)
        {
            var result = DefaultClient.DoPost(MapUrl(apiUrl), null, files, ModuleId, SecretKey);
            return result;
        }

        private ResultObj DoToMap(string apiUrl, object o, bool isPost)
        {
            var parameters = new Dictionary<string, string>();
            var files = new List<FileContent>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                var key = p.Name;

                //获取JsonPropertyAttribute描述信息
                var jsonProperty = (JsonPropertyAttribute[])p.GetCustomAttributes(typeof(JsonPropertyAttribute), true);
                if (jsonProperty.Any())
                {
                    key = jsonProperty[0].PropertyName;
                }

                MethodInfo mi = p.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    var valueTemp = mi.Invoke(o, new string[] { });
                    if (valueTemp != null)
                    {
                        if (valueTemp.GetType().FullName.Contains("Enum"))
                        {
                            var value = ((int)valueTemp).ToString();
                            parameters.Add(key, value);
                        }
                        else if (valueTemp.GetType() == typeof(List<string>))
                        {
                            var value = "";
                            foreach (var item in (List<string>)valueTemp)
                            {
                                value += item + ",";
                            }
                            parameters.Add(key, value.Remove(value.IndexOf(",")));

                        }
                        else if (valueTemp.GetType() == typeof(List<int>))
                        {
                            var value = "";
                            foreach (var item in (List<int>)valueTemp)
                            {
                                value += item + ",";
                            }
                            parameters.Add(key, value.Remove(value.IndexOf(",")));
                        }
                        else if (valueTemp.GetType() == typeof(List<long>))
                        {
                            var value = "";
                            foreach (var item in (List<long>)valueTemp)
                            {
                                value += item + ",";
                            }
                            parameters.Add(key, value.Remove(value.IndexOf(",")));
                        }
                        else if (valueTemp.GetType() == typeof(FileContent))
                        {
                            files.Add((FileContent)valueTemp);
                        }
                        else if (valueTemp.GetType() == typeof(List<FileContent>))
                        {
                            foreach (var item in (List<FileContent>)valueTemp)
                            {
                                files.Add(item);
                            }
                        }
                        else
                        {
                            var value = valueTemp.ToString();
                            parameters.Add(key, value);
                        }

                    }
                    else
                    {
                        parameters.Add(key, "");
                    }
                }
            }

            if (AdditionalParm != null)
            {
                //附加参数
                foreach (var item in AdditionalParm)
                {
                    if (!parameters.Keys.Select(p => p.ToLower()).Contains(item.Key.ToLower()))
                    {
                        parameters.Add(item.Key, item.Value);
                    }
                }
            }

            if (isPost)
            {
                if (files.Count > 0)
                {
                    return DefaultClient.DoPost(MapUrl(apiUrl), parameters, files, ModuleId, SecretKey);
                }
                else
                {
                    return DefaultClient.DoPost(MapUrl(apiUrl), parameters, ModuleId, SecretKey);
                }
            }
            else

            {
                return DefaultClient.DoGet(MapUrl(apiUrl), parameters, ModuleId, SecretKey);
            }
        }

        private string MapUrl(string apiUrl)
        {
            apiUrl = BaseAddress + "/" + Version + "/" + apiUrl;
            apiUrl = apiUrl.Replace("://", "$").Replace("//", "/").Replace("//", "/").Replace("//", "/").Replace("//", "/").Replace("$", "://");
            return apiUrl;
        }
        
    }
}
