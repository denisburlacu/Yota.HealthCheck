using System;
using System.Linq;

namespace Yota.HealthCheck
{
    internal static class UrlHelper
    {
        public static string Combine(params string[] urls)
        {
            var uri = new Uri(urls.First());
            uri = Append(uri, urls.Skip(1).ToArray());
            return uri.ToString();
        }

        private static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
        }
    }
}