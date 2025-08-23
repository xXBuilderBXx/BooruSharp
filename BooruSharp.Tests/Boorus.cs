using BooruSharp.Booru;
using BooruSharp.Others;
using System.Net;

namespace BooruSharp.Tests;

internal static class Boorus
{
    private static readonly Dictionary<Type, Task<ABooru>> _boorus = new Dictionary<Type, Task<ABooru>>();



    public static Task<ABooru> GetAsync(Type type)
    {
        lock (_boorus)
        {
            if (!_boorus.TryGetValue(type, out Task<ABooru>? booruTask))
            {
                booruTask = Task.Run(() => CreateBooruAsync(type));
                _boorus[type] = booruTask;
            }

            return booruTask;
        }
    }

    public static Task<ABooru> GetAsync<T>() where T : ABooru
    {
        return GetAsync(typeof(T));
    }

    private static async Task<ABooru> CreateBooruAsync(Type type)
    {
        ABooru? booru = (ABooru)Activator.CreateInstance(type, new BooruOptions());
        booru.HttpClient = ABooru.CreateHttpClient(new BooruOptions
        {
            Proxy = new WebProxy(Environment.GetEnvironmentVariable("proxy_ip"), int.Parse(Environment.GetEnvironmentVariable("proxy_port")))
        });
        booru.HttpClient.DefaultRequestHeaders.Add("User-Agent", ABooru._userAgentHeaderValue);
        if (booru is Pixiv pixiv)
        {
            string refresh = Environment.GetEnvironmentVariable("PIXIV_REFRESH_TOKEN");

            Skip.If(refresh == null, "Pixiv tokens aren't set.");

            await pixiv.LoginAsync(refresh);
        }
        else
        {
            if (booru is Atfbooru)
            {
                booru.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", Environment.GetEnvironmentVariable("atf_cookie"));
            }
            else if (booru is Gelbooru)
            {
                booru.Options.Auth = new BooruAuth(Environment.GetEnvironmentVariable("gel_user"), Environment.GetEnvironmentVariable("gel_key"));
            }
            else if (booru is Rule34)
            {
                booru.Options.Auth = new BooruAuth(Environment.GetEnvironmentVariable("r34_user"), Environment.GetEnvironmentVariable("r34_key"));
            }
        }

        return booru;
    }
}
