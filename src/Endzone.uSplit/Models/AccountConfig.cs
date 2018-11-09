using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Configuration;
using Umbraco.Core;
using Endzone.uSplit.Utility;

namespace Endzone.uSplit.Models
{
    public class AccountConfig
    {
        public string Name { get; }
        public string GoogleAccountId { get; }
        public string GoogleWebPropertyId { get; }
        public string GoogleProfileId { get; }

        public string UniqueId => GoogleProfileId;

        public AccountConfig(NameValueCollection settings, string name)
        {
            Name = name;
            GoogleAccountId = GetValue(settings, Constants.AppSettings.GoogleAccountId);
            GoogleWebPropertyId = GetValue(settings, Constants.AppSettings.GoogleWebPropertyId);
            GoogleProfileId = GetValue(settings, Constants.AppSettings.GoogleProfileId);
        }
        
        protected string GetValue(NameValueCollection appSettings, string key)
        {
            return appSettings[GetFullKey(Name, key)];
        }

        protected static string GetFullKey(string name, string key)
        {
            if (name.IsNullOrWhiteSpace())
            {
                return $"{Constants.AppSettings.Prefix}:{key}";
            }
            return $"{Constants.AppSettings.Prefix}:{name}:{key}";
        }

        public static IEnumerable<AccountConfig> GetAll()
        {
            var prefix = Constants.AppSettings.Prefix + ":";
            var keys = WebConfigurationManager.AppSettings.AllKeys;
            var names = new HashSet<string>();

            if(!keys.Where(x => x.StartsWith(Constants.AppSettings.Prefix)).Any())
            {
                // Probably need to try the dictionary
                DictionaryUtils uDic = new DictionaryUtils();
                var dictItems = uDic.GetDictionaryItemsSimple();
                keys = dictItems.Where(x => x.Key.StartsWith(Constants.AppSettings.Prefix)).Select(x => x.Key).ToArray();
            }

            foreach (var key in keys)
            {
                if (!key.StartsWith(prefix)) continue;
                var parts = key.Split(':');
                if (parts.Length == 2)
                {
                    names.Add(string.Empty);
                }
                else if (parts.Length == 3)
                {
                    names.Add(parts[1]);
                }
            }

            return names.Select(GetByName).Where(x => !string.IsNullOrEmpty(x.GoogleProfileId));
        }
        
        public static AccountConfig GetByName(string name)
        {
            var keys = WebConfigurationManager.AppSettings.AllKeys;

            AccountConfig res;

            // Get from appSettings or the dictionary
            if (keys.Where(x => x.StartsWith(Constants.AppSettings.Prefix)).Any())
            {
                // Try appSettings first
                res = new AccountConfig(WebConfigurationManager.AppSettings, name);
            }
            else
            {
                // Try the dictionary
                DictionaryUtils uDic = new DictionaryUtils();
                var dictItems = uDic
                                .GetDictionaryItemsSimple()
                                .Where(x => x.Key.StartsWith(Constants.AppSettings.Prefix));
                NameValueCollection nvc = dictItems
                                            .Aggregate(new NameValueCollection(),
                                                (seed, current) =>
                                                {
                                                    seed.Add(current.Key, current.Value);
                                                    return seed;
                                                });

                res = new AccountConfig(nvc, name);
            }

            return res;
        }

        public static AccountConfig GetByUniqueId(string uniqueId)
        {
            return GetAll().First(x => x.UniqueId == uniqueId);
        }
    }
}
