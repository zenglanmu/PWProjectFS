using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWProjectFS.PWProvider
{

    /// <summary>
    /// 缓存类，用于pw接口数据的缓存。
    /// 因为文件系统可能短时间多次调用相同api
    /// 注意这个类不是线程安全的
    /// </summary>
    public class PWResourceCache
    {
        private int m_expireSeconds { get; set; }
        /* 全局性的缓存过期时间 */
        private Dictionary<string, object> m_cacheItems {get;set;}
        /* 保存实际缓存数据 */
        private Dictionary<string, DateTime> m_cacheTimes { get; set; }       
        /* 存储缓存的时间 */
        private Dictionary<string, int> m_cache_expireSeconds{ get; set; }
        /* 可单独给某缓存key设置过期时间 */

        public PWResourceCache(int ExpireSeconds)
        {
            this.m_expireSeconds = ExpireSeconds;
            this.m_cacheItems = new Dictionary<string, object>();
            this.m_cacheTimes = new Dictionary<string, DateTime>();
            this.m_cache_expireSeconds = new Dictionary<string, int>();
        }


        /// <summary>
        /// 如果cache存在返回true，否则false，value设置为null
        /// </summary>
        /// <param name="cache_key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get(string cache_key, out object value)
        {
            if (!this.m_cacheItems.ContainsKey(cache_key) || !this.m_cacheTimes.ContainsKey(cache_key))
            {
                value = null;
                return false;
            }
            // 判断是否过期
            var cached_at = this.m_cacheTimes[cache_key];
            var expireSeconds = this.m_expireSeconds;
            if (this.m_cache_expireSeconds.ContainsKey(cache_key))
            {
                expireSeconds = this.m_cache_expireSeconds[cache_key];
            }
            var expire_at = cached_at + TimeSpan.FromSeconds(expireSeconds);
            if (expire_at < DateTime.Now)
            {
                this.m_cacheItems.Remove(cache_key);
                this.m_cacheTimes.Remove(cache_key);
                value = null;
                return false;
            }
            else
            {
                value=  this.m_cacheItems[cache_key];
                return true;
            }
        }

        public void Set(string cache_key, object value, int? expire_seconds = null)
        {
            this.m_cacheItems.Remove(cache_key);
            this.m_cacheTimes.Remove(cache_key);
            this.m_cacheItems.Add(cache_key, value);
            this.m_cacheTimes.Add(cache_key, DateTime.Now);

            this.m_cache_expireSeconds.Remove(cache_key);
            if (expire_seconds != null)
            {
                this.m_cache_expireSeconds.Add(cache_key, expire_seconds.Value);
            }
        }


        /// <summary>
        /// 封装从缓存中获取值，如果未缓存，调用方法获取并设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache_key"></param>
        /// <param name="get_value_func"></param>
        /// <param name="expire_seconds"></param>
        /// <returns></returns>
        public T TryGet<T>(string cache_key, Func<T> get_value_func, int? expire_seconds = null)
        {
            var cached = this.Get(cache_key, out object value);
            if (cached)
            {
                return (T)value;
            }
            else
            {
                var item = get_value_func();
                this.Set(cache_key, item, expire_seconds);
                return item;
            }
        }
    }
}
