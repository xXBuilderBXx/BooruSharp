namespace BooruSharp.Booru
{
    /// <summary>
    /// All The Fallen.
    /// <para>https://booru.allthefallen.moe/</para>
    /// </summary>
    public sealed class Atfbooru : Template.Danbooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Atfbooru"/> class.
        /// </summary>
        public Atfbooru(BooruOptions options = null)
            : base("booru.allthefallen.moe", options)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
