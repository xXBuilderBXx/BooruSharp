namespace BooruSharp.Booru
{
    /// <summary>
    /// Danbooru.
    /// <para>https://danbooru.donmai.us/</para>
    /// </summary>
    public sealed class DanbooruDonmai : Template.Danbooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DanbooruDonmai"/> class.
        /// </summary>
        public DanbooruDonmai(BooruOptions options = null)
            : base("danbooru.donmai.us", options)
        {
            options.Flags |= BooruFlag.NoMoreThan2Tags;
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
