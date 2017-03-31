using System;
using Newtonsoft.Json;

namespace HybridCommon.Agreement
{
    /// <summary>
    /// 开启新页面参数
    /// </summary>
    public class PageParam
    {
        /// <summary>
        /// 是否是亲戚
        /// 如果是亲戚则和父窗口使用同一个WebView
        /// </summary>
        public bool IsRelative { get; set; }
        /// <summary>
        /// 是否一隐藏导航条
        /// </summary>
        public bool IsHideNavBar { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 页面跳转需要传输的额外参数.
        /// </summary>
        [JsonProperty("Param")]
        public string OpenParam { get; set; }

        /// <summary>
        /// 页面返回需要传递的参数
        /// </summary>
        public string BackParam { get; set; }

        /// <summary>
        /// 父页面Id,从那个页面启动跳转.那个页面的pageId就是这个参数
        /// </summary>
        public string ParentPageId { get; set; }

        /// <summary>
        /// 页面Id
        /// </summary>
        public string PageId { get; set; }

        /// <summary>
        /// 页面Url
        /// </summary>
        public string PageUrl { get; set; }
        public string ParentPageUrl { get; set; }
        /// <summary>
        /// 是否允许返回
        /// 决定是否有返回按钮显示
        /// </summary>
        public bool AllowBack { get; set; }

        /// <summary>
        /// 返回页面参数
        /// </summary>
        public BackPagepParam BackPagepParam { get; set; }

        /// <summary>
        /// 允许使用导航条右侧按钮
        /// </summary>
        public bool AllowRightBtn { get; set; }

        /// <summary>
        /// 右侧按钮参数
        /// </summary>
        public RightBtnParam RightBtnParam { get; set; }
        
        public string WebViewKey { get; set; }
        public string ParentWebViewKey { get; set; }

        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool IsLoadFinished { get; set; }
    }

    /// <summary>
    /// 返回页面参数
    /// </summary>
    public class BackPagepParam
    {
        /// <summary>
        /// 返回页面时触发调用
        /// </summary>
        public Action<object, EventArgs> CallBackAction { get; set; }

    }

    /// <summary>
    /// 右侧按钮参数
    /// </summary>
    public class RightBtnParam
    {
        /// <summary>
        /// 按钮类型
        /// {1:文字}
        /// </summary>
        public int BtnType { get; set; }

        /// <summary>
        /// 按钮Title
        /// </summary>
        public string BtnTitle { get; set; }

        /// <summary>
        /// 点击按钮时触发调用
        /// </summary>
        public Action<object, EventArgs> CallBackAction { get; set; }

    }
}
