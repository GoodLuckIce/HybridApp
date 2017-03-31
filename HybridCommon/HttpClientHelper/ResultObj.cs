using System;
using System.Collections.Generic;
using HybridCommon.Enum;
using Newtonsoft.Json;

namespace HybridCommon.HttpClientHelper
{
    public class ResultObj
    {
        /// <summary>
        /// 开始执行时间
        /// </summary>
        public DateTime StartExecutionTime { get; set; }
        /// <summary>
        /// 结束执行时间
        /// </summary>
        public DateTime EndExecutionTime { get; set; }
        /// <summary>
        /// 执行耗时毫秒
        /// </summary>
        public double ExecutionTiem
        {
            get
            {
                if (StartExecutionTime != DateTime.MinValue && EndExecutionTime != DateTime.MinValue)
                {
                    return EndExecutionTime.Subtract(StartExecutionTime).TotalMilliseconds;
                }
                return 0;
            }
        }


        /// <summary>
        /// 是否是Post访问
        /// </summary>
        public bool IsPost { get; set; }

        /// <summary>
        /// 当前访问Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 当前访问的参数
        /// </summary>
        public IDictionary<string, string> Param { get; set; }

        /// <summary>
        /// 是否错误
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 错误类型
        /// </summary>
        public EnumErrorType ErrorType { get; set; }

        /// <summary>
        /// 消息代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 返回的数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 自身的json,方便查看展示
        /// </summary>
        public string ThisJosn { get; set; }

        /// <summary>
        /// 创建验证错误
        /// </summary>
        /// <param name="content">错误提示</param>
        /// <returns></returns>
        public static ResultObj ValidationError(string content)
        {
            var model = new ResultObj
            {
                Message = content,
                Content = content,
                Code = (int)EnumHttpClientCode.Err803,
                IsError = true,
                ErrorType = EnumErrorType.ValidationError,
            };
            model.ThisJosn = JsonConvert.SerializeObject(model);
            return model;
        }

        public static ResultObj UnKnownError(string content)
        {

            var model = new ResultObj
            {
                Message = content,
                Content = content,
                Code = (int)EnumHttpClientCode.Err801,
                IsError = true,
                ErrorType = EnumErrorType.UnknownError,
            };
            model.ThisJosn = JsonConvert.SerializeObject(model);
            return model;
        }

        /// <summary>
        /// 正常返回
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ResultObj Ok(string content = "true")
        {

            var model = new ResultObj
            {
                Content = content,
                Message = content,
                Code = (int)EnumHttpClientCode.OK,
                IsError = false,
                ErrorType = EnumErrorType.None,
            };
            model.ThisJosn = JsonConvert.SerializeObject(model);
            return model;
        }
    }

    public enum EnumErrorType
    {
        /// <summary>
        /// 正确
        /// </summary>
        None = 0,

        /// <summary>
        /// 验证错误(需要给用户提示的错误)
        /// </summary>
        ValidationError = 1,

        /// <summary>
        /// 未知错误(不能显示给用户的)
        /// </summary>
        UnknownError = 2

    }

}
