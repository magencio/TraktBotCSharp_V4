using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.Extensions
{
    public static class SerializationExtensions
    {
        public static async Task<T> DeserializeAsync<T>(this Task<string> content)
            => JsonConvert.DeserializeObject<T>(await content, new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        Debug.WriteLine($"JsonConvert.DeserializeObject error: {args.ErrorContext.Error}");
                        args.ErrorContext.Handled = true;
                    }
                });

        public static string Serialize(this object content)
            => JsonConvert.SerializeObject(content, new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        Debug.WriteLine($"JsonConvert.SerializeObject error: {args.ErrorContext.Error}");
                        args.ErrorContext.Handled = true;
                    }
                });
    }
}
