using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HybridCommon.Utils
{
    public class Cryptography
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string txt)
        {
            MD5 md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(txt))).Replace("-", "");
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string txt)
        {
            byte[] StrRes = Encoding.UTF8.GetBytes(txt);
            HashAlgorithm iSHA = SHA1.Create();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        /// <summary>
        /// 使用MD5加密进行签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string SignRequest(IDictionary<string, string> parameters, string secret)
        {
            ///把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            var query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }
            //MD5 md5 = MD5.Create();
            //byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            //// 第四步：把二进制转化为大写的十六进制
            //StringBuilder result = new StringBuilder();
            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    result.Append(bytes[i].ToString("X2"));
            //}

            //return result.ToString();
            return Cryptography.MD5Encrypt(query.ToString());
        }
    }
}
