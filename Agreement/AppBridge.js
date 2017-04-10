import router from '../router/index'
import store from '../store/index'
import cookieHelp from './Cookie'

var CallAppBridgeData = undefined
var CallAppBridgeInterval = undefined

function GetPlatformType() {

  /*window.location = url;*/
  var ua = window.navigator.userAgent, os = {};
  var android = ua.match(/(Android);?[\s\/]+([\d.]+)?/),
    /*osx = !!ua.match(/\(Macintosh\; Intel /),*/
    ipad = ua.match(/(iPad).*OS\s([\d_]+)/),
    ipod = ua.match(/(iPod)(.*OS\s([\d_]+))?/),
    iphone = !ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/);
  if (android) {
    os.android = true, os.version = android[2];
  }
  if (iphone && !ipod) {
    os.ios = os.iphone = true, os.version = iphone[2].replace(/_/g, '.');
  }
  if (ipad) {
    os.ios = os.ipad = true, os.version = ipad[2].replace(/_/g, '.');
  }
  if (ipod) {
    os.ios = os.ipod = true, os.version = ipod[3] ? ipod[3].replace(/_/g, '.') : null;
  }

  if(os.ios)
  {
    return AppDate.EnumPlatformType.iOS;
  }
  if(os.android)
  {
    return AppDate.EnumPlatformType.Android;
  }

  return AppDate.EnumPlatformType.WebBrowser;
}


//协议桥.负责Js和App通讯
function CallAppBridge(url) {
  if (GetPlatformType() == AppDate.EnumPlatformType.iOS) {
    window.location = url;
  } else {
    var ifr = document.createElement('iframe');
    ifr.setAttribute('style', 'display: none;');
    ifr.setAttribute('src', url);
    document.querySelector('body').appendChild(ifr);

    setTimeout(function () {
      ifr.parentNode.removeChild(ifr);
    }, 1000);
  }
}
//拼接回掉url
function route_getHybridUrl(address, callback) {
  if (callback) {
    address += '&cb=' + callback
  }
  return address;
}
//callBack对象转方法名.为url提供
function routeMadeCallBack(callback, q) {
  /*生成唯一执行函数，执行后销毁*/
  var time = (new Date().getTime());
  var t = 'hybrid_' + time + (q || '');
  var tmpFn;

  /*处理有回调的情况*/
  if (callback) {
    tmpFn = callback;
    callback = t;
    window.H5Api[t] = function (data) {
      tmpFn(data);
      delete window.Hybrid[t];
    }
  }
  return callback;
}
//异步协议桥.负责Js和App通讯,异步桥解决,同时触发多个协议的时候.导致先触发的协议调用不成功.
function CallAppBridgeAsyn(url) {
  if (CallAppBridgeData == undefined) {
    CallAppBridgeData = [];
  }
  CallAppBridgeData.push(url)

  if (CallAppBridgeInterval == undefined) {
    CallAppBridgeInterval = setInterval(function () {

      var tempUrl = CallAppBridgeData.shift();
      if (tempUrl != undefined) {
        CallAppBridge(tempUrl);
      }
    }, 100);
  }
}
//请求app母体
function RequestHybrid(address, callback) {
  if (AppBridge.PlatformType != AppDate.EnumPlatformType.WebBrowser) {
    callback = routeMadeCallBack(callback);
    CallAppBridgeAsyn(route_getHybridUrl(address, callback));
  }
}

//基础数据,模型,等
var AppDate = {
  Scheme: "HybridAgreement",
  //平台类型
  EnumPlatformType: {
    Android: "Android",
    iOS: "iOS",
    WindowsPhone: "WindowsPhone",
    WebBrowser: "WebBrowser",
  },
  OpenPageParam: {
    IsRelative: false,
    IsHideNavBar: true,
    PageName: "页面名称",
    Param: "",
    //这个PageId必须是唯一值绝对不能重复
    PageId: "",
    PageUrl: "",
    AllowBack: false,
    AllowRightBtn: false,
    RightBtnParam: {
      BtnType: 1,
      BtnTitle: ""
    }
  }
}

//协议规则
var AppBridge = {
  //父PageId
  ParentPageId: "",
  //当前页面Id
  PageId: "",
  //存储当前运行的平台类型 : EnumPlatform
  PlatformType: GetPlatformType(),
  //路由对象Router
  Router: router,
  //状态树对象Store
  Store:store,
  //跳转到新页面
  JumpPage: function (openPageParam) {
    //console.log("111");
    if (AppBridge.PlatformType == AppDate.EnumPlatformType.WebBrowser) {
      var tempUrl =openPageParam.PageUrl.toString();
      var Param=openPageParam.Param.toString();

      //判断URL末尾是否有斜杠
      if(tempUrl.substring(tempUrl.length-1)=="/") {
        tempUrl = tempUrl.substring(0, tempUrl.length-1);
      }
      //判断是否有参数
      if(openPageParam.Param!=undefined&&openPageParam.Param!="") {
        //判断参数前是否有斜杠
        if(Param.indexOf("/")==0) {
          Param = Param.substring(1);
        }
      }
      //拼接URL字符串
      tempUrl = tempUrl +"/"+ Param;
      //console.log(tempUrl);
      router.push(tempUrl)
    }
    else {
      openPageParam.ParentPageId = AppBridge.PageId;
      if(openPageParam.PageId == "")
      {
        openPageParam.PageId = openPageParam.PageUrl;
      }
      var strJson = JSON.stringify(openPageParam);
      RequestHybrid(`${AppDate.Scheme}://JumpPage?p=${strJson}`);
    }
  },
  //返回页面方法
  //参数-> pageId -> striing -> 需要返回到的页面Id,如果为空则返回上一个页面
  //参数-> param -> striing ->  给即将返回的页面传递是参数,多个参数转换为json传递
  PageBack: function (pageId, param) {
    if (AppBridge.PlatformType == AppDate.EnumPlatformType.WebBrowser) {
      router.go(-1)
    }
    else {
      var currentPageId = AppBridge.PageId;
      RequestHybrid(`${AppDate.Scheme}://PageBack?currentPageId=${currentPageId}&pageId=${pageId}&p=${param}`);
    }
  },
  SaveUserContext: function (userId, userName,avatar) {

    store.state.User.UserContext = {
      UserId: userId,
      UserName: userName,
      Avatar:avatar
    };

    if (AppBridge.PlatformType == AppDate.EnumPlatformType.WebBrowser) {
      //todo:写coike
      cookieHelp("UserId",userId,{path: '/'});
      cookieHelp("UserName",userName,{path: '/'});
      cookieHelp("Avatar",avatar,{path: '/'});
      console.log(userName);
    }
    else {
      var parentPageId = AppBridge.ParentPageId;
      RequestHybrid(`${AppDate.Scheme}://SaveUserContext?userId=${userId}&userName=${userName}&avatar=${avatar}`);
    }
  },
  //返回按钮回掉
  //当页面点击导航栏返回或者按返回按钮时触发
  //页面可以重写覆盖此方法
  //参数-> IsRelative -> bool -> 当前页面是否是近亲(和父页面公用一个浏览器)
  OnBackBtn: function (pageId, IsRelative) {
    console.log("触发OnBacktn");
    AppBridge.PageBack();
  },

  //导航栏右侧按钮回掉
  //当页点击导航栏右侧按钮时触发
  //页面可以重写覆盖此方法
  OnRightBtn: function (pageId) {
    console.log("触发OnRightBtn");
  },
  //当从其他页面返回到此页面时触发
  OnResume: function (pageId, param) {
    console.log("触发OnResume");
  },
  //页面创建时触发,传递上个页面传过来的参数
  //页面可以重写覆盖此方法
  OnCreate: function (parentPageId, pageId, param) {
    console.log("触发OnCreate", parentPageId, pageId, param);
  },
  //设置PageId
  SetPageId: function (parentPageId, pageId) {
    console.log("设置PageId",pageId);
    AppBridge.ParentPageId = parentPageId;
    AppBridge.PageId = pageId;
    RequestHybrid(`${AppDate.Scheme}://SetPageIdComplete?parentPageId=${parentPageId}&pageId=${pageId}`);
  },
  //设置设备信息
  SetDeviceInfo:function (paramJson) {
    var paramObj = JSON.parse(paramJson);
    store.state.DeviceInfo = paramObj;
  },
  //设置用户数据
  SetUserContext: function (paramJson) {
    if (AppBridge.PlatformType == AppDate.EnumPlatformType.WebBrowser) {

     var UserId= cookieHelp("UserId");
     var UserName= cookieHelp("UserName");
      var Avatar= cookieHelp("Avatar");
      store.state.User.UserContext = {
        UserId: UserId,
        UserName: UserName,
        Avatar : Avatar
      };

    }else {
      var paramObj = JSON.parse(paramJson);
      store.state.User.UserContext = {
        UserId: paramObj.UserId,
        UserName: paramObj.UserName,
        Avatar: paramObj.Avatar
      };
    }
  },

}


window.AppDate = AppDate;
window.AppBridge = AppBridge;

export {AppBridge as default, AppDate, RequestHybrid}
