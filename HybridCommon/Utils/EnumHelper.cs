using System;
using System.Collections.Generic;

namespace HybridCommon.Utils
{
    public class EnumHelper
    {
        public static Dictionary<int, string> GetDescription<T>()
        {
            var result = new Dictionary<int, string>();

            var t = typeof(T);
            var arrays = System.Enum.GetValues(t);
            foreach (var item in arrays)
            {
                var test = (T)item;
                var type = test.GetType();
                var fieldInfo = type.GetField(test.ToString());
                if (fieldInfo == null)
                {
                    var name = type.GetEnumNames().GetValue(0).ToString();
                    fieldInfo = type.GetField(name);
                }
                var attribArray = (object[])fieldInfo.GetCustomAttributes(false);
                if (attribArray.Length > 0)
                {
                    var attrib = (EnumDescriptionAttribute)attribArray[0];
                    result.Add((int)fieldInfo.GetValue(test), attrib.Description);
                }
            }
            return result;
        }

        public static string GetDescription(System.Enum em)
        {
            var type = em.GetType();
            var fieldInfo = type.GetField(em.ToString());
            if (fieldInfo == null)
            {
                return "";
            }
            var attribArray = fieldInfo.GetCustomAttributes(false);
            var attrib = (EnumDescriptionAttribute)attribArray[0];
            return attrib.Description;
        }
        
        public static int GetEnumByDescription<T>(string description)
        {
            var result = GetDescription<T>();
            if (result.ContainsValue(description))
            {
                foreach (var item in result)
                {
                    if (item.Value == description)
                    {
                        return item.Key;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 检测值中是否包含某成员值（用于以2的N次方定义的枚举操作）
        /// </summary>
        /// <param name="enumSum"></param>
        /// <param name="enumItemValue"></param>
        /// <returns></returns>
        public static bool EnumContains(int enumSum, int enumItemValue)
        {
            return (enumSum & enumItemValue) == enumItemValue;
        }
    }
    public class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
        public string Description { get; set; }
    }
}
