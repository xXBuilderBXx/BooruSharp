using BooruSharp.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Defines basic capabilities of a booru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract partial class ABooru
    {
        /// <summary>
        /// Gets whether this booru is considered safe (that is, all posts on
        /// this booru have rating of <see cref="Search.Post.Rating.Safe"/>).
        /// </summary>
        public abstract bool IsSafe { get; }

        private protected virtual Search.Comment.SearchResult GetCommentSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult GetPostSearchResult(JToken obj)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult[] GetPostsSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual JToken ParseFirstPostSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Related.SearchResult GetRelatedSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Tag.SearchResult GetTagSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Wiki.SearchResult GetWikiSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Autocomplete.SearchResult[] GetAutocompleteResultAsync(object json)
            => throw new FeatureUnavailable();
        private protected virtual async Task<IEnumerable> GetTagEnumerableSearchResultAsync(Uri url)
        {
            if (TagsUseXml)
            {
                XmlDocument xml = await GetXmlAsync(url);
                return xml.LastChild;
            }
            else
            {
                return JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));
            }
        }

        /// <summary>
        /// Gets whether it is possible to search for related tags on this booru.
        /// </summary>
        public bool HasRelatedAPI => !Options.Flags.HasFlag(BooruFlag.NoRelated);

        /// <summary>
        /// Gets whether it is possible to search for wiki entries on this booru.
        /// </summary>
        public bool HasWikiAPI => !Options.Flags.HasFlag(BooruFlag.NoWiki);

        /// <summary>
        /// Gets whether it is possible to search for comments on this booru.
        /// </summary>
        public bool HasCommentAPI => !Options.Flags.HasFlag(BooruFlag.NoComment);

        /// <summary>
        /// Gets whether it is possible to search for tags by their IDs on this booru.
        /// </summary>
        public bool HasTagByIdAPI => !Options.Flags.HasFlag(BooruFlag.NoTagByID);

        /// <summary>
        /// Gets whether it is possible to search for the last comments on this booru.
        /// </summary>
        // As a failsafe also check for the availability of comment API.
        public bool HasSearchLastComment => HasCommentAPI && !Options.Flags.HasFlag(BooruFlag.NoLastComments);

        /// <summary>
        /// Gets whether it is possible to search for posts by their MD5 on this booru.
        /// </summary>
        public bool HasPostByMd5API => !Options.Flags.HasFlag(BooruFlag.NoPostByMD5);

        /// <summary>
        /// Gets whether it is possible to search for posts by their ID on this booru.
        /// </summary>
        public bool HasPostByIdAPI => !Options.Flags.HasFlag(BooruFlag.NoPostByID);

        /// <summary>
        /// Gets whether it is possible to get the total number of posts on this booru.
        /// </summary>
        public bool HasPostCountAPI => !Options.Flags.HasFlag(BooruFlag.NoPostCount);

        /// <summary>
        /// Gets whether it is possible to get multiple random images on this booru.
        /// </summary>
        public bool HasMultipleRandomAPI => !Options.Flags.HasFlag(BooruFlag.NoMultipleRandom);

        /// <summary>
        /// Gets whether this booru supports adding or removing favorite posts.
        /// </summary>
        public bool HasFavoriteAPI => !Options.Flags.HasFlag(BooruFlag.NoFavorite);

        /// <summary>
        /// Gets whether it is possible to autocomplete searches in this booru.
        /// </summary>
        public bool HasAutocompleteAPI => !Options.Flags.HasFlag(BooruFlag.NoAutocomplete);

        /// <summary>
        /// Gets whether this booru can't call post functions without search arguments.
        /// </summary>
        public bool NoEmptyPostSearch => Options.Flags.HasFlag(BooruFlag.NoEmptyPostSearch);

        /// <summary>
        /// Gets a value indicating whether searching by more than two tags at once is not allowed.
        /// </summary>
        public bool NoMoreThanTwoTags => Options.Flags.HasFlag(BooruFlag.NoMoreThan2Tags);

        /// <summary>
        /// Gets a value indicating whether http:// scheme is used instead of https://.
        /// </summary>
        protected bool UsesHttp => Options.Flags.HasFlag(BooruFlag.UseHttp);

        /// <summary>
        /// Gets a value indicating whether tags API uses XML instead of JSON.
        /// </summary>
        protected bool TagsUseXml => Options.Flags.HasFlag(BooruFlag.TagApiXml);

        /// <summary>
        /// Gets a value indicating whether comments API uses XML instead of JSON.
        /// </summary>
        protected bool CommentsUseXml => Options.Flags.HasFlag(BooruFlag.CommentApiXml);

        /// <summary>
        /// Gets a value indicating whether the max limit of posts per search is increased (used by Gelbooru).
        /// </summary>
        protected bool SearchIncreasedPostLimit => Options.Flags.HasFlag(BooruFlag.LimitOf20000);

        /// <summary>
        /// Checks for the booru availability.
        /// Throws <see cref="HttpRequestException"/> if service isn't available.
        /// </summary>
        public async Task CheckAvailabilityAsync()
        {
            await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _imageUrl));
        }

        /// <summary>
        /// Add booru authentification to current request
        /// </summary>
        /// <param name="message">The request that is going to be sent</param>
        protected virtual void PreRequest(HttpRequestMessage message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABooru"/> class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="format">The URL format to use.</param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected ABooru(string domain, UrlFormat format, BooruOptions options = null)
        {
            HttpClient = null;
            if (options == null)
                options = new BooruOptions();
            Options = options;

            bool useHttp = UsesHttp; // Cache returned value for faster access.
            BaseUrl = new Uri("http" + (useHttp ? "" : "s") + "://" + domain, UriKind.Absolute);
            _format = format;
            _imageUrl = CreateQueryString(format, format == UrlFormat.Philomena ? string.Empty : "post");

            if (_format == UrlFormat.IndexPhp)
                _imageUrlXml = new Uri(_imageUrl.AbsoluteUri.Replace("json=1", "json=0"));
            else if (_format == UrlFormat.PostIndexJson)
                _imageUrlXml = new Uri(_imageUrl.AbsoluteUri.Replace("index.json", "index.xml"));
            else
                _imageUrlXml = null;

            _tagUrl = CreateQueryString(format, "tag");

            if (HasWikiAPI)
                _wikiUrl = format == UrlFormat.Danbooru
                    ? CreateQueryString(format, "wiki_page")
                    : CreateQueryString(format, "wiki");

            if (HasRelatedAPI)
                _relatedUrl = format == UrlFormat.Danbooru
                    ? CreateQueryString(format, "related_tag")
                    : CreateQueryString(format, "tag", "related");

            if (HasCommentAPI)
                _commentUrl = CreateQueryString(format, "comment");

            if (HasAutocompleteAPI)
                _autocompleteUrl = format == UrlFormat.IndexPhp
                    ? new Uri(BaseUrl + "autocomplete.php")
                    : CreateQueryString(format, "autocomplete");
            switch (_format)
            {
                case UrlFormat.IndexPhp:
                    _autocompleteUrl = new Uri(BaseUrl + "autocomplete.php");
                    break;
                case UrlFormat.Danbooru:
                    _autocompleteUrl = new Uri(BaseUrl + "tags/autocomplete.json");
                    break;
                default:
                    _autocompleteUrl = CreateQueryString(_format, "autocomplete"); //this isn't supposed to work.
                    break;
            }
        }

        private protected Uri CreateQueryString(UrlFormat format, string query, string squery = "index")
        {
            string queryString;

            switch (format)
            {
                case UrlFormat.PostIndexJson:
                    queryString = query + "/" + squery + ".json";
                    break;

                case UrlFormat.IndexPhp:
                    queryString = "index.php?page=dapi&s=" + query + "&q=index&json=1";
                    break;

                case UrlFormat.Danbooru:
                    queryString = query == "related_tag" ? query + ".json" : query + "s.json";
                    break;

                case UrlFormat.Sankaku:
                    queryString = query == "wiki" ? query : query + "s";
                    break;

                case UrlFormat.Philomena:
                    queryString = $"api/v1/json/search/{query}{(string.IsNullOrEmpty(query) ? string.Empty : "s")}";
                    break;

                case UrlFormat.BooruOnRails:
                    queryString = $"api/v3/search/{query}s";
                    break;

                default:
                    return BaseUrl;
            }

            return new Uri(BaseUrl + queryString);
        }

        // TODO: Handle limitrate

        private protected async Task<string> GetJsonAsync(string url)
        {
            if (Options.Flags.HasFlag(BooruFlag.CookieRequired) && string.IsNullOrEmpty(Options.Cookie))
                throw new AuthentificationRequired("Browser cookie is required to use this Booru source .");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
            PreRequest(message);
            if (!string.IsNullOrEmpty(Options.Cookie))
                message.Headers.Add("Cookie", Options.Cookie);
            HttpResponseMessage msg = await HttpClient.SendAsync(message);

            if (msg.StatusCode == HttpStatusCode.Forbidden)
                throw new AuthentificationRequired();

            if (msg.StatusCode == (HttpStatusCode)422)
                throw new TooManyTags();

            msg.EnsureSuccessStatusCode();

            if (msg.Content.Headers.ContentType.MediaType != "application/json" && msg.Content.Headers.ContentType.MediaType != "text/xml")
                throw new AuthentificationRequired("Booru source is using captcha or other blocking features.");

            return await msg.Content.ReadAsStringAsync();
        }

        private protected Task<string> GetJsonAsync(Uri url)
        {
            return GetJsonAsync(url.AbsoluteUri);
        }

        private async Task<XmlDocument> GetXmlAsync(string url)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlString = await GetJsonAsync(url);
            // https://www.key-shortcut.com/en/all-html-entities/all-entities/
            xmlDoc.LoadXml(Regex.Replace(xmlString, "&([a-zA-Z]+);", HttpUtility.HtmlDecode("$1")));
            return xmlDoc;
        }

        private Task<XmlDocument> GetXmlAsync(Uri url)
        {
            return GetXmlAsync(url.AbsoluteUri);
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            if (Options.Flags.HasFlag(BooruFlag.CookieRequired) && string.IsNullOrEmpty(Options.Cookie))
                throw new AuthentificationRequired("Browser cookie is required to use this Booru source .");

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "index.php?page=post&s=random&tags=" + tags);
            PreRequest(message);
            if (!string.IsNullOrEmpty(Options.Cookie))
                message.Headers.Add("Cookie", Options.Cookie);
            HttpResponseMessage msg = await HttpClient.SendAsync(message);

            msg.EnsureSuccessStatusCode();
            return HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id");
        }

        private Uri CreateUrl(Uri url, params string[] args)
        {
            UriBuilder builder = new UriBuilder(url);

            if (builder.Query?.Length > 1)
                builder.Query = builder.Query.Substring(1) + "&" + string.Join("&", args);
            else
                builder.Query = string.Join("&", args);

            return builder.Uri;
        }

        private string TagsToString(string[] tags)
        {
            if (tags == null || !tags.Any())
            {
                // Philomena doesn't support search with no tag so we search for all posts with ID > 0
                return _format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "q=id.gte:0" : "tags=";
            }
            return (_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "q=" : "tags=")
                + string.Join(_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "," : "+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant();
        }

        private string SearchArg(string value)
        {
            return _format == UrlFormat.Danbooru
                ? "search[" + value + "]="
                : value + "=";
        }

        /// <summary>
        /// Sets the <see cref="System.Net.Http.HttpClient"/> instance that will be used
        /// to make requests. If <see langword="null"/> or left unset, the default
        /// <see cref="System.Net.Http.HttpClient"/> instance will be used.
        /// <para>This property can only be read in <see cref="ABooru"/> subclasses.</para>
        /// We advice you to disable the cookies and set automatic decompression to GZip and Deflate
        /// </summary>
        public HttpClient HttpClient
        {
            protected get
            {
                // If library consumers didn't provide their own client,
                // initialize and use singleton client instead.
                return _client ?? CreateHttpClient(Options);
            }
            set
            {
                _client = value;

                // Add our User-Agent if client's User-Agent header is empty.
                if (_client != null && !_client.DefaultRequestHeaders.Contains("User-Agent"))
                    _client.DefaultRequestHeaders.Add("User-Agent", _userAgentHeaderValue);
            }
        }

        /// <summary>
        /// Gets the instance of the thread-safe, pseudo-random number generator.
        /// </summary>
        protected static Random Random { get; } = new ThreadSafeRandom();

        /// <summary>
        /// Gets the base API request URL.
        /// </summary>
        public Uri BaseUrl { get; }

        private HttpClient _client;
        private readonly Uri _imageUrlXml, _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl, _autocompleteUrl; // URLs for differents endpoints
        // All options are stored in a bit field and can be retrieved using related methods/properties.
        public readonly BooruOptions Options;
        private readonly UrlFormat _format; // URL format
        private const string _userAgentHeaderValue = "Mozilla/5.0 BooruSharp";
        private protected readonly DateTime _unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static HttpClient CreateHttpClient(BooruOptions options)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            if (options.Proxy != null)
            {
                handler.UseProxy = true;
                handler.Proxy = options.Proxy;
            }
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", _userAgentHeaderValue);
            return client;
        }
    }
}
