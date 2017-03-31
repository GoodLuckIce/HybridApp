using HybridCommon.Utils;

namespace HybridCommon.Enum
{
    /// <summary>
    /// EnumHttpClientCode结果标示码
    /// </summary>
    public enum EnumHttpClientCode
    {

        /// <summary>
        /// 手机网络不给力
        /// </summary>
        [EnumDescription("手机网络不给力")]
        Err800 = 800,

        /// <summary>
        /// 意料之外的异常
        /// </summary>
        [EnumDescription("意料之外的异常")]
        Err801 = 801,

        /// <summary>
        /// 提交数据异常_POST表单数据值或上传的文件对象同时不存在
        /// </summary>
        [EnumDescription("提交数据异常_POST表单数据值或上传的文件对象同时不存在")]
        Err802 = 802,

        /// <summary>
        /// 业务验证错误
        /// </summary>
        [EnumDescription("业务验证错误")]
        Err803 = 803,

        /// <summary>
        /// 接口意料之外的异常
        /// </summary>
        [EnumDescription("接口意料之外的异常")]
        Err804 = 804,

        Continue = 100,
        SwitchingProtocols = 101,
        /// <summary>
        /// 正常
        /// </summary>
        [EnumDescription("正常")]
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        Ambiguous = 300,
        MultipleChoices = 300,
        Moved = 301,
        MovedPermanently = 301,
        Found = 302,
        Redirect = 302,
        RedirectMethod = 303,
        SeeOther = 303,
        NotModified = 304,
        UseProxy = 305,
        Unused = 306,
        RedirectKeepVerb = 307,
        TemporaryRedirect = 307,
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestUriTooLong = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        UpgradeRequired = 426,
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
    }
}
