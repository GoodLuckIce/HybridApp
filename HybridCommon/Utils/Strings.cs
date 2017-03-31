using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HybridCommon.Utils
{
    public static class Strings
    {
        /// <summary>
        /// 密码加密，三次加密，加密顺序：MD5，SHA1，MD5
        /// </summary>
        /// <param name="text">需要加密的密码</param>
        /// <returns></returns>
        public static string PasswordEncrypt(string text)
        {

            return Cryptography.MD5Encrypt(Cryptography.SHA1Encrypt(Cryptography.MD5Encrypt(text)));
        }

        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }

        /// <summary>
        /// 建立密文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CreateToken(string text)
        {
            return Cryptography.MD5Encrypt("hibruin" + text + "@outlook" + "#lite");
        }

        /// <summary>
        /// 将字段key-value形式数据组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildUrlQuery(IDictionary<string, string> parameters)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");
                    //postData.Append(value);//在没有找到asp.net 5中如何URL编码之前直接输出
                    postData.Append(WebUtility.UrlEncode(value));
                    //postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 组装GET请求URL。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>带参数的GET请求URL</returns>
        public static string BuildGetUrl(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + BuildUrlQuery(parameters);
                }
                else
                {
                    url = url + "?" + BuildUrlQuery(parameters);
                }
            }
            return url;
        }

        /// <summary>
        /// 对URL参数进行切割，并拼装成字典
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IDictionary<string, string> SplitUrlQuery(string query)
        {
            query = query.Trim(new char[] { '?', ' ' });
            if (query.Length == 0)
            {
                return new Dictionary<string, string>();
            }
            IDictionary<string, string> result = new Dictionary<string, string>();

            string[] pairs = query.Split(new char[] { '&' });
            if (pairs != null && pairs.Length > 0)
            {
                foreach (string pair in pairs)
                {
                    string[] oneParam = pair.Split(new char[] { '=' }, 2);
                    if (oneParam != null && oneParam.Length == 2)
                    {
                        result.Add(oneParam[0], oneParam[1]);
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// 验证字符串是否为手机号码（返回true是手机号码，返回false不是）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsMobile(this string str)
        {
            //if (Regex.IsMatch(str, @"^[1]+[3,4,5,7,8]+\d{9}"))
            if (Regex.IsMatch(str, @"^(13[0-9]|14[0-9]|15[0-9]|16[0-9]|17[0-9]|18[0-9])\d{8}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证字符串是否为电话号码（返回true是电话号码，返回false不是）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPhone(this string str)
        {
            if (Regex.IsMatch(str, @"^(\d{4}-?)?\d{7,8}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将字符串转换成可空的基础类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Nullable<T> TryParseTo<T>(this string s) where T : struct
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            if (typeof(T) == typeof(System.Guid))
            {
                return TryParseToGuid(s) as Nullable<T>;
            }

            object obj = null;
            try
            {
                obj = Convert.ChangeType(s, typeof(T));
            }
            catch (Exception)
            {
                return null;
            }

            return obj as Nullable<T>;
        }

        static Guid? TryParseToGuid(string s)
        {
            Guid temp;
            if (Guid.TryParse(s, out temp))
            {
                return temp;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取字符串字节长度
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetBytesLength(string input)
        {
            return System.Text.Encoding.Default.GetBytes(input.ToCharArray()).Length;
        }

        /// <summary>
        /// 获取字符串字节长度,中文算两个字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetChBytesLength(string input)
        {
            char[] ch = input.ToCharArray();

            int varlength = 0;
            for (int i = 0; i < ch.Length; i++)
            {
                // changed by zyf 0825 , bug 6918，加入中文标点范围 ， TODO 标点范围有待具体化
                if ((ch[i] >= 0x2E80 && ch[i] <= 0xFE4F) || (ch[i] >= 0xA13F && ch[i] <= 0xAA40) || ch[i] >= 0x80)
                { // 中文字符范围0x4e00 0x9fbb
                    varlength = varlength + 2;
                }
                else
                {
                    varlength++;
                }
            }
            return varlength;
        }

        /// <summary>
        /// 判断输入的字符串只包含汉字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChineseCh(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex("^[\u4e00-\u9fa5]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断是否为正确港口(只能包含中文,-,英文字符)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsSeaPort(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex("^([\u4e00-\u9fa5]+)|(-+)|(\\w+)$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串只包含汉字或英文和括号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChineseOrEnglishCh(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex(@"^([a-z]+)|([\u4e00-\u9fa5]+([\(（][\u4e00-\u9fa5]+[\)）])?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex.IsMatch(input);
        }



        /// <summary>
        /// 判断输入的字符串只包含数字
        /// 可以匹配整数和浮点数
        /// ^-?\d+$|^(-?\d+)(\.\d+)?$
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 匹配非负整数
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotNagtive(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex(@"^\d+$");
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 匹配正整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsUint(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex("^[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 匹配非负小数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPositiveFloat(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex("^\\d+(\\.\\d+)?$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串字包含英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEnglisCh(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex("^[A-Za-z]+$");
            return regex.IsMatch(input);
        }


        /// <summary>
        /// 判断输入的字符串是否是一个合法的Email地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            if (input.Length < 6 || input.Length > 30) return false;
            const string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>
        /// 判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            const string pattern = @"^[A-Za-z0-9]+$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串只包含汉字或英文和括号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChineseOrEnglishOrNumberCh(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex(@"^[\u4e00-\u9fa5A-Za-z0-9]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串只包含汉字或英文
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsChineseOrEnglish(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var regex = new Regex(@"^[\u4e00-\u9fa5A-Za-z]+$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串是否是一个超链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsURL(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            //string pattern = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            const string pattern = @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>
        /// 判断输入的字符串是否是表示一个IP地址
        /// </summary>
        /// <param name="input">被比较的字符串</param>
        /// <returns>是IP地址则为True</returns>
        public static bool IsIPv4(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var IPs = input.Split('.');
            var regex = new Regex(@"^\d+$");
            for (var i = 0; i < IPs.Length; i++)
            {
                if (!regex.IsMatch(IPs[i]))
                {
                    return false;
                }
                if (Convert.ToUInt16(IPs[i]) > 255)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 计算字符串的字符长度，一个汉字字符将被计算为两个字符
        /// </summary>
        /// <param name="input">需要计算的字符串</param>
        /// <returns>返回字符串的长度</returns>
        public static int GetCount(string input)
        {
            return Regex.Replace(input, @"[\u4e00-\u9fa5/g]", "aa").Length;
        }

        /// <summary>
        /// 调用Regex中IsMatch函数实现一般的正则表达式匹配
        /// </summary>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>
        public static bool IsMatch(string pattern, string input)
        {
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="replacement">用于替换的字符串</param>
        /// <returns>返回被替换后的结果</returns>
        public static string Replace(string pattern, string input, string replacement)
        {
            var regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        /// <summary>
        /// 在由正则表达式模式定义的位置拆分输入字符串。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static string[] Split(string pattern, string input)
        {
            var regex = new Regex(pattern);
            return regex.Split(input);
        }
        /// <summary>
        /// 判断输入的字符串是否是合法的IPV6 地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIPV6(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            var pattern = "";
            var temp = input;
            var strs = temp.Split(':');
            if (strs.Length > 8)
            {
                return false;
            }
            var count = GetStringCount(input, "::");
            if (count > 1)
            {
                return false;
            }
            else if (count == 0)
            {
                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";

                var regex = new Regex(pattern);
                return regex.IsMatch(input);
            }
            else
            {
                pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                var regex1 = new Regex(pattern);
                return regex1.IsMatch(input);
            }

        }
        /* *******************************************************************
        * 1、通过“:”来分割字符串看得到的字符串数组长度是否小于等于8
        * 2、判断输入的IPV6字符串中是否有“::”。
        * 3、如果没有“::”采用 ^([\da-f]{1,4}:){7}[\da-f]{1,4}$ 来判断
        * 4、如果有“::” ，判断"::"是否止出现一次
        * 5、如果出现一次以上 返回false
        * 6、^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$
        * ******************************************************************/
        /// <summary>
        /// 判断字符串compare 在 input字符串中出现的次数
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="compare">用于比较的字符串</param>
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>
        private static int GetStringCount(string input, string compare)
        {
            var index = input.IndexOf(compare);
            if (index != -1)
            {
                return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 生成指定数量的HTML空格符号
        /// </summary>
        /// <param name="spacesCount">空格数量</param>
        /// <returns></returns>
        public static string GetSpacesString(int spacesCount)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < spacesCount; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 当传至为null值时转为字符空值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NullAsEmpty(string text)
        {
            return text ?? string.Empty;
        }

        /// <summary>
        /// 当传null对象时转为字符空值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NullAsEmpty(object obj)
        {
            if (obj != null)
                return obj.ToString();
            return string.Empty;
        }

        /// <summary>
        /// 查询是否包含特殊字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool FoundSymbol(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            const string symbols = @"~`!@#$%^&*-+=|\/?,.:;'""()<>[]{}";
            return symbols.Any(t => input.IndexOf(t) != -1);
        }

        /// <summary>
        /// 查询是否包含字符!@#$%^
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool FoundSymbolSimple(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            const string symbols = @"!@#$%^";
            return symbols.Any(t => input.IndexOf(t) != -1);
        }

        /// <summary>
        /// 将字符串中的HTML符号转换成编码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TextEncode(string text)
        {
            var sb = new StringBuilder(text);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&#39;");
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串中的HTML编码转换成HTML符号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TextDecode(string text)
        {
            var sb = new StringBuilder(text, text.Length);
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&quot;", "\"");
            sb.Replace("&#39;", "'");
            sb.Replace("&amp;", "&");
            return sb.ToString();
        }
        /// <summary>
        /// 对HTML代码编码转换
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlEncode(string text)
        {
            var sb = new StringBuilder(text);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("\'", "&#39;");
            sb.Replace("\t", "&nbsp; &nbsp; ");
            sb.Replace("\r", "");
            sb.Replace("\n", "<br />");
            return sb.ToString();
        }
        /// <summary>
        /// HTML代码还原
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlDecode(string text)
        {
            var sb = new StringBuilder(text);
            sb.Replace("<br />", "\r\n");
            sb.Replace("<p></p>", "\r\n\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&quot;", "\"");
            sb.Replace("&#39;", "'");
            sb.Replace("&nbsp;&nbsp;&nbsp;&nbsp;", "\t");
            sb.Replace("&nbsp; &nbsp; ", "\t");
            sb.Replace("&amp;", "&");
            sb.Replace("&ldquo;", "“");
            sb.Replace("&rdquo;", "”");
            return sb.ToString();
        }

        /// <summary>
        /// 删除HTML标签代码
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveHtml(string text)
        {
            return Regex.Replace(text, @"<[a-zA-Z]+[\s\S]*>", "");
        }

        public static string RemoveHtmlComment(string text)
        {
            return Regex.Replace(text, @"<!--(.|\s)*?-->", "");
        }

        /// <summary>
        /// 对email字符串转换成HTML编码链接
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string EmailEncode(string email)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<a href=\"mailto:{0}\">{0}</a>", email);
            sb.Replace("@", "&#64;");
            sb.Replace(".", "&#46;");
            return sb.ToString();
        }


        /// <summary>
        /// 返回字符串真实长度，1汉字长度为2
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetStringLength(string text)
        {
            return Encoding.Default.GetBytes(text).Length;
        }

        public static string Left(string text, int characterCount)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            if (text.Length <= characterCount)
            {
                return text;
            }
            return text.Substring(0, characterCount);
        }


        /// <summary>
        /// 将URL参数拼接成字符串
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerationRequestUrl(Dictionary<string, string> parameters)
        {
            var equals = "=";
            var ampersand = "&";

            var sb = new StringBuilder();
            var first = true;
            foreach (var item in parameters)
            {

                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(ampersand);
                }
                sb.Append(item.Key).Append(equals).Append(item.Key);
            }
            return sb.ToString();
        }

        #region 产生唯一的由字母和数字组成的字符
        /// <summary>
        /// 产生唯一的由字母和数字组成的字符
        /// </summary>
        /// <returns></returns>
        public static string GenerateId()
        {
            long i = 1;
            foreach (var b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        #endregion

        /// <summary>
        /// 使用时间搓方式生成12位长度的数字
        /// </summary>
        /// <returns></returns>
        public static string GenerateNumber()
        {
            System.Threading.Thread.Sleep(1);
            var dt = DateTime.Now;
            var ts = new TimeSpan(dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            var sb = new StringBuilder(ts.TotalMilliseconds.ToString());
            var random = new Random();
            while (sb.Length < 12)
            {
                System.Threading.Thread.Sleep(1);
                sb.Append(random.Next(0, 9));
            }
            return sb.ToString();

        }

        /// <summary>
        /// 多个空格合并成一个
        /// </summary>
        /// <param name="orinStr"></param>
        /// <returns></returns>
        public static string CombineInsideSpaces(string orinStr)
        {
            var poses = new List<int>();
            for (var i = 0; i <= orinStr.Length - 1; i++)
            {//获取所有空格位置
                if (orinStr[i] == ' ')
                {
                    poses.Add(i);
                }
            }
            for (var i = poses.Count - 1; i > 0; i--)
            {//遍历每个空格位置,检查前位 是否未空格
                var cur = poses[i];
                var prev = poses[i - 1];
                if (prev == cur - 1)
                {
                    orinStr = orinStr.Remove(cur, 1);
                }
            }
            return orinStr;
        }

        /// 转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///
        public static string ToSBC(String input)
        {
            // 半角转全角：
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }

        /**/
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public static string ToDBC(String input)
        {
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }

        public static string FilterHTML(string html)
        {
            var regex1 =
                  new System.Text.RegularExpressions.Regex(@"<script[sS]+</script *>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex2 =
                  new System.Text.RegularExpressions.Regex(@" href *= *[sS]*script *:",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex3 =
                  new System.Text.RegularExpressions.Regex(@" no[sS]*=",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex4 =
                  new System.Text.RegularExpressions.Regex(@"<iframe[sS]+</iframe *>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex5 =
                  new System.Text.RegularExpressions.Regex(@"<frameset[sS]+</frameset *>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex6 =
                  new System.Text.RegularExpressions.Regex(@"<img[^>]+>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex7 =
                  new System.Text.RegularExpressions.Regex(@"</p>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex8 =
                  new System.Text.RegularExpressions.Regex(@"<p>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var regex9 =
                  new System.Text.RegularExpressions.Regex(@"<[^>]*>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记 
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性 
            html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件 
            html = regex4.Replace(html, ""); //过滤iframe 
            html = regex5.Replace(html, ""); //过滤frameset 
            html = regex6.Replace(html, ""); //过滤frameset 
            html = regex7.Replace(html, ""); //过滤frameset 
            html = regex8.Replace(html, ""); //过滤frameset 
            html = regex9.Replace(html, "");
            //html = html.Replace(" ", "");
            html = html.Replace("</strong>", "");
            html = html.Replace("<strong>", "");
            html = Regex.Replace(html, "[\f\n\r\t\v]", "");  //过滤回车换行制表符
            return html;
        }


        #region   检查密码强度

        /// <summary> 
        /// 检查字符是属于哪一类 
        /// </summary> 
        /// <param name="_in"></param> 
        /// <returns></returns> 
        private static int CharMode(int _in)
        {
            if (_in >= 48 && _in <= 57) // 数字   
                return 1;
            if (_in >= 65 && _in <= 90) // 大写字母   
                return 2;
            if (_in >= 97 && _in <= 122) // 小写   
                return 4;
            return 8; // 特殊字符   
        }

        /// <summary> 
        /// 计算出当前密码当中一共有多少种模式 
        /// </summary> 
        /// <param name="num"></param> 
        /// <returns></returns> 
        private static int BitTotal(int num)
        {
            var modes = 0;
            for (var i = 0; i < 4; i++)
            {
                // 逐位运算 
                if ((num & 1) == 0)
                {
                    modes++;
                }
                // 右移位运算 
                num >>= 1;
            }
            return modes;
        }


        /// <summary> 
        /// 检查密码字符串强度，返回密码的强度级别 
        /// </summary> 
        /// <param name="pwd"></param>
        /// <returns>1=强,2=中,3=弱</returns> 
        public static int CheckStrong(string pwd)
        {
            // 判断密码长度是否大于6位 
            if (pwd.Length <= 6)
            {
                return 3; // 密码太短   
            }
            var modes = 0;
            for (var i = 0; i < pwd.Length; i++)
            {
                // 获取密码字符串字节数组 
                var byPwd = Encoding.Default.GetBytes(pwd);
                // 或赋值运算 
                modes |= CharMode(byPwd[i]);
            }
            return BitTotal(modes);
        }
        #endregion


        /// <summary>
        /// 字符串数组转成int数组
        /// </summary>
        /// <param name="strArr"></param>
        /// <returns></returns>
        public static int[] TransStringArrToIntArr(string[] strArr)
        {
            List<int> data = new List<int>();
            for (int i = 0; i < strArr.Length; i++)
            {
                data.Add(Convert.ToInt32(strArr[i]));
            }
            return data.ToArray();
        }


    }
}
