using BooruSharp.Booru.Template;
using System;
using System.Net.Http;
using System.Web;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Ponybooru.
    /// <para>https://ponybooru.org/</para>
    /// </summary>
    public class Ponybooru : Philomena
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ponybooru"/> class.
        /// </summary>
        public Ponybooru(BooruOptions options = null) : base("ponybooru.org", options)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        protected override int FilterID => 2;

        protected override void PreRequest(HttpRequestMessage message)
        {
            UriBuilder uriBuilder = new UriBuilder(message.RequestUri.AbsoluteUri);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (Auth != null)
            {
                query["key"] = Auth.PasswordHash;
            }
            else
            {
                query["filter_id"] = $"{FilterID}"; // filter 2 for Ponybooru still hide stuff so we only add it if auth isn't given
            }
            uriBuilder.Query = query.ToString();
            message.RequestUri = new Uri(uriBuilder.ToString());
        }
    }
}
