using BooruSharp.Search;
using System;
using System.Net.Http;
using System.Web;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Rule 34.
    /// <para>https://rule34.xxx/</para>
    /// </summary>
    public sealed class Rule34 : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule34"/> class.
        /// </summary>
        public Rule34(BooruOptions options = null)
            // The limit is in fact 200000 but search with tags make it incredibly hard to know what is really your pid
            : base("rule34.xxx", options)
        {
            options.Flags = options.Flags | BooruFlag.NoComment | BooruFlag.LimitOf20000;
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        protected override void PreRequest(HttpRequestMessage message)
        {
            UriBuilder uriBuilder = new UriBuilder(message.RequestUri.AbsoluteUri);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (Options.Auth != null)
            {
                query["user_id"] = Options.Auth.UserId;
                query["api_key"] = Options.Auth.PasswordHash;
            }
            else
            {
                throw new AuthentificationRequired();
            }
            uriBuilder.Query = query.ToString();
            message.RequestUri = new Uri(uriBuilder.ToString());
        }
    }
}
