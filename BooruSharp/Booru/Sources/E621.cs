namespace BooruSharp.Booru
{
    /// <summary>
    /// E621.
    /// <para>https://e621.net/</para>
    /// </summary>
    public sealed class E621 : Template.E621
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="E621"/> class.
        /// </summary>
        public E621(BooruOptions options = null)
            : base("e621.net", options)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
