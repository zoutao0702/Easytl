using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Easytl;
using Easytl.WebHelper;

namespace Easytl.WebControllers
{
    /// <summary>
    /// 自定义控制器基类
    /// </summary>
    /// <typeparam name="Model_T">登录用户类类型</typeparam>
    public abstract class BaseController<Model_T> : Controller
    {
        #region 自定义字段

        #endregion

        #region 属性

        /// <summary>
        /// 弹出消息类型
        /// </summary>
        public enum eAlertType
        {
            /// <summary>
            /// 信息
            /// </summary>
            Info,
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 提醒
            /// </summary>
            Warning,
            /// <summary>
            /// 错误
            /// </summary>
            Danger
        }

        eAlertType? _alerttype = eAlertType.Info;
        /// <summary>
        /// 获取或设置弹出消息类型
        /// </summary>
        public eAlertType? AlertType
        {
            get { return _alerttype; }
            set { _alerttype = value; TempData["AlertJsonStr"] = AlertJsonStr; }
        }

        string _alertmsg = string.Empty;
        /// <summary>
        /// 获取或设置弹出消息内容
        /// </summary>
        public string AlertMsg
        {
            get { return _alertmsg; }
            set { _alertmsg = value; TempData["AlertJsonStr"] = AlertJsonStr; }
        }

        /// <summary>
        /// 获取弹出消息Json字符串
        /// </summary>
        protected string AlertJsonStr
        {
            get 
            {
                if (_alerttype.HasValue)
                {
                    if (_alertmsg == null)
                    {
                        _alertmsg = string.Empty;
                    }
                    return "{\"AlertType\" : \"" + Enum.GetName(typeof(eAlertType), _alerttype) + "\", \"AlertMsg\" : \"" + _alertmsg + "\"}";
                }
                return null;
            }
        }

        bool _checklogindefault = false;
        /// <summary>
        /// 检查登录默认值
        /// </summary>
        public bool CheckLoginDefault
        {
            get { return _checklogindefault; }
            set { _checklogindefault = value; }
        }

        bool _checkpermissionsdefault = false;
        /// <summary>
        /// 检查权限默认值
        /// </summary>
        public bool CheckPermissionsDefault
        {
            get { return _checkpermissionsdefault; }
            set { _checkpermissionsdefault = value; }
        }

        /// <summary>
        /// 当页面需要验证登录，而用户未登录时跳转的页面
        /// </summary>
        public string LoginFailUrl { get; set; }

        /// <summary>
        /// 当页面需要验证权限，而用户没有权限时跳转的页面
        /// </summary>
        public string PermissionsFailUrl { get; set; }

        Model_T _model_login;
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public Model_T Model_Login
        {
            get { return _model_login; }
        }

        string _cookie_login_adr = "Cookie_Login";
        /// <summary>
        /// 保存在Cookie里的登录信息的地址
        /// </summary>
        public string Cookie_Login_Adr
        {
            get { return _cookie_login_adr; }
            set { _cookie_login_adr = value; }
        }

        /// <summary>
        /// 保存在Cookie里的登录信息的用户名取自Model_Login哪个属性
        /// </summary>
        public string Cookie_LoginName_ModelProName { get; set; }

        /// <summary>
        /// 保存在Cookie里的登录信息的用户密码取自Model_Login哪个属性
        /// </summary>
        public string Cookie_LoginPwd_ModelProName { get; set; }

        int _cookie_login_expires = 7;
        /// <summary>
        /// 保存在Cookie里的登录信息的过期时间（以天为单位，默认7天）
        /// </summary>
        public int Cookie_Login_Expires
        {
            get { return _cookie_login_expires; }
            set { _cookie_login_expires = value; }
        }

        string _previouspagename = "PreviousPage";
        /// <summary>
        /// 上一页的保存在Cookie里的键名称
        /// </summary>
        public string PreviousPageName 
        { 
            get { return _previouspagename; }
            set { _previouspagename = value; }
        }

        /// <summary>
        /// 上一页不包含哪些页面
        /// </summary>
        public List<string> PreviousPageUrl_NoContain { get; set; }

        /// <summary>
        /// 上一页默认地址（当无上一页时跳转到该地址）
        /// </summary>
        public string PreviousPageUrl_Default { get; set; }

        /// <summary>
        /// 上一页（两次跳转相同页不重新赋值，去除OPT后统计）
        /// </summary>
        public string PreviousPageUrl
        {
            get
            {
                string PreviousPageUrlValue = WebHelper.WebHelper.GetCookieValue(PreviousPageName);
                if (!string.IsNullOrEmpty(PreviousPageUrlValue))
                {
                    return PreviousPageUrlValue;
                }
                return PreviousPageUrl_Default;
            }
            set
            {
                bool IsContain = false;
                if (PreviousPageUrl_NoContain != null)
                {
                    foreach (string item in PreviousPageUrl_NoContain)
                    {
                        if (value.Contains(item))
                        {
                            IsContain = true;
                            break;
                        }
                    }
                }
                if (!IsContain)
                {
                    WebHelper.WebHelper.SetCookieValue(PreviousPageName, value, DateTime.Today.AddDays(1));
                }
            }
        }

        /// <summary>
        /// Cookie中验证码的加密密钥
        /// </summary>
        public string CookieVCodePwd { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 类型转换
        /// </summary>
        protected T C<T>(object ChangeValue, T DefaultValue)
        {
            return THelper.C(ChangeValue, DefaultValue);
        }

        /// <summary>
        /// 类实例化
        /// </summary>
        protected T R<T>(ref T RClass, object[] Params = null)
        {
            return THelper.R(ref RClass, Params);
        }

        /// <summary>
        /// 从数据库获取登录用户的信息，必须重写此方法
        /// </summary>
        protected abstract Model_T GetLoginModel(Model_T LoginModel);

        /// <summary>
        /// 验证权限是否正确
        /// </summary>
        protected virtual bool CheckPermissions() 
        {
            return false;
        }

        /// <summary>
        /// 创建一个将视图呈现给响应的 System.Web.Mvc.ViewResult 对象。
        /// </summary>
        /// <returns>将视图呈现给响应的视图结果。</returns>
        protected new ActionResult View()
        {
            return View(null);
        }

        /// <summary>
        /// 使用模型创建一个将视图呈现给响应的 System.Web.Mvc.ViewResult 对象。
        /// </summary>
        /// <param name="model">视图呈现的模型。</param>
        /// <returns>视图结果。</returns>
        protected new ActionResult View(object model)
        {
            string OPT = Request.QueryString["OPT"];
            if (!string.IsNullOrEmpty(OPT))
            {
                string Url = string.Empty;
                foreach (string key in Request.QueryString.Keys)
                {
                    if (key != "OPT")
                    {
                        if (!string.IsNullOrEmpty(Url))
                        {
                            Url += "&";
                        }
                        Url += Request.QueryString[key];
                    }
                }
                return Redirect(Request.Path + ((!string.IsNullOrEmpty(Url)) ? "?" + Url : string.Empty));
            }

            //返回上一页地址
            if ((Request.UrlReferrer != null) && (Request.UrlReferrer != Request.Url))
            {
                PreviousPageUrl = Request.UrlReferrer.ToString();
            }

            return base.View(model);
        }

        /// <summary>
        /// 在调用操作方法前调用。
        /// </summary>
        /// <param name="filterContext">有关当前请求和操作的信息</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //获取登录用户信息
            _model_login = GetLoginCookie();
            if (_model_login != null)
                _model_login = GetLoginModel(_model_login);

            //检查是否需要特殊验证
            CheckAttribute CheckAttribute = THelper.GetCustomAttribute<CheckAttribute>(filterContext.Controller.GetType().GetMethod(filterContext.ActionDescriptor.ActionName, (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)));
            if (CheckAttribute == null)
                CheckAttribute = THelper.GetCustomAttribute<CheckAttribute>(filterContext.Controller.GetType());

            //特殊验证（登录）
            if (CheckAttribute != null)
                CheckLoginDefault = CheckAttribute.CheckLogin;                 //检查是否是否需要验证登录
            if (CheckLoginDefault)
            {
                //检测是否已登录成功
                if (Model_Login == null)
                    filterContext.Result = Redirect((string.IsNullOrEmpty(LoginFailUrl)) ? "/Home/Index" : LoginFailUrl);
                {
                    //特殊验证（权限）
                    if (CheckAttribute != null)
                        CheckPermissionsDefault = CheckAttribute.CheckPermissions;                 //检查是否是否需要验证权限
                    if (CheckPermissionsDefault)
                    {
                        if (!CheckPermissions())
                            filterContext.Result = Redirect((string.IsNullOrEmpty(PermissionsFailUrl)) ? "/Home/Index" : PermissionsFailUrl);
                    }
                }
            }
            ViewBag.Model_Login = Model_Login;

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 保存用户登录信息
        /// </summary>
        protected void SetLoginCookie(Model_T model)
        {
            if (!string.IsNullOrEmpty(Cookie_Login_Adr))
            {
                if (!string.IsNullOrEmpty(Cookie_LoginName_ModelProName))
                {
                    Type Model_T_Type = typeof(Model_T);
                    System.Collections.Specialized.NameValueCollection Cookie_ModelPros = new System.Collections.Specialized.NameValueCollection();
                    Cookie_ModelPros.Add("LoginName", Model_T_Type.GetProperty(Cookie_LoginName_ModelProName).GetValue(model, null).ToString());
                    if (!string.IsNullOrEmpty(Cookie_LoginPwd_ModelProName))
                    {
                        Cookie_ModelPros.Add("LoginPwd", Model_T_Type.GetProperty(Cookie_LoginPwd_ModelProName).GetValue(model, null).ToString());
                    }
                    WebHelper.WebHelper.SetCookie(Cookie_Login_Adr, Cookie_ModelPros, DateTime.Now.AddDays(Cookie_Login_Expires));
                }
                else
                {
                    throw new Exception("未设置'Cookie_LoginName_ModelProName'值");
                }
            }
            else
            {
                throw new Exception("未设置'Cookie_Login_Adr'值");
            }
        }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns>用户登录信息</returns>
        private Model_T GetLoginCookie()
        {
            Model_T model = default(Model_T);
            if (!string.IsNullOrEmpty(Cookie_Login_Adr))
            {
                if (!string.IsNullOrEmpty(Cookie_LoginName_ModelProName))
                {
                    System.Collections.Specialized.NameValueCollection CookieValues = WebHelper.WebHelper.GetCookie(Cookie_Login_Adr);
                    if (CookieValues != null)
                    {
                        model = System.Activator.CreateInstance<Model_T>();
                        Type Model_T_Type = typeof(Model_T);
                        Model_T_Type.GetProperty(Cookie_LoginName_ModelProName).SetValue(model, CookieValues["LoginName"], null);
                        if (!string.IsNullOrEmpty(Cookie_LoginPwd_ModelProName))
                        {
                            Model_T_Type.GetProperty(Cookie_LoginPwd_ModelProName).SetValue(model, CookieValues["LoginPwd"], null);
                        }
                    }
                }
                else
                {
                    throw new Exception("未设置'Cookie_LoginName_ModelProName'值");
                }
            }
            else
            {
                throw new Exception("未设置'Cookie_Login_Adr'值");
            }
            return model;
        }

        /// <summary>
        /// 清除用户登录信息
        /// </summary>
        protected void ClearLoginCookie()
        {
            if (!string.IsNullOrEmpty(Cookie_Login_Adr))
            {
                WebHelper.WebHelper.ClearCookie(Cookie_Login_Adr);
            }
        }

        /// <summary>
        /// 创建一个随机验证码并保存在Cookie中
        /// </summary>
        /// <param name="CookieKey">Cookie键名</param>
        protected void SetCookieVCode(string CookieKey)
        {
            if (!string.IsNullOrEmpty(CookieKey))
            {
                string RandomString = Easytl.THelper.GetRandomString(8);
                Easytl.WebHelper.WebHelper.SetCookieValue(CookieKey, SafeHelper.EncryptionHelper.Encrypt(SafeHelper.EncryptionHelper.Encrypt_RPType.Str16, Easytl.SafeHelper.EncryptionHelper.Encrypt_Type.MD5, RandomString + CookieVCodePwd) + RandomString, DateTime.Now.AddMinutes(15));
            }
        }

        /// <summary>
        /// 验证Cookie中验证码是否存在并正确
        /// </summary>
        /// <param name="CookieKey">Cookie键名</param>
        protected bool VerificationCookieVCode(string CookieKey)
        {
            if (!string.IsNullOrEmpty(CookieKey))
            {
                string EncryptString = Easytl.WebHelper.WebHelper.GetCookieValue(CookieKey);
                if (!string.IsNullOrEmpty(EncryptString))
                {
                    if (EncryptString.Length > 32)
                    {
                        string RandomString = EncryptString.Substring(32);
                        if ((SafeHelper.EncryptionHelper.Encrypt(SafeHelper.EncryptionHelper.Encrypt_RPType.Str16, Easytl.SafeHelper.EncryptionHelper.Encrypt_Type.MD5, RandomString + CookieVCodePwd) + RandomString) == EncryptString)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 提取地址栏参数
        /// </summary>
        protected System.Collections.Specialized.NameValueCollection UrlParas
        {
            get { return HttpContext.Request.QueryString; }
        }

        /// <summary>
        /// 提取表单参数
        /// </summary>
        protected System.Collections.Specialized.NameValueCollection FormParas
        {
            get { return HttpContext.Request.Form; }
        }

        #endregion
    }
}