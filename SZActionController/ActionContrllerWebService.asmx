﻿<%@ WebService Language="C#" Class="cn.ctgu.webgis.WebService.ActionControllerWebService" %>

using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
//using log4net;
using cn.ctgu.webgis.Utils;
using cn.ctgu.webgis.Config;
using cn.ctgu.webgis.Action.Controller;
using cn.ctgu.webgis.Base;
using cn.ctgu.webgis.Accounts.Business;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Web.Script;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Net;
/// <summary>
/// ActionControllerWebService 的摘要说明
/// 在此构建工作流逻辑（初始化、获取实例，建立连接等工作）
/// </summary>

namespace cn.ctgu.webgis.WebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    public class ActionControllerWebService : System.Web.Services.WebService
    {

        private const string ACTION_ASSEMBLY_NAME = "cn.ctgu.webgis.Action";
        private const string ACTION_PARM_KEY = "action";
        private const string DISPATCH_PARM_KEY = "dispatch";
        private const string ERROR_KEY = "WEBGISErrors";
        private static byte[] _key1 = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private string myKey = "scusztda";  //密钥-8位，密钥可以更改为文件形式存在服务器上web.config文件中

        public ActionControllerWebService()
        {
            //CODEGEN: 该调用是 ASP.NET Web 服务设计器所必需的
            InitializeComponent();
        }

        #region 组件设计器生成的代码

        //Web 服务设计器所必需的
        private IContainer components = null;

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        [WebMethod(EnableSession = true)]
        public string[] Run(string language, string[] names, string[] values)
        {
            //调用日志组件
            log4net.ILog logger = log4net.LogManager.GetLogger(this.GetType());
            //logger.Debug("debug");

            //解析返回调用数据
            ResultSetUtils ret = new ResultSetUtils(language);

            // log4：开始
            // 检查用户是否已经登录利用Siteprincipal
            SitePrincipal userInfo = Session["UserInfo"] as SitePrincipal;
            // log4：结束

            try
            {
                SiteIdentity identity;
                // 获取页面传来的输入数据
                if (language != "json")
                {
                    identity = (SiteIdentity)userInfo.Identity;
                    myKey = FormsAuthentication.HashPasswordForStoringInConfigFile(identity.Password, "SHA1").Substring(0, 8);
                    names = StringsDeEncrypt(names);
                    values = StringsDeEncrypt(values);
                }
                // 获取页面传来的输入数据
                Properties inParams = new Properties();
                inParams.load(names, values);

                // 根据用户请求查询数据字典，找到相关配置信息
                string strAction = (string)inParams[ACTION_PARM_KEY];
                string strDispatch = (string)inParams[DISPATCH_PARM_KEY];

                ActionConfig actionConfig = PMSConfig.FindActionConfig(strAction);
                DispatchConfig dispatchConfig = actionConfig.FindDispatchConfig(strDispatch);

                //// 检查用户是否已经登录利用Siteprincipal
                //SitePrincipal userInfo = Session["UserInfo"] as SitePrincipal;

                if (userInfo == null && !Login.IsLogin(actionConfig.Name, dispatchConfig.Name))
                {
                    if (language != "json")
                       throw new ApplicationException("@SESSION_TIMEOUT");
                }

                // 预处理输入数据，将所有输入数据封装到form中
                FormConfig formConfig = PMSConfig.FindFormConfig(dispatchConfig.Form);
                DynamicForm form = new DynamicForm();

                Hashtable sessionData = new Hashtable();
                form.AddProperty("session", sessionData);
                sessionData.Add("UserInfo", userInfo);
                foreach (DictionaryEntry entry in formConfig.Properties)
                {
                    FormPropertyConfig prop = (FormPropertyConfig)entry.Value;
                    if (prop.Name.StartsWith("session."))
                    {
                        // 将Session中的指定数据添加到form中
                        string propName = prop.Name.Substring("session.".Length);
                        string prefix = "";
                        if (prop.Type != "" && inParams[prop.Type] != null)
                        {
                            prefix = (string)inParams[prop.Type] + ".";
                        }
                        sessionData.Add(propName, Session[prefix + propName]);
                    }
                    else
                    {
                        if (inParams.Contains(prop.Name))
                        {
                            // 将页面传来的指定输入数据添加到form中（所有输入将被转换为正确的数据类型）
                            form.AddProperty(prop.Name, ConvertBeanUtils.Convert(prop.Type, (string)inParams[prop.Name]));
                        }
                    }
                }

                // 获取ActionController，由它将用户请求转发到业务处理函数
                IController controller = (IController)Assembly.Load(ACTION_ASSEMBLY_NAME).CreateInstance(actionConfig.Type);
                Hashtable result = controller.excute(dispatchConfig, form);

                // 处理输出数据，将需要保存状态的数据添加到Session中
                if (result["session"] != null)
                {
                    foreach (DictionaryEntry entry in (Hashtable)result["session"])
                    {
                        Session[(string)entry.Key] = entry.Value;
                    }
                }
                result.Remove("session");

                // 将其它输出数据添加到结果集中
                foreach (DictionaryEntry entry in result)
                {
                    ret.AddResult(entry.Key.ToString(), entry.Value);
                }

                // 将用户ID添加到结果集中
                if (language != "json")
                {
                    userInfo = (Session["UserInfo"] == null ? null : (SitePrincipal)Session["UserInfo"]);
                    if (userInfo != null)
                    {
                        ret.AddResult("userId", ((SiteIdentity)userInfo.Identity).UserID);// ((SiteIdentity)userInfo.Identity).UserID);
                    }
                    else
                    {
                        ret.AddResult("userId", "");
                    }
                }
                //logger.Debug(userInfo.Identity.Name + "#" + Session["UserIP"].ToString().Trim() + "#" + dispatchConfig.Name);
            }
            catch (Exception e)
            {
                Exception ex = (e.InnerException == null) ? e : e.InnerException;
                ret.ErrorMessage = "在{" + ex.Source + "::" + ex.TargetSite.Name + "(...)}中发生异常：" + ex.Message;
                if (language != "json")
                {
                    logger.Error("在ActionControllerWebService中捕获到异常", e);
                }
                //logger.Error(userInfo.Identity.Name, e);
            }
            string[] retStr = ret.ResultSet;
            if (language != "json")
            {
                retStr[0] = StringCompress(retStr[0]);
            }
            
            return retStr;
        }
        [WebMethod(EnableSession = true)]
        public string RunLogin(string usercode, string password)
        {
            string result = "";
            try
            {
                string nickName;
                int userid;
                if (ValidateLogin(usercode, password, out nickName,out userid))
                {
                   result = "{\"success\":\"true\",\"msg\":\"" + nickName + "\",\"userid\":\"" + userid + "\"}";
                }
                else
                    result = "{\"success\":\"false\",\"msg\":\"用户名或密码错误\"}";
            }
            catch (Exception e)
            {
                Exception ex = (e.InnerException == null) ? e : e.InnerException;
                result = "{\"success\":\"false\",\"msg\":\"验证错误\"}"; 
            }
            return result;
        }
        [WebMethod(EnableSession = true)]
        public string RunSZWeather(string Type, string appid, string appKey,string startDate, string endDate)
        {
            string result = "";
            try
            {
                string url = "http://opendata.sz.gov.cn/api/339779363/1/service.xhtml";
                string spage = "10";
                string srow = "100";
                string context = appKey;
                StringBuilder data = new StringBuilder();
                data.Append("&page=" + spage);
                data.Append("&rows=" + srow);
                data.Append("&startDate=" + startDate);
                data.Append("&endDate=" + endDate);
                data.Append("&appKey=" + context);
                result = HttpGet(url, data.ToString());
            }
            catch (Exception e)
            {
                Exception ex = (e.InnerException == null) ? e : e.InnerException;
                //result = "{\"success\":\"false\",\"msg\":\"验证错误\"}";
            }
            return result;
        }
[WebMethod(EnableSession = true)]
        public string RunSZTyphoon(string Type, string code, string productType, string key)
        {
            string result = "";
            try
            {
                string url = "http://open.gd121.cn/share/getData.do";
                           
                StringBuilder data = new StringBuilder();
                data.Append("type=" + Type);
                data.Append("&code=" + code);
                data.Append("&productType=" + productType);
                data.Append("&key=" + key);
                result = HttpGet(url, data.ToString());
                //?type=json&code=440100&productType=TF&key=5092168d081cb9e54fe412139487cb50

            }
            catch (Exception e)
            {
                Exception ex = (e.InnerException == null) ? e : e.InnerException;
                //result = "{\"success\":\"false\",\"msg\":\"验证错误\"}";
            }
            return result;
        }
        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateLogin(string UserCode, string password, out string nickName,out int userId)
        {
            nickName = "";
            userId = 0;
            Login newLogin=new Login();
            //传递无加密的信息，返回无加密信息
            Hashtable ret = newLogin.getUserInfoTable(UserCode, password);
            if ((int)ret["GetUserInfo"] == 1)
            {
                // 将用户ID添加到结果集中
                // 处理输出数据，将需要保存状态的数据添加到Session中
                if (ret["session"] != null)
                {
                    foreach (DictionaryEntry entry in (Hashtable)ret["session"])
                    {
                        Session[(string)entry.Key] = entry.Value;
                    }
                }
                SitePrincipal userInfo = (Session["UserInfo"] == null ? null : (SitePrincipal)Session["UserInfo"]);
                nickName = userInfo.Identity.Name;
                SiteIdentity useriden =(SiteIdentity)userInfo.Identity;
                userId = useriden.UserID;
                return true;
            }
            else
            {
                return false;
            }
         }
        #region 获取树状菜单数据
        /// <summary>
        /// 获取树状菜单数据
        /// </summary>
        /// <param name="functionTag">树桩菜单识别符，如"监测信息;人工", "监测信息","异常查询","数据分析","分布图"</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public void GetJsonTree(string JsontreeID)
        {
            //string userInfo = Session["UserInfo"] as string;//传递用户信息
            // 检查用户是否已经登录利用Siteprincipal
            SitePrincipal userInfo = Session["UserInfo"] as SitePrincipal;
            if (userInfo == null)//如果没有登入
            {
                this.Context.Response.Write("{\"success\":\"false\",\"msg\":\"没有登入\"}");
                return;
            }
            //myKey = FormsAuthentication.HashPasswordForStoringInConfigFile(Config.userList[userInfo].ToString(), "MD5").Substring(0, 8);
            SiteIdentity identity = (SiteIdentity)userInfo.Identity;
            myKey = FormsAuthentication.HashPasswordForStoringInConfigFile(identity.Password, "SHA1").Substring(0, 8);
            JsontreeID = Crypt.DeEncrypt(JsontreeID, myKey);
            DeviceJsonTree Jsontree = new DeviceJsonTree();
            string tempResult = "{\"success\":\"true\",\"data\":\"" +
                Crypt.StringCompress(Jsontree.Jsontree(JsontreeID), myKey) + "\"}";
            this.Context.Response.Write(tempResult);
        }
        #endregion
        /// <summary>
        /// 对数组解密
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private string[] StringsDeEncrypt(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                string queryQ = Crypt.DeEncrypt(values[i], myKey);
                values[i] = queryQ;
            }
            return values;
        }
        /// <summary>  
        /// 压缩字符串  
        /// </summary>  
        /// <param name="strUncompressed">未压缩的字符串</param>  
        /// <returns>压缩的字符串</returns>  
        private string StringCompress(DataSet strUncompressed)
        {
            byte[] bytData = GetBinaryFormatData(strUncompressed);  //System.Text.Encoding.Unicode.GetBytes(strUncompressed);
            byte[] dataCompressed = CompressBytes(bytData);
            //return System.Convert.ToBase64String(dataCompressed, 0, dataCompressed.Length);
            //加密
            byte[] dataEncrypt = Encrypt(dataCompressed, myKey);
            return System.Convert.ToBase64String(dataEncrypt, 0, dataEncrypt.Length);

        }
        /// <summary>  
        /// 压缩字符串  
        /// </summary>  
        /// <param name="strUncompressed">未压缩的字符串</param>  
        /// <returns>压缩的字符串</returns>  
        private string StringCompress(String strUncompressed)
        {
            byte[] bytData = System.Text.Encoding.UTF8.GetBytes(strUncompressed);
            //压缩
            byte[] dataCompressed = CompressBytesBytream(bytData);//  CompressBytes(bytData);//
            //return System.Convert.ToBase64String(dataCompressed, 0, dataCompressed.Length);
            //加密
            byte[] dataEncrypt = Encrypt(dataCompressed, myKey);
            return System.Convert.ToBase64String(dataEncrypt, 0, dataEncrypt.Length);
        }

        /// <summary>  
        /// 压缩二进制  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] CompressBytesBytream(byte[] str)
        {
            MemoryStream ms = new MemoryStream(str);
            ms.Position = 0;
            //{Position = 0};  
            MemoryStream outms = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(outms, CompressionMode.Compress, true))
            {
                byte[] buf = new byte[1024];
                int len;
                while ((len = ms.Read(buf, 0, buf.Length)) > 0)
                    deflateStream.Write(buf, 0, len);
            }
            return outms.ToArray();
        }

        /// <summary>
        /// 压缩byte数组数据
        /// </summary>
        /// <param name="SourceByte">需要被压缩的Byte数组数据</param>
        /// <returns></returns>
        private byte[] CompressBytes(byte[] SourceByte)
        {
            try
            {
                MemoryStream stmInput = new MemoryStream(SourceByte);
                Stream stmOutPut = CompressStream(stmInput);
                byte[] bytOutPut = new byte[stmOutPut.Length];
                stmOutPut.Position = 0;
                stmOutPut.Read(bytOutPut, 0, bytOutPut.Length);
                return bytOutPut;
            }
            catch
            {
                return null;
            }
        }



        private byte[] GZipCompressBytes(byte[] SourceByte)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                Stream s = new GZipStream(ms, CompressionMode.Compress);
                s.Write(SourceByte, 0, SourceByte.Length);
                s.Close();
                byte[] dataCompressed = (byte[])ms.ToArray();
                return dataCompressed;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 压缩流
        /// </summary>
        /// <param name="SourceStream">需要被压缩的流数据</param>
        /// <returns></returns>
        private Stream CompressStream(Stream SourceStream)
        {
            try
            {
                MemoryStream stmOutTemp = new MemoryStream();
                zlib.ZOutputStream outZStream = new zlib.ZOutputStream(stmOutTemp, zlib.zlibConst.Z_DEFAULT_COMPRESSION);
                CopyStream(SourceStream, outZStream);
                outZStream.finish();
                return stmOutTemp;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 拷贝复制流字节
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
        /// <summary>
        /// 将DataSet格式化成字节数组byte[]
        /// </summary>
        /// <param name="dsOriginal">DataSet对象</param>
        /// <returns>字节数组</returns>
        private static byte[] GetBinaryFormatData(DataSet dsOriginal)
        {
            byte[] binaryDataResult = null;
            MemoryStream memStream = new MemoryStream();
            IFormatter brFormatter = new BinaryFormatter();
            dsOriginal.RemotingFormat = SerializationFormat.Binary;
            brFormatter.Serialize(memStream, dsOriginal);
            binaryDataResult = memStream.ToArray();
            memStream.Close();
            memStream.Dispose();
            return binaryDataResult;
        }
        private static byte[] Encrypt(byte[] inputByteArray, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyIV = keyBytes;

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            MemoryStream memStream = new MemoryStream();
            CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            crypStream.Write(inputByteArray, 0, inputByteArray.Length);
            crypStream.FlushFinalBlock();
            //return Convert.ToBase64String(memStream.ToArray());
            byte[] bytOutPut = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(bytOutPut, 0, bytOutPut.Length);
            return bytOutPut;
        }
        /// <summary>
        /// AES加密算法
        /// </summary>
        /// <param name="plainText">明文字符串</param>
        /// <param name="strKey">密钥</param>
        /// <returns>返回加密后的密文字节数组</returns>
        public static byte[] AESEncrypt(byte[] inputByteArray, string strKey)
        {
            //分组加密算法

            SymmetricAlgorithm des = Rijndael.Create();
            //byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组
            //设置密钥及密钥向量
            des.Key = Encoding.UTF8.GetBytes(strKey);
            des.IV = _key1;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//得到加密后的字节数组
            cs.Close();
            ms.Close();
            return cipherBytes;
        }
       
       
    }
}
