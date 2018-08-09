using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Easytl.WebControllers
{
    /// <summary>
    /// 自定义控制器分页基类
    /// </summary>
    /// <typeparam name="Model_T">登录用户类类型</typeparam>
    public abstract class PagerController<Model_T> : BaseController<Model_T>
    {
        string _PageIndexName = "PageIndex";
        /// <summary>
        /// 获取或设置当前页的参数名
        /// </summary>
        protected string PageIndexName
        {
            get { return _PageIndexName; }
            set { _PageIndexName = value; }
        }

        /// <summary>
        /// 获取或设置页条数的参数名
        /// </summary>
        protected string PageSizeName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取当前页码
        /// </summary>
        protected int PageIndex
        {
            get
            {
                if (!string.IsNullOrEmpty(PageIndexName))
                {
                    return Easytl.WebHelper.RequestHelper.GetPara(Easytl.WebHelper.RequestHelper.RequestType.Get, PageIndexName, 1);
                }
                return 1;
            }
        }

        int _PageSize = 10;
        /// <summary>
        /// 获取或设置当前每页显示数据条数
        /// </summary>
        protected int PageSize
        {
            get
            {
                if (!string.IsNullOrEmpty(PageSizeName))
                {
                    return Easytl.WebHelper.RequestHelper.GetPara(Easytl.WebHelper.RequestHelper.RequestType.Get, PageSizeName, _PageSize);
                }
                return _PageSize;
            }
            set { _PageSize = value; }
        }

        int _RecordCount = -1;
        /// <summary>
        /// 获取或设置总记录数
        /// </summary>
        protected int RecordCount 
        {
            get { return _RecordCount; }
            set { _RecordCount = value; }
        }

        /// <summary>
        /// 获取总页码数
        /// </summary>
        protected int PageCount { get { return ((RecordCount % PageSize) > 0) ? ((RecordCount / PageSize) + 1) : (RecordCount / PageSize); } }

        int _ShowPageCount = 5;
        /// <summary>
        /// 获取或设置显示页码数
        /// </summary>
        protected int ShowPageCount { get { return _ShowPageCount; } set { _ShowPageCount = value; } }


        /// <summary>
        /// 在调用操作方法后调用
        /// </summary>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.RecordCount = RecordCount;
            ViewBag.PageCount = PageCount;

            if (RecordCount >= 0)
            {
                //传输地址栏参数
                string PageParamStr = string.Empty;
                foreach (string key in Request.QueryString.Keys)
                {
                    if (!string.IsNullOrEmpty(PageParamStr))
                    {
                        PageParamStr += "&";
                    }
                    PageParamStr += key + "=" + Request.QueryString[key];
                }
                ViewBag.Pagination = new Pagination(Request.Path, PageIndexName, PageSizeName, PageIndex, PageSize, PageCount, ShowPageCount, PageParamStr);
            }

            base.OnActionExecuted(filterContext);
        }
    }

    /// <summary>
    /// 分页类
    /// </summary>
    public class Pagination
    {
        string LiUrl;
        string PageIndexName;
        string PageSizeName;
        int PageIndex;
        int PageSize;
        int PageCount;
        int ShowPageCount;
        string PageParamStr;

        public string ul_class { get; set; }

        public string li_class { get; set; }

        public string li_active_class { get; set; }

        public string li_disabled_class { get; set; }

        public string li_a_class { get; set; }

        public string li_a_active_class { get; set; }

        public string li_a_disabled_class { get; set; }

        public Pagination(string liUrl, string pageIndexName, string pageSizeName, int pageIndex, int pageSize, int pageCount, int showPageCount, string pageParamStr = "")
        {
            LiUrl = liUrl;
            PageIndexName = pageIndexName;
            PageSizeName = pageSizeName;
            PageIndex = pageIndex;
            PageSize = pageSize;
            PageCount = pageCount;
            ShowPageCount = showPageCount;
            PageParamStr = pageParamStr;
        }

        public override string ToString()
        {
            string PagerHtml = string.Empty;
            if (PageCount > 0)
            {
                string RawUrl = System.Text.RegularExpressions.Regex.Match(LiUrl, "[^?]*").Groups[0].Value + "?";
                if (!string.IsNullOrEmpty(PageParamStr))
                {
                    RawUrl += PageParamStr;
                    RawUrl = System.Text.RegularExpressions.Regex.Replace(RawUrl, "&{0,}" + PageIndexName + "=\\d*", string.Empty);
                    if (!string.IsNullOrEmpty(PageSizeName))
                        RawUrl = System.Text.RegularExpressions.Regex.Replace(RawUrl, "&{0,}" + PageSizeName + "=\\d*", string.Empty);
                    if (RawUrl.IndexOf("?") < (RawUrl.Length - 1))
                    {
                        RawUrl += "&";
                    }
                }

                if (!string.IsNullOrEmpty(PageSizeName))
                    RawUrl += "" + PageSizeName + "=" + PageSize + "&";


                //输出分页Html代码
                string ul_class_str = (!string.IsNullOrEmpty(ul_class)) ? "class='" + ul_class + "'" : "";
                string li_class_str = (!string.IsNullOrEmpty(li_class)) ? "class='" + li_class + "'" : "";
                string li_active_class_str = (!string.IsNullOrEmpty(li_active_class)) ? "class='" + li_active_class + "'" : "";
                string li_disabled_class_str = (!string.IsNullOrEmpty(li_disabled_class)) ? "class='" + li_disabled_class + "'" : "";
                string li_a_class_str = (!string.IsNullOrEmpty(li_a_class)) ? "class='" + li_a_class + "'" : "";
                string li_a_active_class_str = (!string.IsNullOrEmpty(li_a_active_class)) ? "class='" + li_a_active_class + "'" : "";
                string li_a_disabled_class_str = (!string.IsNullOrEmpty(li_a_disabled_class)) ? "class='" + li_a_disabled_class + "'" : "";

                PagerHtml += @"<ul " + ul_class_str + @">
                                     <li " + li_class_str + "><a " + li_a_class_str + " href='" + RawUrl + "" + PageIndexName + "=1'>首页</a></li>";
                if ((PageIndex - 1) > 0)
                {
                    PagerHtml += "<li " + li_class_str + "><a " + li_a_class_str + " href='" + RawUrl + "" + PageIndexName + "=" + (PageIndex - 1) + "'>上页</a></li>";
                }
                else
                {
                    PagerHtml += "<li " + li_disabled_class_str + "><a " + li_a_disabled_class_str + ">上页</a></li>";
                }

                if (PageCount < ShowPageCount)
                {
                    ShowPageCount = PageCount;
                }

                int StartPageIndex = 1;
                if ((PageIndex - (ShowPageCount / 2)) > 0)
                {
                    StartPageIndex = PageIndex - (ShowPageCount / 2);
                }
                if ((PageIndex + (ShowPageCount / 2)) >= PageCount)
                {
                    StartPageIndex = PageCount - ShowPageCount + 1;
                }
                int EndPageIndex = StartPageIndex + ShowPageCount;
                for (int i = StartPageIndex; i < EndPageIndex; i++)
                {
                    if (i == PageIndex)
                    {
                        PagerHtml += "<li " + li_active_class_str + "><a " + li_a_active_class_str + " href='" + RawUrl + "" + PageIndexName + "=" + i + "'>" + i + "</a></li>";
                    }
                    else
                    {
                        PagerHtml += "<li " + li_class_str + "><a " + li_a_class_str + " href='" + RawUrl + "" + PageIndexName + "=" + i + "'>" + i + "</a></li>";
                    }
                }

                if ((PageIndex + 1) <= PageCount)
                {
                    PagerHtml += "<li " + li_class_str + "><a " + li_a_class_str + " href='" + RawUrl + "" + PageIndexName + "=" + (PageIndex + 1) + "'>下页</a></li>";
                }
                else
                {
                    PagerHtml += "<li " + li_disabled_class_str + "><a " + li_a_disabled_class_str + ">下页</a></li>";
                }

                PagerHtml += @"<li " + li_class_str + "><a " + li_a_class_str + " href='" + RawUrl + "" + PageIndexName + "=" + PageCount + @"'>尾页</a></li>
                            </ul>";
            }
            PagerHtml += @"
                       <script type='text/javascript'>
                          var PageParamStr = '" + PageParamStr + @"';
                             if (PageParamStr != null) {
                                var PageParams = PageParamStr.split('&');
                                for (var i = 0; i < PageParams.length; i++) {
                                    if (PageParams[i] != null) {
                                        var PageParamsI = PageParams[i].split('=');
                                        if (PageParamsI.length >= 2) {
                                            if (PageParamsI[1] != '') {
                                                $('#' + PageParamsI[0]).val(PageParamsI[1]);
                                            }
                                        }
                                    }
                                }
                            }
                       </script>";

            return PagerHtml;
        }

        public HtmlString ToHtmlString()
        {
            return new HtmlString(this.ToString());
        }
    }
}
