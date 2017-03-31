using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HybridCommon.Utils
{
    /// <summary>
    /// 转换属性对象
    /// </summary>
    public class CastProperty
    {
        public PropertyAccessorHandler SourceProperty
        {
            get;
            set;
        }

        public PropertyAccessorHandler TargetProperty
        {
            get;
            set;
        }
    }


    /// <summary>
    /// 属性访问器
    /// </summary>
    public class PropertyAccessorHandler
    {

        public PropertyAccessorHandler(PropertyInfo propInfo)
        {
            this.PropertyName = propInfo.Name;
            if (propInfo.CanRead)
                this.Getter = propInfo.GetValue;

            if (propInfo.CanWrite)
                this.Setter = propInfo.SetValue;
        }
        public string PropertyName { get; set; }
        public Func<object, object[], object> Getter { get; private set; }
        public Action<object, object, object[]> Setter { get; private set; }
    }

    /// <summary>
    /// 属性转换类，将一个类的属性值转换给另外一个类的同名属性，注意该类使用的是浅表复制。
    /// <example>
    ///        下面几种用法一样:
    ///        Mapping.GetCast(typeof(CarInfo), typeof(ImplCarInfo)).Cast(info, ic);
    ///        Mapping.CastObject《CarInfo, ImplCarInfo》(info, ic);
    ///        Mapping.CastObject(info, ic);
    ///
    ///        ImplCarInfo icResult= info.CopyTo《ImplCarInfo》(null);
    ///
    ///        ImplCarInfo icResult2 = new ImplCarInfo();
    ///        info.CopyTo《ImplCarInfo》(icResult2);
    /// 
    /// </example>
    /// </summary>
    public class Mapping
    {
        private List<CastProperty> mProperties = new List<CastProperty>();

        static Dictionary<Type, Dictionary<Type, Mapping>> mCasters = new Dictionary<Type, Dictionary<Type, Mapping>>(256);

        private static Dictionary<Type, Mapping> GetModuleCast(Type sourceType)
        {
            Dictionary<Type, Mapping> result;
            lock (mCasters)
            {
                if (!mCasters.TryGetValue(sourceType, out result))
                {
                    result = new Dictionary<Type, Mapping>(8);
                    mCasters.Add(sourceType, result);
                }
            }
            return result;
        }

        /// <summary>
        /// 将源类型的属性值转换给目标类型同名的属性
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Cast(object source, object target)
        {
            Cast(source, target, null);
        }

        /// <summary>
        /// 将源类型的属性值转换给目标类型同名的属性，排除要过滤的属性名称
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="filter">要过滤的属性名称</param>
        public void Cast(object source, object target, string[] filter)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (target == null)
                throw new ArgumentNullException("target");

            for (int i = 0; i < mProperties.Count; i++)
            {
                CastProperty cp = mProperties[i];

                if (cp.SourceProperty.Getter != null)
                {
                    object Value = cp.SourceProperty.Getter(source, null); //PropertyInfo.GetValue(source,null);
                    if (cp.TargetProperty.Setter != null)
                    {
                        if (filter == null)
                        {
                            cp.TargetProperty.Setter(target, Value, null);
                        }
                        else if (!filter.Contains(cp.TargetProperty.PropertyName))
                        {
                            cp.TargetProperty.Setter(target, Value, null);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 获取要转换的当前转换类实例
        /// </summary>
        /// <param name="sourceType">要转换的源类型</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static Mapping GetCast(Type sourceType, Type targetType)
        {
            Dictionary<Type, Mapping> casts = GetModuleCast(sourceType);
            Mapping result;
            lock (casts)
            {
                if (!casts.TryGetValue(targetType, out result))
                {
                    result = new Mapping(sourceType, targetType);
                    casts.Add(targetType, result);
                }
            }
            return result;
        }

        /// <summary>
        /// 以两个要转换的类型作为构造函数，构造一个对应的转换类
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        public Mapping(Type sourceType, Type targetType)
        {
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo sp in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (PropertyInfo tp in targetProperties)
                {
                    if (sp.Name == tp.Name)
                    {
                        if (sp.PropertyType == tp.PropertyType)
                        {
                            CastProperty cp = new CastProperty();
                            cp.SourceProperty = new PropertyAccessorHandler(sp);
                            cp.TargetProperty = new PropertyAccessorHandler(tp);
                            mProperties.Add(cp);
                            break;
                        }
                        else
                        {
                            //关于Nullable<T>跟T类型对比
                            //string nullableName = typeof (Nullable<>).Name;

                            Type spType = (typeof(Nullable<>).Name == sp.PropertyType.Name) ? sp.PropertyType.GetProperties()[1].PropertyType : sp.PropertyType;

                            Type tpType = (typeof(Nullable<>).Name == tp.PropertyType.Name) ? tp.PropertyType.GetProperties()[1].PropertyType : tp.PropertyType;

                            if (spType == tpType)
                            {
                                CastProperty cp = new CastProperty();
                                cp.SourceProperty = new PropertyAccessorHandler(sp);
                                cp.TargetProperty = new PropertyAccessorHandler(tp);
                                mProperties.Add(cp);
                                break;
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        public static void CastObject<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            Mapping.GetCast(typeof(TSource), typeof(TTarget)).Cast(source, target);
        }
    }

    public static class Mapper
    {
        /// <summary>
        /// 将当前对象的属性值复制到目标对象，使用浅表复制
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象，如果为空，将生成一个</param>
        /// <param name="filter">要过滤的属性名称数组</param>
        /// <returns>复制过后的目标对象</returns>
        public static T CopyTo<T>(this object source, T target, string[] filter) where T : class,new()
        {
            if (source == null)
            {
                if (target == null)
                {
                    return null;
                }
                else
                {
                    return target;
                }
            }
            else
            {
                if (target == null)
                {
                    target = new T();
                }
            }
            Mapping.GetCast(source.GetType(), typeof(T)).Cast(source, target, filter);
            return target;
        }

        /// <summary>
        /// 拷贝当前对象的属性值到目标对象上
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="source">当前对象</param>
        /// <param name="target">目标对象</param>
        /// <returns>返回赋值过后的目标对象</returns>
        public static T CopyTo<T>(this object source, T target) where T : class,new()
        {
            return source.CopyTo<T>(target, null);
        }

        public static T CopyTo<T>(this object source, params object[] sources) where T : class, new()
        {
            var temp = source.CopyTo<T>(null, null);
            foreach (var item in sources)
            {
                temp = item.CopyTo(temp, null);
            }
            return temp;
        }

        /// <summary>
        /// 拷贝当前对象的属性值到目标对象上
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="source">当前对象</param>
        /// <returns>返回赋值过后的目标对象</returns>
        public static T CopyTo<T>(this object source) where T : class, new()
        {
            return source.CopyTo<T>(null, null);
        }

    }


}
