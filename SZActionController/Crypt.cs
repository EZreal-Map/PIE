using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using cn.ctgu.webgis.Utils;
//using cn.scu.DamMonitor.Action.Controller;
using System.Configuration;
using System.Xml;
using System.Web.Script;

namespace cn.ctgu.webgis.WebService
{
    public class Crypt
    {
        private Crypt()
        {
        }
        private static byte[] _key1 = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        #region 加密解密
        /// <summary>  
        /// 压缩字符串  
        /// </summary>  
        /// <param name="strUncompressed">未压缩的字符串</param>  
        /// <returns>压缩的字符串</returns>  
        public static string StringCompress(DataSet strUncompressed, string myKey)
        {
            byte[] bytData = GetBinaryFormatData(strUncompressed);  //System.Text.Encoding.Unicode.GetBytes(strUncompressed);
            byte[] dataCompressed = CompressBytes(bytData);
            //加密
            byte[] dataEncrypt = Encrypt(dataCompressed, myKey);
            return System.Convert.ToBase64String(dataEncrypt, 0, dataEncrypt.Length);

        }
        /// <summary>  
        /// 压缩字符串  
        /// </summary>  
        /// <param name="strUncompressed">未压缩的字符串</param>  
        /// <returns>压缩的字符串</returns>  
        public static string StringCompress(String strUncompressed, string myKey)
        {
            byte[] bytData = System.Text.Encoding.UTF8.GetBytes(strUncompressed);
            //压缩
            byte[] dataCompressed = CompressBytesBytream(bytData);//  CompressBytes(bytData);//
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
        private static byte[] CompressBytes(byte[] SourceByte)
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
        private static byte[] GZipCompressBytes(byte[] SourceByte)
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
        private static Stream CompressStream(Stream SourceStream)
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
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DeEncrypt(string inputString, string key)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);//将密匙转换成二进制数组
            byte[] byIV = byKey;//将初始初始向量转换成二进制数据

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(inputString);//将加密的文字转换成数组
            }
            catch
            {
                return null;
            }
            MemoryStream ms = new MemoryStream(byEnc);//将内存流接到上面去
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();//创建des加密算法执行类

            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);//解密流接到内存流上
            StreamReader sr = new StreamReader(cst);//文本读取流
            return sr.ReadToEnd();//加密的文字转化过来的数组------>内存流---------->解密流------------>原始文字
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

        #endregion

    }
}