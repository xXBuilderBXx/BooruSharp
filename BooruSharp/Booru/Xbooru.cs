namespace BooruSharp.Booru
{
    /// <summary>
    /// Xbooru.
    /// <para>https://xbooru.com/</para>
    /// </summary>
    public sealed class Xbooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Xbooru"/> class.
        /// </summary>
        public Xbooru(BooruOptions options = null)
            : base("xbooru.com", options)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
