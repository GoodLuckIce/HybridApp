using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using HybridCommon.Enum;
using HybridCommon.Utils;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace HybridCommon.HttpClientHelper
{
    public class DefaultClient
    {
        /// <summary>
        /// 通过GET方式调用API
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="parameters">参数字典</param>
        /// <param name="partnerId">合作者或模块ID</param>
        /// <param name="secretKey">加密参数</param>
        /// <returns></returns>
        public static ResultObj DoGet(string url, IDictionary<string, string> parameters, string partnerId, string secretKey)
        {
            IDictionary<string, string> txtParams;
            if (parameters != null)
            {
                txtParams = new Dictionary<string, string>(parameters);
            }
            else
            {
                txtParams = new Dictionary<string, string>();
            }

            txtParams.Add("PartnerId", partnerId);
            txtParams.Add("Timestamp", DateTime.Now.ToUniversalTime().Ticks.ToString());//时间戳以时间周期数表示
            txtParams.Add("Sign", Cryptography.SignRequest(txtParams, secretKey));

            var urlTemp = Strings.BuildGetUrl(url, txtParams);

            var customResult = new ResultObj
            {
                Url = urlTemp,
                IsPost = false,
                IsError = true,
                Param = txtParams,
                StartExecutionTime = DateTime.Now
            };

            //var appLogModel = new AppLog
            //{
            //    CreateTime = DateTime.Now,
            //    IsError = false,
            //    LogId = Guid.NewGuid().ToString(),
            //    Message = "接口访问日志",
            //    ContentJson = JsonConvert.SerializeObject(customResult)
            //};
            //var db = DbConHelper.NewDbAsyncCon();
            //db.InsertAsync(appLogModel);

            if (!CrossConnectivity.Current.IsConnected)
            {
                customResult.IsError = true;
                customResult.Code = (int)EnumHttpClientCode.Err800;
                customResult.Content = "无法连接网络,请检查网络连接";
                customResult.Message = customResult.Content;
                customResult.ErrorType = EnumErrorType.ValidationError;
                return customResult;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    var jsonResult = client.GetStringAsync(urlTemp).Result;
                    if (jsonResult.Contains(@"""Code"":"))
                    {
                        var model = new { Code = 11, Message = string.Empty, Content = new object(), IsError = false };
                        model = JsonConvert.DeserializeAnonymousType(jsonResult, model);
                        customResult.Code = model.Code;
                        customResult.Message = model.Message;
                        customResult.IsError = model.IsError;
                        customResult.Content = JsonConvert.SerializeObject(model.Content).Trim('"');

                        customResult.EndExecutionTime = DateTime.Now;
                        //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                        //db.UpdateAsync(appLogModel);
                        //customResult.Content = model.Content.ToString();
                        //var jsonStr = JsonConvert.SerializeObject(customResult.Content).Remove(0, 1);
                        //customResult.Content = jsonStr.Remove(jsonStr.Length - 1, 1);
                        return customResult;
                    }
                    else
                    {
                        customResult.Code = (int)EnumHttpClientCode.OK;
                        customResult.IsError = false;
                        customResult.Content = jsonResult.Trim('"');
                        customResult.Message = customResult.Content;

                        customResult.EndExecutionTime = DateTime.Now;
                        //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                        //db.UpdateAsync(appLogModel);
                        return customResult;
                    }
                }
                catch (Exception ex)
                {
                    customResult.Content = "";
                    var ie = ex;
                    while (true)
                    {
                        if (!customResult.Content.Contains(ie.Message))
                        {
                            //customResult.Content += ie.Message;
                            customResult.Content = ie.Message;
                        }
                        if (ie.InnerException != null)
                        {
                            ie = ie.InnerException;
                        }
                        else
                        {
                            break;
                        }
                    }
                    customResult.IsError = true;
                    customResult.Code = (int)EnumHttpClientCode.Err804;
                    customResult.Message = customResult.Content;

                    customResult.EndExecutionTime = DateTime.Now;
                    //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                    //appLogModel.IsError = true;
                    //db.UpdateAsync(appLogModel);
                    return customResult;
                }
            }
        }

        /// <summary>
        /// 通过Post方式调用WebApi,不包含文件上传
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="parameters">表单数据字典</param>
        /// <param name="partnerid">合作者或模块ID</param>
        /// <param name="secretKey">加密参数</param>
        /// <returns></returns>
        public static ResultObj DoPost(string url, IDictionary<string, string> parameters, string partnerid, string secretKey)
        {
            if (parameters == null)
            {
                var customResultTemp = new ResultObj
                {
                    Url = url,
                    IsPost = true,
                    IsError = true,
                    Param = new Dictionary<string, string>(),
                    Code = (int)EnumHttpClientCode.Err802,
                    Message = "POST表单数据值或上传的文件对象同时不存在",
                    Content = "POST表单数据值或上传的文件对象同时不存在"
                };
                return customResultTemp;
            }

            IDictionary<string, string> txtParams = new Dictionary<string, string>(parameters);

            IDictionary<string, string> urlParams = new Dictionary<string, string>();
            urlParams.Add("PartnerId", partnerid);
            urlParams.Add("Timestamp", DateTime.Now.ToUniversalTime().Ticks.ToString());//时间戳以时间周期数表示

            foreach (var item in urlParams)
            {
                txtParams.Add(item.Key, item.Value);
            }

            var sign = Cryptography.SignRequest(txtParams, secretKey);
            urlParams.Add("Sign", sign);

            var urlTemp = Strings.BuildGetUrl(url, urlParams);

            HttpContent result = new FormUrlEncodedContent(parameters);

            var customResult = new ResultObj
            {
                Url = urlTemp,
                IsPost = true,
                IsError = true,
                Param = txtParams,
                StartExecutionTime = DateTime.Now
            };

            //var appLogModel = new AppLog
            //{
            //    CreateTime = DateTime.Now,
            //    IsError = false,
            //    LogId = Guid.NewGuid().ToString(),
            //    Message = "接口访问日志",
            //    ContentJson = JsonConvert.SerializeObject(customResult)
            //};
            //var db = DbConHelper.NewDbAsyncCon();
            //db.InsertAsync(appLogModel);


            if (!CrossConnectivity.Current.IsConnected)
            {
                customResult.IsError = true;
                customResult.Code = (int)EnumHttpClientCode.Err800;
                customResult.Message = "系统提示";
                customResult.Content = "无法连接网络,请检查网络连接";
                customResult.Message = customResult.Content;
                customResult.ErrorType = EnumErrorType.ValidationError;
                return customResult;
            }


            //指定返回XML格式
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            //指定返回JSON
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json")); 
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    var response = client.PostAsync(urlTemp, result);
                    var responseResult = response.Result;
                    if (responseResult.IsSuccessStatusCode)
                    {
                        var responseResultReadAsync = responseResult.Content.ReadAsStringAsync();
                        var jsonResult = responseResultReadAsync.Result;
                        if (jsonResult.Contains(@"""Code"":"))
                        {
                            var model = new { Code = 11, Message = string.Empty, Content = new object(), IsError = true };
                            model = JsonConvert.DeserializeAnonymousType(jsonResult, model);
                            customResult.Code = model.Code;
                            customResult.Message = model.Message;
                            customResult.IsError = model.IsError;
                            customResult.Content = JsonConvert.SerializeObject(model.Content).Trim('"');

                            customResult.EndExecutionTime = DateTime.Now;
                            //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                            //appLogModel.IsError = false;
                            //db.UpdateAsync(appLogModel);
                            return customResult;
                        }
                        else
                        {
                            customResult.Code = (int)responseResult.StatusCode;
                            customResult.Message = responseResult.StatusCode.ToString();
                            customResult.Content = jsonResult.Trim('"');
                            customResult.IsError = false;

                            customResult.EndExecutionTime = DateTime.Now;
                            //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                            //appLogModel.IsError = false;
                            //db.UpdateAsync(appLogModel);
                            return customResult;
                        }
                    }
                    else
                    {
                        customResult.IsError = true;
                        customResult.Code = (int)responseResult.StatusCode;
                        customResult.Message = responseResult.StatusCode.ToString();
                        customResult.Content = responseResult.RequestMessage.ToString();
                        customResult.Message = customResult.Content;

                        customResult.EndExecutionTime = DateTime.Now;
                        //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                        //appLogModel.IsError = true;
                        //db.UpdateAsync(appLogModel);
                        return customResult;
                    }
                }
                catch (Exception ex)
                {
                    customResult.Content = "";
                    var ie = ex;
                    while (true)
                    {
                        if (!customResult.Content.Contains(ie.Message))
                        {
                            //customResult.Content += ie.Message;
                            customResult.Content = ie.Message;
                        }
                        if (ie.InnerException != null)
                        {
                            ie = ie.InnerException;
                        }
                        else
                        {
                            break;
                        }
                    }
                    customResult.IsError = true;
                    customResult.Code = (int)EnumHttpClientCode.Err804;
                    customResult.Message = customResult.Content;

                    customResult.EndExecutionTime = DateTime.Now;
                    //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                    //appLogModel.IsError = true;
                    //db.UpdateAsync(appLogModel);
                    return customResult;
                }
            }
        }

        /// <summary>
        /// 通过Post方式调用WebApi,包含文件上传
        /// </summary>
        /// <param name="url">api url</param>
        /// <param name="parameters">表单数据字典</param>
        /// <param name="files">表单中要上传的文件</param>
        /// <param name="partnerid">合作者或模块ID</param>
        /// <param name="secretKey">加密参数</param>
        /// <returns></returns>
        public static ResultObj DoPost(string url, IDictionary<string, string> parameters, List<FileContent> files, string partnerid, string secretKey)
        {
            if (parameters == null && files.Count == 0)
            {
                var customResultTemp = new ResultObj
                {
                    Url = url,
                    IsPost = true,
                    IsError = true,
                    Param = new Dictionary<string, string>(),
                    Code = (int)EnumHttpClientCode.Err802,
                    Message = "POST表单数据值或上传的文件对象同时不存在",
                    Content = "POST表单数据值或上传的文件对象同时不存在"
                };
                return customResultTemp;
            }

            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            IDictionary<string, string> txtParams = new Dictionary<string, string>(parameters);

            TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks);
            IDictionary<string, string> urlParams = new Dictionary<string, string>();
            urlParams.Add("PartnerId", partnerid);
            urlParams.Add("Timestamp", DateTime.Now.ToUniversalTime().Ticks.ToString());//时间戳以时间周期数表示

            foreach (var item in urlParams)
            {
                txtParams.Add(item.Key, item.Value);
            }

            var sign = Cryptography.SignRequest(txtParams, secretKey);
            urlParams.Add("Sign", sign);

            var urlTemp = Strings.BuildGetUrl(url, urlParams);

            var result = new MultipartFormDataContent();

            var customResult = new ResultObj
            {
                Url = urlTemp,
                IsPost = true,
                IsError = true,
                Param = txtParams,
                StartExecutionTime = DateTime.Now
            };

            //var appLogModel = new AppLog
            //{
            //    CreateTime = DateTime.Now,
            //    IsError = false,
            //    LogId = Guid.NewGuid().ToString(),
            //    Message = "接口访问日志",
            //    ContentJson = JsonConvert.SerializeObject(customResult)
            //};
            //var db = DbConHelper.NewDbAsyncCon();
            //db.InsertAsync(appLogModel);


            if (!CrossConnectivity.Current.IsConnected)
            {
                customResult.IsError = true;
                customResult.Code = (int)EnumHttpClientCode.Err800;
                customResult.Message = "系统提示";
                customResult.Content = "无法连接网络,请检查网络连接";
                customResult.Message = customResult.Content;
                customResult.ErrorType = EnumErrorType.ValidationError;
                return customResult;
            }

            foreach (var item in parameters)
            {
                result.Add(new StringContent(item.Value), item.Key);
            }

            foreach (var file in files)
            {
                //StreamContent fileContent = new StreamContent(file.FileStream);
                //fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                //fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                //fileContent.Headers.ContentDisposition.FileName = file.FileName;
                //fileContent.Headers.ContentDisposition.Name = file.FieldName;
                StreamContent fileContent = new StreamContent(file.FileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                fileContent.Headers.ContentDisposition.FileName = file.FileName;
                fileContent.Headers.ContentDisposition.Name = file.FieldName;
                if (!string.IsNullOrWhiteSpace(file.ContentType))
                {
                    fileContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("ContentType", file.ContentType.Replace("/", "-")));
                }


                result.Add(fileContent);
            }

            //指定返回XML格式
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            //指定返回JSON
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json")); 

            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(3);
                    var response = client.PostAsync(urlTemp, result);
                    var responseResult = response.Result;
                    if (responseResult.IsSuccessStatusCode)
                    {
                        var responseResultReadAsync = responseResult.Content.ReadAsStringAsync();
                        var jsonResult = responseResultReadAsync.Result;
                        if (jsonResult.Contains(@"""Code"":"))
                        {
                            var model = new { Code = 11, Message = string.Empty, Content = new object(), IsError = true };
                            model = JsonConvert.DeserializeAnonymousType(jsonResult, model);
                            customResult.Code = model.Code;
                            customResult.Message = model.Message;
                            customResult.IsError = model.IsError;
                            customResult.Content = JsonConvert.SerializeObject(model.Content).Trim('"');

                            customResult.EndExecutionTime = DateTime.Now;
                            //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                            //appLogModel.IsError = false;
                            //db.UpdateAsync(appLogModel);
                            return customResult;
                        }
                        else
                        {
                            customResult.Code = (int)responseResult.StatusCode;
                            customResult.Message = responseResult.StatusCode.ToString();
                            customResult.IsError = false;
                            customResult.Content = jsonResult.Trim('"');

                            customResult.EndExecutionTime = DateTime.Now;
                            //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                            //appLogModel.IsError = false;
                            //db.UpdateAsync(appLogModel);
                            return customResult;
                        }
                    }
                    else
                    {
                        customResult.IsError = true;
                        customResult.Code = (int)responseResult.StatusCode;
                        customResult.Message = responseResult.StatusCode.ToString();
                        customResult.Content = responseResult.RequestMessage.ToString();
                        customResult.Message = customResult.Content;

                        customResult.EndExecutionTime = DateTime.Now;
                        //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                        //appLogModel.IsError = true;
                        //db.UpdateAsync(appLogModel);
                        return customResult;
                    }
                }
                catch (Exception ex)
                {
                    customResult.Content = "";
                    var ie = ex;
                    while (true)
                    {
                        if (!customResult.Content.Contains(ie.Message))
                        {
                            //customResult.Content += ie.Message;
                            customResult.Content = ie.Message;
                        }
                        if (ie.InnerException != null)
                        {
                            ie = ie.InnerException;
                        }
                        else
                        {
                            break;
                        }
                    }
                    customResult.IsError = true;
                    customResult.Code = (int)EnumHttpClientCode.Err804;
                    customResult.Message = customResult.Content;

                    customResult.EndExecutionTime = DateTime.Now;
                    //appLogModel.ContentJson = JsonConvert.SerializeObject(customResult);
                    //appLogModel.IsError = true;
                    //db.UpdateAsync(appLogModel);
                    return customResult;
                }
            }
        }
    }
}
