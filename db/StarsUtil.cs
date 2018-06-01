using System.Linq;

namespace db
{
    public static class StarsUtil
    {
        /// <summary>
        /// Default star
        /// </summary>
        public const int NORMAL_TYPE = 0;

        /// <summary>
        /// Crown star
        /// </summary>
        public const int CROWN_TYPE = 1;

        public const int PLUS_TYPE = 2;

        public const int ROBO_TYPE = 3;

        public const int FOCUS_TYPE = 4;

        public const int SWORD_TYPE = 5;

        public const int TROLL_TYPE = 6;

        private static readonly int[] stars = { 20, 150, 400, 800, 2000 };

        public static int StarCount => stars.Length;

        public static int GetStars(Stats stats)
        {
            if (stats.StarsOverride >= 0)
                return stats.StarsOverride;
            return (from i in stats.ClassStates from t in stars.Where(t => i.BestFame >= t) select i).Count();
        }

        public static int GetFameGoal(int fame)
        {
            return stars.FirstOrDefault(star => star >= fame);
        }
    }
}