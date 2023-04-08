using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Reflection;//这个是用反射的类库
namespace SZRiverSys.JSON
{
    /// <summary>
    /// class_memo:服务器缓存操作类 
    /// create_user:王铄文
    /// create_time:2018-3-20
    /// </summary>
    public static class JsonConvertHelper
    {
        #region
        /// <summary>
        /// 泛型反射赋值
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : class,new()
        {
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)//遍历原数据每行
            {
                T model = new T();
                foreach (var property in model.GetType().GetProperties())//反射寻找对象的每个字段
                {
                    if (dt.Columns.Contains(property.Name))
                    {
                        property.SetValue(model, dr[property.Name] == null ? "" : dr[property.Name], null);//赋值给对象中的字段
                    }
                }
                list.Add(model);
            }
            return list;
        }


        /// <summary>
        /// 序列化Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="drArray"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataRow[] drArray) where T : class,new()
        {

            List<T> list = new List<T>();
            DataTable dt = drArray[0].Table;
            foreach (DataRow item in drArray)
            {
                T model = new T();//实例化对象装datarow的数据
                //获取实体类的所有字段

                //typeof(T).GetProperties()-- model.GetType().GetProperties() 这两种写法一样的 此写法也是一样的道理 PropertyInfo[] 字段 = typeof(T).GetProperties();

                foreach (var property in model.GetType().GetProperties())//反射寻找对象的每个字段，找的就是T这个对象的字段
                {
                    if (dt.Columns.Contains(property.Name))
                    {
                        property.SetValue(model, item[property.Name], null);//赋值给对象中的字段
                        //字段.SetValue(要往那个对象中赋值, 值, null); 
                    }
                }
                list.Add(model);
            }
            return list;
        }

        public static string ToJson<T>(this DataTable obj) where T : class,new()
        {
            return JsonConvert.SerializeObject(obj.ToList<T>());//序列化对象集合
        }
        #endregion
    }
}