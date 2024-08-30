using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        private Dictionary<string, object> m_cacheItems { get; set; }
        /* 保存实际缓存数据 */
        private Dictionary<string, DateTime> m_cacheTimes { get; set; }
        /* 存储缓存的时间 */
        private Dictionary<string, int> m_cache_expireSeconds { get; set; }
        /* 可单独给某缓存key设置过期时间 */

        private uint recycle_item_count { get; set; }
        /* TODO,当m_cacheItems数量超过一定时，触发扫描回收过期的  */

        public PWResourceCache(int ExpireSeconds)
        {
            this.m_expireSeconds = ExpireSeconds;
            this.m_cacheItems = new Dictionary<string, object>();
            this.m_cacheTimes = new Dictionary<string, DateTime>();
            this.m_cache_expireSeconds = new Dictionary<string, int>();
        }


        /// <summary>
        /// 清空已有缓存
        /// </summary>
        public void Invalidate()
        {
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
                this.Delete(cache_key);
                value = null;
                return false;
            }
            else
            {
                value = this.m_cacheItems[cache_key];
                return true;
            }
        }

        public void Set(string cache_key, object value, int? expire_seconds = null)
        {
            this.Delete(cache_key);
            this.m_cacheItems.Add(cache_key, value);
            this.m_cacheTimes.Add(cache_key, DateTime.Now);

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


        /// <summary>
        /// 按照具体的cache_key删除
        /// </summary>
        /// <param name="cache_key"></param>
        public void Delete(string cache_key)
        {
            this.m_cacheItems.Remove(cache_key);
            this.m_cacheTimes.Remove(cache_key);
            this.m_cache_expireSeconds.Remove(cache_key);
        }


        /// <summary>
        /// 按照值去删除，以及cache_key_pattern的模糊匹配
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cache_key_pattern">支持*,?等</param>
        public void DeleteByValue(object value, string cache_key_pattern)
        {
            var matched = this.GetByKeyPattern(cache_key_pattern);
            foreach (var kv in matched)
            {
                if (kv.Value == value)
                {
                    this.Delete(kv.Key);
                }
            }
        }

        public Dictionary<string, object> GetByKeyPattern(string cache_key_pattern)
        {
            var match_keys = new List<string>();
            foreach (var key in this.m_cacheItems.Keys)
            {
                if (new WildcardPattern(cache_key_pattern).IsMatch(key))
                {
                    match_keys.Add(key);
                }
            }
            var matched = new Dictionary<string, object>();
            foreach (var cache_key in match_keys)
            {
                var cached = this.Get(cache_key, out object value);
                if (cached)
                {
                    matched.Add(cache_key, value);
                }
            }
            return matched;
        }
    }

    public class WildcardPattern
    {
        private readonly string _expression;
        private readonly Regex _regex;

        public WildcardPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));

            _expression = "^" + Regex.Escape(pattern)
                .Replace("\\\\\\?", "??").Replace("\\?", ".").Replace("??", "\\?")
                .Replace("\\\\\\*", "**").Replace("\\*", ".*").Replace("**", "\\*") + "$";
            _regex = new Regex(_expression, RegexOptions.Compiled);
        }

        public bool IsMatch(string value)
        {
            return _regex.IsMatch(value);
        }
    }
}
