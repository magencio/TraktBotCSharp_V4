using Alejacma.TraktTv.Model;

namespace Alejacma.TraktTv.Extensions
{

    public static class ExtendedInfoLevelExtensions
    {
        private static readonly string[] ExtendedInfoLevelValues = new string[] {
            "min",
            "full",
            "episodes",
            "full,episodes"
        };

        public static string GetString(this ExtendedInfoLevel extendedInfoLevel) 
            => ExtendedInfoLevelValues[(int)extendedInfoLevel];
    }
}
