using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using Easytl.Web.WebHelper;
using Easytl.Web.PayHelper.WeiXin;

namespace Easytl.Web.PayHelper
{
    public partial class WeiXinHelper
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        string appid;

        /// <summary>
        /// 商户号
        /// </summary>
        string mch_id;

        /// <summary>
        /// 设备号
        /// </summary>
        string device_info;

        /// <summary>
        /// API密钥
        /// </summary>
        string APIKey;
    }

    public partial class WeiXinHelper
    {
        static SafeHelper.EncryptionHelper WX_EncryptionHelper = new SafeHelper.EncryptionHelper() { Encode = Encoding.GetEncoding("UTF-8") };
        static SignHelper.SignHelper WX_SignHelper = new SignHelper.SignHelper();

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="mch_id">商户号</param>
        /// <param name="device_info">设备号</param>
        /// <param name="APIKey">API密钥</param>
        public WeiXinHelper(string appid, string mch_id, string device_info, string APIKey)
        {
            this.appid = appid;
            this.mch_id = mch_id;
            this.device_info = device_info;
            this.APIKey = APIKey;
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        string GetSign<T>(T model)
        {
            return WX_EncryptionHelper.Encrypt(SafeHelper.EncryptionHelper.InOutParaType.Str16, SafeHelper.EncryptionHelper.Encrypt_Type.MD5, SignHelper.SignHelper.GetStringSignTemp(model, APIKey, bindingAttr: System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        WXReturn WeiXinResStrToWXReturn<T>(string ResStr) where T : WXReturn
        {
            WXReturn WXReturnModel = WXConvert.GetWXObject<WXReturn>(ResStr);
            if (WXReturnModel.return_code_enum == WXReturn.ReturnCode.SUCCESS)
            {
                WXReturn_Success WXReturn_SuccessModel = WXConvert.GetWXObject<WXReturn_Success>(ResStr);
                if (WXReturn_SuccessModel.result_code_enum == WXReturn.ReturnCode.SUCCESS)
                {
                    string sign = WXReturn_SuccessModel.sign;
                    T Tmodel = WXConvert.GetWXObject<T>(ResStr);
                    if (sign == GetSign(Tmodel))
                    {
                        return Tmodel;
                    }
                    else
                    {
                        WXReturnModel.return_code = Enum.GetName(typeof(WXReturn.ReturnCode), WXReturn.ReturnCode.FAIL);
                        WXReturnModel.return_msg = "签名错误";
                    }
                }
                return WXReturn_SuccessModel;
            }
            return WXReturnModel;
        }
        
        /// <summary>
        /// 调用微信接口
        /// </summary>
        WXReturn WeiXinFunc<T, T1>(string RequestUrl, T1 WXPara_Model) where T: WXReturn where T1 : WXPara
        {
            WXPara_Model.appid = appid;
            WXPara_Model.mch_id = mch_id;
            WXPara_Model.device_info = device_info;
            WXPara_Model.nonce_str = THelper.GetRandomString(32);
            WXPara_Model.sign = GetSign(WXPara_Model);

            HttpStatusCode OpStatusCode = HttpStatusCode.ExpectationFailed;
            string OpStatusDescription = null;

            try
            {
                string ResStr = RequestHelper.HttpRequest(WebHelper.WebHelper.RequestType.Post, out OpStatusCode, out OpStatusDescription, RequestUrl, WXConvert.ToXmlString(WXPara_Model, false));
                if (OpStatusCode == HttpStatusCode.OK)
                {
                    return WeiXinResStrToWXReturn<T>(ResStr);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return default(T);
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        public object unifiedorder(WXP_unifiedorder WXP_unifiedorder_Model)
        {
            string RequestUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            WXReturn WXReturnModel = WeiXinFunc<WXR_unifiedorder, WXP_unifiedorder>(RequestUrl, WXP_unifiedorder_Model);
            if (WXReturnModel is WXR_unifiedorder)
            {
                WXR_unifiedorder WXR_unifiedorderModel = WXReturnModel as WXR_unifiedorder;
                WXP_apprequest WXP_apprequestModel = new WXP_apprequest();
                WXP_apprequestModel.appid = this.appid;
                WXP_apprequestModel.partnerid = this.mch_id;
                WXP_apprequestModel.prepayid = WXR_unifiedorderModel.prepay_id;
                WXP_apprequestModel.package = "Sign=WXPay";
                WXP_apprequestModel.noncestr = THelper.GetRandomString(32);
                WXP_apprequestModel.timestamp = THelper.GetTimestamp().ToString();
                WXP_apprequestModel.sign = GetSign(WXP_apprequestModel);
                return WXP_apprequestModel;
            }
            return WXReturnModel;
        }

        /// <summary>
        /// 支付结果通知
        /// </summary>
        public WXReturn paynotify(string paynotify_xml, out WXReturn WXReturnModel_notify_return)
        {
            WXReturnModel_notify_return = new WXReturn();
            WXReturn WXReturnModel = WeiXinResStrToWXReturn<WXR_paynotify>(paynotify_xml);
            if (WXReturnModel != null)
            {
                if (WXReturnModel.return_code_enum == WXReturn.ReturnCode.SUCCESS)
                {
                    WXR_paynotify WXR_paynotifyModel = WXReturnModel as WXR_paynotify;
                    if (WXR_paynotifyModel.sign == GetSign(WXR_paynotifyModel))
                    {
                        string[] ProNames = { "coupon_id_", "coupon_fee_" };
                        Type WXR_paynotify_Type = typeof(WXR_paynotify);
                        foreach (string ProName in ProNames)
                        {
                            try
                            {
                                System.Text.RegularExpressions.GroupCollection Groups = System.Text.RegularExpressions.Regex.Match(paynotify_xml, @"<" + ProName + @"(\d*)>" + WXReturn.WXReturnPattern.Replace("{1}", "(.*)") + @"</" + ProName + @"\d*>").Groups;
                                if (Groups.Count > 2)
                                {
                                    switch (ProName)
                                    {
                                        case "coupon_id_":
                                            WXR_paynotify_Type.GetProperty(ProName + "n").SetValue(WXR_paynotifyModel, new KeyValuePair<int, string>(Convert.ToInt32(Groups[1].Value), Groups[2].Value), null);
                                            break;
                                        case "coupon_fee_":
                                            WXR_paynotify_Type.GetProperty(ProName + "n").SetValue(WXR_paynotifyModel, new KeyValuePair<int, int>(Convert.ToInt32(Groups[1].Value), Convert.ToInt32(Groups[2].Value)), null);
                                            break;
                                    }
                                }
                            }
                            catch
                            {
                                WXReturnModel_notify_return.return_code = Enum.GetName(typeof(WXReturn.ReturnCode), WXReturn.ReturnCode.FAIL);
                                WXReturnModel_notify_return.return_msg = "参数格式校验错误";
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(WXReturnModel_notify_return.return_code))
                        {
                            WXReturnModel_notify_return.return_code = Enum.GetName(typeof(WXReturn.ReturnCode), WXReturn.ReturnCode.SUCCESS);
                            WXReturnModel_notify_return.return_msg = "OK";
                        }
                    }
                    else
                    {
                        WXReturnModel_notify_return.return_code = Enum.GetName(typeof(WXReturn.ReturnCode), WXReturn.ReturnCode.FAIL);
                        WXReturnModel_notify_return.return_msg = "签名错误";
                    }
                }
                else
                {
                    WXReturnModel_notify_return.return_code = WXReturnModel.return_code;
                    WXReturnModel_notify_return.return_msg = WXReturnModel.return_msg;
                }
            }
            else
            {
                WXReturnModel_notify_return.return_code = Enum.GetName(typeof(WXReturn.ReturnCode), WXReturn.ReturnCode.FAIL);
                WXReturnModel_notify_return.return_msg = "参数格式校验错误";
            }

            return WXReturnModel;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        public WXReturn orderquery(WXP_orderquery WXP_unifiedorder_Model)
        {
            string RequestUrl = "https://api.mch.weixin.qq.com/pay/orderquery";
            return WeiXinFunc<WXR_orderquery, WXP_orderquery>(RequestUrl, WXP_unifiedorder_Model);
        }
    }
}
