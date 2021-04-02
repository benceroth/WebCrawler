// <copyright file="FileProcessor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.Sitemap
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Fake file processor.
    /// </summary>
    public class FileProcessor : IFileProcessor
    {
        private static readonly Random Random = new Random();
        private static readonly Regex ImageRegex = new Regex(".*?(\\.(?i)(jpg|jpeg|png|gif|bmp|mp3|pdf))$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex LinkRegex = new Regex(@"<(a).*?href=(""|')?(.+?)("".*?>|'.*?>| .*?>|>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Crawler crawler;
        private readonly IProducerConsumerCollection<Request<SitemapItem>> requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessor"/> class.
        /// </summary>
        /// <param name="requests">Requests.</param>
        public FileProcessor(Crawler crawler, IProducerConsumerCollection<Request<SitemapItem>> requests)
        {
            this.crawler = crawler;
            this.requests = requests;
        }

        /// <inheritdoc />
        public async Task Process(Request request)
        {
            var sitemapRequest = (Request<SitemapItem>)request;
            var input = await File.ReadAllTextAsync(request.Path);
            this.AddNewRequests(sitemapRequest, input);
            this.Finish();
        }

        public void Finish(bool execute = false)
        {
            if (execute || this.requests.Where(x =>
             x.DownloadState == DownloadState.None ||
             x.DownloadState == DownloadState.Queued ||
             x.DownloadState == DownloadState.Downloading ||
             x.ProcessState == ProcessState.Queued ||
             x.ProcessState == ProcessState.Processing).Count() == 1)
            {
                string head = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><urlset xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\" xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";
                string end = "</urlset>";
                List<StringBuilder> items = new List<StringBuilder>();
                foreach (var request in this.requests.Where(x => x.ProcessState == ProcessState.Completed).OrderByDescending(x => x.ItemDataBound.Priority))
                {
                    var sb = new StringBuilder();
                    sb.Append($"<url>");
                    sb.AppendLine();
                    sb.Append($"\t<loc>{request.Url}</loc>");
                    sb.AppendLine();
                    sb.Append($"\t<priority>{Math.Round(request.ItemDataBound.Priority, 4).ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture)}</priority>");
                    sb.AppendLine();
                    sb.Append($"\t<changefreq>weekly</changefreq>");
                    sb.AppendLine();
                    sb.Append($"\t<lastmod>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00</lastmod>");
                    sb.AppendLine();
                    sb.Append($"</url>");
                    items.Add(sb);
                }

                var output = head + Environment.NewLine + string.Join(Environment.NewLine, items.Select(x => x.ToString())) + Environment.NewLine + end;
                File.WriteAllText("sitemap.xml", output);
            }
        }

        private void AddNewRequests(Request<SitemapItem> request, string input)
        {
            foreach (Match match in LinkRegex.Matches(input))
            {
                if (match.Success && match.Groups.Count > 3 && match.Groups.Values.Skip(3).First() is Group group && group.Value.Length > 1)
                {
                    Request<SitemapItem> newRequest = null;
                    if (this.SameDomainButNotEqualUrls(request.Url, group.Value))
                    {
                        newRequest = new Request<SitemapItem>()
                        {
                            Name = group.Value,
                            FileName = this.Base64Encode(group.Value + this.GetUnixMs()),
                            Url = group.Value,
                        };
                    }
                    else if (!Uri.TryCreate(group.Value, UriKind.Absolute, out Uri _))
                    {
                        var url = this.RelativeToAbsoluteConversion(request.Url, group.Value);
                        newRequest = new Request<SitemapItem>()
                        {
                            Name = url,
                            FileName = this.Base64Encode(url + this.GetUnixMs()),
                            Url = url,
                        };
                    }

                    if (newRequest != null && this.AllowedUrl(newRequest.Url))
                    {
                        if (newRequest.Url.EndsWith("yourishop"))
                        {
                            newRequest.Url += "/";
                        }

                        newRequest.ItemDataBound = new SitemapItem { Priority = request.ItemDataBound.Priority * 0.8 };
                        this.requests.TryAdd(newRequest);
                        this.crawler.EnqueueDownloadRequest(newRequest);
                    }
                }
            }
        }

        private double GetUnixMs()
        {
            return DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private bool AllowedUrl(string url)
        {
            return !url.Contains('?') && !url.Contains("/cdn-cgi/l/email-protection") && !ImageRegex.IsMatch(url) && this.requests.FirstOrDefault(x => this.UrlsEqual(x.Url, url)) == null;
        }

        private string RelativeToAbsoluteConversion(string absUrl, string relUrl)
        {
            var absParts = absUrl.Split('/');
            var relParts = relUrl.Split('/');

            if (absUrl.IndexOf("yourishop") is int index && index >= 0)
            {
                if (absUrl.Length == index + 9)
                    absUrl += '/';
                return absUrl.Remove(index + 9) + '/' + relUrl;
            }

            for (int i = absParts.Length - 1; i >= 0; i--)
            {
                for (int j = relParts.Length - 1; j >= 0; j--)
                {
                    if (absParts[i] == relParts[j] && !string.IsNullOrEmpty(absParts[i]))
                    {
                        return string.Join('/', absParts.Take(i).Concat(relParts.Skip(j)));
                    }
                }
            }

            if (relUrl[0] != '/' && absUrl[absUrl.Length - 1] != '/')
            {
                return string.Join('/', absParts.Take(absParts.Length - 1)) + '/' + relUrl;
            }
            else
            {
                return absUrl + relUrl;
            }
        }

        private bool SameDomainButNotEqualUrls(string url1, string url2)
        {
            return !this.UrlsEqual(url1, url2) && Uri.TryCreate(url1, UriKind.Absolute, out Uri uri1) &&
                Uri.TryCreate(url2, UriKind.Absolute, out Uri uri2) && uri1.Host == uri2.Host;
        }

        private bool UrlsEqual(string url1, string url2)
        {
            url1 = url1.Replace("/", string.Empty);
            url2 = url2.Replace("/", string.Empty);
            if (url1.Equals(url2, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
