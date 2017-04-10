using System;
using System.Collections.Generic;
using System.Linq;

namespace HybridCommon.Agreement
{
    public interface IAgreementProvider
    {
        #region Js触发的方法
        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="prarm">跳转页面需要传递的参数</param>
        void JumpPage(PageParam prarm);

        /// <summary>
        /// 返回到指定页面
        /// </summary>
        /// <param name="currentPageId">当前页面PageId</param>
        /// <param name="pageId">需要返回的指定PageId,如果为空则返回上一个页面</param>
        /// <param name="param">需要传递的参数</param>
        void PageBack(string currentPageId, string pageId, string param);

        /// <summary>
        /// 执行JavaScript
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="js"></param>
        void ExecuteJavaScript(string pageId, string js);

        /// <summary>
        /// 页面加载完毕时调用的方法
        /// </summary>
        /// <param name="pageId"></param>
        void PageFinished( string pageId);

        /// <summary>
        /// SetPageId完成
        /// </summary>
        /// <param name="parentPageId"></param>
        /// <param name="pageId"></param>
        void SetPageIdComplete(string parentPageId, string pageId);
        #endregion

        #region App触发的方法

        /// <summary>
        /// 点击返回按钮时触发
        /// </summary>
        /// <param name="isRelative"></param>
        void OnBackBtn(string pageId,bool isRelative);

        /// <summary>
        /// 点击右侧按钮时触发
        /// </summary>
        void OnRightBtn(string pageId);

        /// <summary>
        /// 从其他页面返回过来时触发
        /// </summary>
        void OnResume(string pageId, string prarm);


        /// <summary>
        /// 页面打开时触发
        /// </summary>
        /// <param name="parentPageId"></param>
        /// <param name="pageId"></param>
        /// <param name="prarm">传递参数</param>
        void OnCreate(string parentPageId, string pageId, string prarm);

        #endregion



    }
}
