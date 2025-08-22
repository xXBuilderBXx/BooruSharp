using BooruSharp.Search;
using System;
using System.Net.Http;
using System.Web;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Gelbooru.
    /// <para>https://gelbooru.com/</para>
    /// </summary>
    public sealed class Gelbooru : Template.Gelbooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru"/> class.
        /// </summary>
        public Gelbooru(BooruOptions options = null)
            : base("gelbooru.com", options)
        { }

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
