using System;
using System.Reflection;

namespace ShardingCore.Extensions
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 16 December 2020 13:59:46
* @Email: 326308290@qq.com
*/
    public static class ObjectExtension
    {
        private static readonly BindingFlags _bindingFlags
            = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
        
        /// <summary>
        /// 获取某字段值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static object GetTypeFieldValue(this Type type,object obj, string fieldName)
        {
            return type.GetField(fieldName, _bindingFlags).GetValue(obj);
        }
        /// <summary>
        /// 获取某字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static object GetFieldValue(this object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName, _bindingFlags).GetValue(obj);
        }
        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName, _bindingFlags);
            if (property != null)
            {
                return obj.GetType().GetProperty(propertyName, _bindingFlags)?.GetValue(obj);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取某字段值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetTypePropertyValue(this Type type,object obj, string propertyName)
        {
            var property=type.GetProperty(propertyName, _bindingFlags);
            if (property != null)
            {
                return type.GetProperty(propertyName, _bindingFlags)?.GetValue(obj);
            }
            else
            {
                return null;
            }
        }

    }
}