namespace BooruSharp.Booru
{
    /// <summary>
    /// Konachan.
    /// <para>https://konachan.com/</para>
    /// </summary>
    public sealed class Konachan : Template.Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Konachan"/> class.
        /// </summary>
        public Konachan(BooruOptions options = null)
            : base("konachan.com", options)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
