namespace TraktBot.Model
{
    public class Episode
    {
        // Min
        public int Season { get; set; }

        public int Number { get; set; }

        public int IdsTrakt { get; set; }

        // Full

        // Watched Progress
        public bool Completed { get; set; }
    }
}
