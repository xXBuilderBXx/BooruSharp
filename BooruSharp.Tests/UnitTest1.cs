using BooruSharp.Booru;
using BooruSharp.Booru.Sources;
using BooruSharp.Others;

namespace BooruSharp.Tests;

public class UnitTest1
{
    private const int _randomPostCount = 5;

    public static IEnumerable<object[]> BooruParams { get; } = new object[][]
    {
        new object[] { typeof(Atfbooru) },
        new object[] { typeof(DanbooruDonmai) },
        new object[] { typeof(E621) },
        new object[] { typeof(E926) },
        new object[] { typeof(Derpibooru) },
        new object[] { typeof(Gelbooru) },
        new object[] { typeof(Konachan) },
        new object[] { typeof(Ponybooru) },
        new object[] { typeof(Realbooru) },
        new object[] { typeof(Rule34) },
        new object[] { typeof(Safebooru) },
        new object[] { typeof(Sakugabooru) },
        new object[] { typeof(Twibooru) },
        new object[] { typeof(Xbooru) },
        new object[] { typeof(Yandere) },
        new object[] { typeof(Pixiv) },
    };

    public static IEnumerable<object[]> BooruPostCountParams { get; } = new object[][]
    {
        new object[] { typeof(Atfbooru) },
        new object[] { typeof(DanbooruDonmai), "hibiki_(kancolle)" },
        new object[] { typeof(Derpibooru), "swimsuit", "hat" },
        new object[] { typeof(E621), "kantai_collection", "swimwear" },
        new object[] { typeof(E926), "kantai_collection", "swimwear" },
        new object[] { typeof(Gelbooru), "hibiki_(kancolle)" },
        new object[] { typeof(Konachan), "hibiki_(kancolle)" },
        new object[] { typeof(Ponybooru), "swimsuit", "hat" },
        new object[] { typeof(Realbooru), "swimsuit", "asian" },
        new object[] { typeof(Rule34) },
        new object[] { typeof(Safebooru) },
        new object[] { typeof(Sakugabooru), "kantai_collection", "explosions" },
        new object[] { typeof(Twibooru), "swimsuit", "hat" },
        new object[] { typeof(Xbooru), "kantai_collection" },
        new object[] { typeof(Yandere), "kantai_collection", "swimsuits" },
        new object[] { typeof(Pixiv), "響(艦隊これくしょん)", "水着艦娘" },
    };

    public static IEnumerable<object[]> BooruRandomPostsParams { get; } = new object[][]
    {
        new object[] { typeof(Atfbooru) },
        new object[] { typeof(DanbooruDonmai) },
        new object[] { typeof(Derpibooru), "swimsuit" },
        new object[] { typeof(E621) },
        new object[] { typeof(E926) },
        new object[] { typeof(Gelbooru) },
        new object[] { typeof(Konachan) },
        new object[] { typeof(Ponybooru), "swimsuit" },
        new object[] { typeof(Realbooru), "small_breasts" },
        new object[] { typeof(Rule34) },
        new object[] { typeof(Safebooru) },
        new object[] { typeof(Sakugabooru), "kantai_collection" },
        new object[] { typeof(Twibooru), "swimsuit" },
        new object[] { typeof(Xbooru) },
        new object[] { typeof(Yandere) },
        new object[] { typeof(Pixiv), "スク水" },
    };

    public static IEnumerable<object[]> BooruRandomTwoTagsParams { get; } = new object[][]
    {
        new object[] { typeof(Atfbooru) },
        new object[] { typeof(DanbooruDonmai), "hibiki_(kancolle)" },
        new object[] { typeof(Derpibooru), "swimsuit", "hat" },
        new object[] { typeof(E621), "kantai_collection" },
        new object[] { typeof(E926), "kantai_collection" },
        new object[] { typeof(Gelbooru), "hibiki_(kancolle)" },
        new object[] { typeof(Konachan), "hibiki_(kancolle)" },
        new object[] { typeof(Ponybooru), "swimsuit", "hat" },
        new object[] { typeof(Realbooru), "school_swimsuit", "small_breasts" },
        new object[] { typeof(Rule34) },
        new object[] { typeof(Safebooru) },
        new object[] { typeof(Sakugabooru), "kantai_collection", "explosions" },
        new object[] { typeof(Twibooru), "swimsuit", "hat" },
        new object[] { typeof(Xbooru), "kantai_collection" },
        new object[] { typeof(Yandere), "kantai_collection" },
        new object[] { typeof(Pixiv), "響(艦隊これくしょん)", "スク水" },
    };

    public static IEnumerable<object[]> BooruTooManyTagsParams { get; } = new object[][]
    {
        new object[] { typeof(Atfbooru), false },
        new object[] { typeof(DanbooruDonmai), true },
        new object[] { typeof(Derpibooru), false, "swimsuit", "hat", "necklace" },
        new object[] { typeof(E621), false, "sea", "loli", "swimwear" },
        new object[] { typeof(E926), false, "sea", "breasts", "swimwear" },
        new object[] { typeof(Gelbooru), false },
        new object[] { typeof(Konachan), false, "water" },
        new object[] { typeof(Ponybooru), false, "swimsuit", "hat", "necklace" },
        new object[] { typeof(Realbooru), false, "water" },
        new object[] { typeof(Rule34), false },
        new object[] { typeof(Safebooru), false },
        new object[] { typeof(Sakugabooru), false, "kantai_collection", "explosions", "fire" },
        new object[] { typeof(Twibooru), false, "swimsuit", "hat", "necklace" },
        new object[] { typeof(Xbooru), false, "ocean", "small_breasts" },
        new object[] { typeof(Yandere), false, "see_through", "loli", "swimsuits" },
        new object[] { typeof(Pixiv), false, "東方", "貧乳", "水着" },
    };


    [SkippableTheory]
    [MemberData(nameof(BooruParams))]
    public async Task GetByIdAsync(Type t)
    {
        ABooru booru = await Boorus.GetAsync(t);
        if (!booru.HasPostByIdAPI)
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(() => booru.GetPostByIdAsync(0));
        else
        {
            Search.Post.SearchResult result1 = await General.GetRandomPostAsync(booru);
            Search.Post.SearchResult result2 = await booru.GetPostByIdAsync(result1.ID);
            Assert.Equal(result1.ID, result2.ID);
        }
    }
}