namespace BooruSharp.Booru
{
    /// <summary>
    /// Sakugabooru.
    /// <para>https://www.sakugabooru.com/</para>
    /// </summary>
    public sealed class Sakugabooru : Template.Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sakugabooru"/> class.
        /// </summary>
        public Sakugabooru(BooruOptions options = null)
            : base("sakugabooru.com", options)
        {
            options.Flags |= BooruFlag.NoLastComments;
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
