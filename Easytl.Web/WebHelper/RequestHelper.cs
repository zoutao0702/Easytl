﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace Easytl.Web.WebHelper
{
    public class RequestHelper : WebHelper
    {
        /// <summary>
        /// 获取当前请求方式
        /// </summary>
        /// <returns></returns>
        public static RequestType GetRequestType()
        {
            RequestType requestType = RequestType.NoKown;
            if (HttpContext.Current.Request.HttpMethod.Equals("GET"))
            {
                requestType = RequestType.Get;
            }
            else if (HttpContext.Current.Request.HttpMethod.Equals("POST"))
            {
                requestType = RequestType.Post;
            }
            return requestType;
        }


        /// <summary>
        /// 获取参数
        /// </summary>
        public static T GetPara<T>(RequestType requestType, string paraName, T paraValue, bool DefaultNull = false)
        {
            string strvalue = string.Empty;
            switch (requestType)
            {
                case RequestType.Get:
                    strvalue = HttpContext.Current.Request.QueryString[paraName];
                    break;
                case RequestType.Post:
                    strvalue = HttpContext.Current.Request.Form[paraName];
                    break;
            }
            if (strvalue == null)
            {
                if (DefaultNull)
                {
                    return default(T);
                }
                return paraValue;
            }
            else
            {
                try
                {
                    Type DType = typeof(T);
                    if (DType.IsEnum)
                    {
                        return (T)Enum.Parse(DType, strvalue.ToString());
                    }
                    else
                    {
                        switch (DType.Name.ToLower())
                        {
                            case "guid":
                                return (T)Convert.ChangeType(Guid.Parse(strvalue.ToString()), DType);
                            default:
                                return (T)Convert.ChangeType(strvalue, DType);
                        }
                    }
                }
                catch
                {
                    return paraValue;
                }
            }
        }

        /// <summary>
        /// 模拟http请求（raw）
        /// </summary>
        public static string HttpRequest(RequestType requestType, out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, string dataStr = "", NameValueCollection headers = null, string contentType = "text/json", string encodingType = "UTF-8", int timeout = 10000)
        {
            try
            {
                string Method = "GET";
                switch (requestType)
                {
                    case RequestType.NoKown:
                        goto case RequestType.Get;
                    case RequestType.Get:
                        Method = "GET";
                        if (!string.IsNullOrEmpty(dataStr))
                        {
                            url = string.Concat(url, "?", dataStr);
                            dataStr = string.Empty;
                        }
                        break;
                    case RequestType.Post:
                        Method = "POST";
                        break;
                    case RequestType.Put:
                        Method = "PUT";
                        break;
                    case RequestType.Delete:
                        Method = "DELETE";
                        break;
                }

                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        myRequest.Headers.Add(key, headers[key]);
                    }
                }
                myRequest.Method = Method;
                myRequest.Timeout = timeout;
                myRequest.ReadWriteTimeout = timeout;
                myRequest.ContentType = contentType;
                if (!string.IsNullOrEmpty(dataStr))
                {
                    byte[] data = Encoding.GetEncoding(encodingType).GetBytes(dataStr);
                    myRequest.ContentLength = data.Length;
                    Stream newStream = myRequest.GetRequestStream();

                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }

                HttpWebResponse response = null;
                OpStatusCode = HttpStatusCode.NotFound;
                OpStatusDescription = null;
                try
                {
                    response = (HttpWebResponse)myRequest.GetResponse();
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        response = (HttpWebResponse)e.Response;
                    else
                    {
                        OpStatusCode = HttpStatusCode.RequestTimeout;
                        OpStatusDescription = e.Message;
                    }
                }

                if (response != null)
                {
                    OpStatusCode = response.StatusCode;
                    OpStatusDescription = response.StatusDescription;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encodingType)))
                    {
                        string result = sr.ReadToEnd();
                        sr.Close();
                        response.Close();
                        return result;
                    }
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                OpStatusCode = HttpStatusCode.ExpectationFailed;
                OpStatusDescription = ex.Message;
                return string.Empty;
            }
        }


        /// <summary>
        /// 模拟http请求（application/x-www-form-urlencoded）
        /// </summary>
        public static string HttpRequest_Get(out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, NameValueCollection values, NameValueCollection headers = null, string encodingType = "UTF-8", int timeout = 10000)
        {
            string dataStr = string.Empty;
            if (values.Count > 0)
            {
                foreach (string key in values.Keys)
                {
                    if (!string.IsNullOrEmpty(dataStr))
                        dataStr += "&";

                    dataStr += key + "=" + values[key];
                }
            }

            return HttpRequest(RequestType.Get, out OpStatusCode, out OpStatusDescription, url, dataStr, headers, "application/x-www-form-urlencoded;charset=" + Encoding.GetEncoding(encodingType).WebName, encodingType, timeout);
        }


        /// <summary>
        /// 模拟http请求（application/x-www-form-urlencoded）
        /// </summary>
        public static string HttpRequest_Get_T<T>(out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, T values, BindingFlags bindingAttr = BindingFlags.Public, NameValueCollection headers = null, string encodingType = "UTF-8", int timeout = 10000)
        {
            PropertyInfo[] properties;
            if (bindingAttr == BindingFlags.Public)
                properties = values.GetType().GetProperties();
            else
                properties = values.GetType().GetProperties(bindingAttr);

            NameValueCollection nameValueCollection = new NameValueCollection();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.CanRead)
                {
                    object value = propertyInfo.GetValue(values, null);
                    if (value != null)
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                            nameValueCollection.Add(propertyInfo.Name, Convert.ToInt32(value).ToString());
                        else
                            nameValueCollection.Add(propertyInfo.Name, value.ToString());
                    }
                }
            }
            return HttpRequest_Get(out OpStatusCode, out OpStatusDescription, url, nameValueCollection, headers, encodingType, timeout);
        }


        /// <summary>
        /// 模拟http/post请求（application/x-www-form-urlencoded）
        /// </summary>
        public static string HttpRequest_Post(out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, NameValueCollection values, NameValueCollection headers = null, string encodingType = "UTF-8", int timeout = 10000)
        {
            string dataStr = string.Empty;
            if (values.Count > 0)
            {
                foreach (string key in values.Keys)
                {
                    if (!string.IsNullOrEmpty(dataStr))
                        dataStr += "&";

                    dataStr += key + "=" + values[key];
                }
            }

            return HttpRequest(RequestType.Post, out OpStatusCode, out OpStatusDescription, url, dataStr, headers, "application/x-www-form-urlencoded;charset=" + Encoding.GetEncoding(encodingType).WebName, encodingType, timeout);
        }


        /// <summary>
        /// 模拟http/post请求（application/x-www-form-urlencoded）
        /// </summary>
        public static string HttpRequest_Post_T<T>(out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, T values, BindingFlags bindingAttr = BindingFlags.Public, NameValueCollection headers = null, string encodingType = "UTF-8", int timeout = 10000)
        {
            PropertyInfo[] properties;
            if (bindingAttr == BindingFlags.Public)
                properties = values.GetType().GetProperties();
            else
                properties = values.GetType().GetProperties(bindingAttr);

            NameValueCollection nameValueCollection = new NameValueCollection();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.CanRead)
                {
                    object value = propertyInfo.GetValue(values, null);
                    if (value != null)
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                            nameValueCollection.Add(propertyInfo.Name, Convert.ToInt32(value).ToString());
                        else
                            nameValueCollection.Add(propertyInfo.Name, value.ToString());
                    }
                }
            }
            return HttpRequest_Post(out OpStatusCode, out OpStatusDescription, url, nameValueCollection, headers, encodingType, timeout);
        }


        /// <summary>
        /// 模拟http/post请求（multipart/form-data）
        /// </summary>
        public static string HttpRequest_Post_FormData(out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, NameValueCollection values, List<PostUploadFile> files = null, NameValueCollection headers = null, string encodingType = "UTF-8", int timeout = 10000, string boundary = null)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                Encoding encoding = Encoding.GetEncoding(encodingType);
                if (headers != null)
                {
                    foreach (string text in headers.Keys)
                    {
                        httpWebRequest.Headers.Add(text, headers[text]);
                    }
                }
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "*/*";
                httpWebRequest.Timeout = timeout;
                httpWebRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                httpWebRequest.ReadWriteTimeout = timeout;

                if (string.IsNullOrEmpty(boundary))
                    boundary = string.Format("----WebKitFormBoundary{0}", THelper.GetRandomString(15));
                httpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

                Stream requestStream = httpWebRequest.GetRequestStream();
                string Enter = "\r\n";
                string Str;
                byte[] bytes;
                if (values.Count > 0)
                {
                    foreach (string key in values.Keys)
                    {
                        Str = string.Format("--{0}{1}Content-Disposition: form-data; name=\"{2}\"{1}{1}{3}{1}", boundary, Enter, key, values[key]);

                        bytes = encoding.GetBytes(Str);
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }

                if (files != null)
                {
                    foreach (PostUploadFile file in files)
                    {
                        Str = string.Format("--{0}{1}Content-Disposition: form-data; name=\"{2}\"; filename=\"{3}\"{1}", boundary, Enter, file.FieldName, file.FileName);

                        if (!string.IsNullOrEmpty(file.ContentType))
                            Str += string.Format("Content-Type:{0}{1}{1}", file.ContentType, Enter);

                        bytes = encoding.GetBytes(Str);
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Write(file.Content, 0, file.ContentLength);
                        bytes = encoding.GetBytes(Enter);
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }

                Str = string.Format("--{0}--", boundary);
                bytes = encoding.GetBytes(Str);
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();

                HttpWebResponse httpWebResponse = null;
                OpStatusCode = HttpStatusCode.NotFound;
                OpStatusDescription = null;
                try
                {
                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        httpWebResponse = (HttpWebResponse)e.Response;
                    else
                    {
                        OpStatusCode = HttpStatusCode.RequestTimeout;
                        OpStatusDescription = e.Message;
                    }
                }

                if (httpWebResponse != null)
                {
                    OpStatusCode = httpWebResponse.StatusCode;
                    OpStatusDescription = httpWebResponse.StatusDescription;
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), encoding))
                    {
                        string result = streamReader.ReadToEnd();
                        streamReader.Close();
                        httpWebResponse.Close();
                        return result;
                    }
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                OpStatusCode = HttpStatusCode.ExpectationFailed;
                OpStatusDescription = ex.Message;
                return string.Empty;
            }
        }


        /// <summary>
        /// 模拟http/post请求（multipart/form-data）
        /// </summary>
        public static string HttpRequest_Post_FormData_T<T>(out HttpStatusCode OpStatusCode, out string OpStatusDescription, string url, T values, BindingFlags bindingAttr = BindingFlags.Public, List<PostUploadFile> files = null, NameValueCollection headers = null, string encodingType = "UTF-8", int timeout = 10000, string boundary = null)
        {
            PropertyInfo[] properties;
            if (bindingAttr == BindingFlags.Public)
                properties = values.GetType().GetProperties();
            else
                properties = values.GetType().GetProperties(bindingAttr);

            NameValueCollection nameValueCollection = new NameValueCollection();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (propertyInfo.CanRead)
                {
                    object value = propertyInfo.GetValue(values, null);
                    if (value != null)
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                            nameValueCollection.Add(propertyInfo.Name, Convert.ToInt32(value).ToString());
                        else
                            nameValueCollection.Add(propertyInfo.Name, value.ToString());
                    }
                }
            }
            return RequestHelper.HttpRequest_Post_FormData(out OpStatusCode, out OpStatusDescription, url, nameValueCollection, files, headers, encodingType, timeout);
        }


        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="DirectoryVirtualPath">目录虚拟路径</param>
        /// <param name="FileName">文件名称</param>
        /// <param name="FilePostName">文件上传名称</param>
        /// <param name="FileSaveUrl">文件保存后的网络地址</param>
        /// <param name="AddTime">是否在文件名称后加时间</param>
        /// <param name="CreateDateFolder">是否分时间文件夹存储文件</param>
        /// <param name="AddDomainName">是否返回带域名的网络地址</param>
        public static bool SaveUploadFile(string DirectoryVirtualPath, string FileName, string FilePostName, out string FileSaveUrl, bool AddTime = false, bool CreateDateFolder = true, bool AddDomainName = false)
        {
            HttpRequest Request = HttpContext.Current.Request;
            FileSaveUrl = string.Empty;
            if (!string.IsNullOrEmpty(DirectoryVirtualPath))
            {
                int m = 0;
                if (Request.Files.Count > 0)
                {
                    if (CreateDateFolder)
                        DirectoryVirtualPath += @"/" + DateTime.Today.ToString("yyyy-MM");
                    FileHelper.FolderHelper.CreateFolder(Request.MapPath(DirectoryVirtualPath));

                    foreach (string FileKey in Request.Files.Keys)
                    {
                        if (FileKey.Contains(FilePostName))
                        {
                            if (Request.Files[FileKey].ContentLength > 0)
                            {
                                m++;

                                string FileType = string.Empty;
                                string[] FileTypeCollection = Request.Files[FileKey].ContentType.Split('/');
                                if (FileTypeCollection.Length > 1)
                                {
                                    FileType = FileTypeCollection[1].ToLower();
                                    switch (FileType)
                                    {
                                        case "pjpeg":
                                            FileType = "jpeg";
                                            break;
                                        case "x-png":
                                            FileType = "png";
                                            break;
                                        case "octet-stream":
                                            int FileTypeIndex = Request.Files[FileKey].FileName.LastIndexOf('.');
                                            if ((FileTypeIndex > 0) && (FileTypeIndex + 1 < Request.Files[FileKey].FileName.Length))
                                                FileType = Request.Files[FileKey].FileName.Substring(FileTypeIndex + 1);
                                            break;
                                    }
                                }
                                else
                                {
                                    int FileTypeIndex = Request.Files[FileKey].FileName.LastIndexOf('.');
                                    if ((FileTypeIndex > 0) && (FileTypeIndex + 1 < Request.Files[FileKey].FileName.Length))
                                        FileType = Request.Files[FileKey].FileName.Substring(FileTypeIndex + 1);
                                }
                                string FileUrl = DirectoryVirtualPath + "/" + FileName + "-" + m.ToString() + ((AddTime) ? DateTime.Now.ToString("yyyyMMddHHmmss") : string.Empty) + ((string.IsNullOrEmpty(FileType)) ? string.Empty : "." + FileType);
                                Request.Files[FileKey].SaveAs(Request.MapPath(FileUrl));

                                if (!string.IsNullOrEmpty(FileSaveUrl))
                                    FileSaveUrl += ",";

                                FileSaveUrl += FileUrl;
                            }
                        }
                    }
                }

                if (m <= 0)
                    return false;

                if (AddDomainName)
                {
                    if (FileSaveUrl.Substring(0, 1) != "/")
                    {
                        if (FileSaveUrl.Substring(0, 2) == "~/")
                            FileSaveUrl = FileSaveUrl.Substring(1);
                        else
                            FileSaveUrl = "/" + FileSaveUrl;
                    }
                    FileSaveUrl = "http://" + HttpContext.Current.Request.Url.Authority + FileSaveUrl;
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 下载网络资源
        /// </summary>
        /// <param name="Url">网络资源地址</param>
        /// <param name="FileType">网络文件类型</param>
        /// <param name="DirectoryVirtualPath">目录虚拟路径</param>
        /// <param name="FileName">文件名称</param>
        /// <param name="FileSaveUrl">文件保存后的网络地址</param>
        /// <param name="AddTime">是否在文件名称后加时间</param>
        /// <param name="CreateDateFolder">是否分时间文件夹存储文件</param>
        /// <param name="AddDomainName">是否返回带域名的网络地址</param>
        /// <returns></returns>
        public static bool DownloadFile(string Url, string FileType, string DirectoryVirtualPath, string FileName, out string FileSaveUrl, bool AddTime = false, bool CreateDateFolder = true, bool AddDomainName = false)
        {
            HttpRequest Request = HttpContext.Current.Request;
            FileSaveUrl = string.Empty;
            if (!string.IsNullOrEmpty(DirectoryVirtualPath))
            {
                WebClient myWebClient = new WebClient();

                if (CreateDateFolder)
                    DirectoryVirtualPath += @"/" + DateTime.Today.ToString("yyyy-MM");
                FileHelper.FolderHelper.CreateFolder(Request.MapPath(DirectoryVirtualPath));

                string FileUrl = DirectoryVirtualPath + "/" + FileName + "-1" + ((AddTime) ? DateTime.Now.ToString("yyyyMMddHHmmss") : string.Empty) + ((string.IsNullOrEmpty(FileType)) ? string.Empty : "." + FileType);
                myWebClient.DownloadFile(Url, Request.MapPath(FileUrl));
                FileSaveUrl = FileUrl;

                if (AddDomainName)
                {
                    if (FileSaveUrl.Substring(0, 1) != "/")
                    {
                        if (FileSaveUrl.Substring(0, 2) == "~/")
                            FileSaveUrl = FileSaveUrl.Substring(1);
                        else
                            FileSaveUrl = "/" + FileSaveUrl;
                    }
                    FileSaveUrl = "http://" + HttpContext.Current.Request.Url.Authority + FileSaveUrl;
                }

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 模拟浏览器抓取网页内容
        /// </summary>
        public static string GetHtml(string url, string encodingType = "UTF-8")
        {
            string Html = string.Empty;//初始化新的webRequst
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);

            //Request.KeepAlive = true;
            Request.ProtocolVersion = HttpVersion.Version11;
            Request.Method = "GET";
            Request.Accept = "*/* ";
            Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";
            Request.Referer = url;

            HttpWebResponse htmlResponse = (HttpWebResponse)Request.GetResponse();
            //从Internet资源返回数据流
            Stream htmlStream = htmlResponse.GetResponseStream();
            //读取数据流
            StreamReader weatherStreamReader = new StreamReader(htmlStream, Encoding.GetEncoding(encodingType));
            //读取数据
            Html = weatherStreamReader.ReadToEnd();
            weatherStreamReader.Close();
            htmlStream.Close();
            htmlResponse.Close();
            //针对不同的网站查看html源文件
            return Html;
        }

    }

    /// <summary>
    /// 上传文件类
    /// </summary>
    public class PostUploadFile
    {
        /// <summary>
        /// 上传文件时的表单域名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 上传文件的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件字节大小
        /// </summary>
        public int ContentLength { get; set; }

        /// <summary>
        /// 文件二进制内容
        /// </summary>
        public byte[] Content { get; set; }
    }
}
