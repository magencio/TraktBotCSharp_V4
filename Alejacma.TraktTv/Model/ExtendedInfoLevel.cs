namespace Alejacma.TraktTv.Model
{
    public enum ExtendedInfoLevel
    {
        // SHOWS, SEASONS, EPISODES, PEOPLE
        Min,                        // Default. Returns enough info to match locally
        Full,                       // Complete info

        // SEASONS
        Episodes,                   // All episodes
        FullWithEpisodes            // Complete info plus episodes
    }
}
