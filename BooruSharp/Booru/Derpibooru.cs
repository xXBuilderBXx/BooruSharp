using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Derpibooru.
    /// <para>https://derpibooru.org/</para>
    /// </summary>
    public class Derpibooru : Philomena
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Derpibooru"/> class.
        /// </summary>
        public Derpibooru(BooruOptions options = null) : base("derpibooru.org", options)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        protected override int FilterID => 56027;
    }
}
