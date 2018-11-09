using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using Umbraco.Web;

namespace Endzone.uSplit.Utility
{
    public class DictionaryItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    class DictionaryUtils
    {
        private readonly UmbracoHelper _umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

        public IEnumerable<DictionaryItem> GetDictionaryItemsSimple()
        {
            //For now, we're only going to get the English items
            var culture = "en-US";
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            var root = Umbraco.Core.ApplicationContext.Current.Services.LocalizationService.GetRootDictionaryItems();

            var dictionary = new List<DictionaryItem>();

            if (!root.Any())
            {
                return Enumerable.Empty<DictionaryItem>();
            }

            foreach (var item in root)
            {
                dictionary.Add(new DictionaryItem
                {
                    Key = item.ItemKey,
                    Value = _umbracoHelper.GetDictionaryValue(item.ItemKey)
                });
            }

            return dictionary;
        }
    }
}
