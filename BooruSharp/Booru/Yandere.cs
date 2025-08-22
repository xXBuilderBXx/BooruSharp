namespace BooruSharp.Booru
{
    /// <summary>
    /// Yande.re.
    /// <para>https://yande.re/</para>
    /// </summary>
    public sealed class Yandere : Template.Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Yandere"/> class.
        /// </summary>
        public Yandere(BooruOptions options = null)
            : base("yande.re", options)
        {
            options.Flags |= BooruFlag.NoLastComments;
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
