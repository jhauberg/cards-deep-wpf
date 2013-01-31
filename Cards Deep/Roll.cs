using System;

namespace CardsDeep {
    internal static class Roll {
        static readonly Random random =
            new Random((int)DateTime.Now.Ticks);

        public static int Next(int min, int max) {
            return random.Next(min, max);
        }
    }
}
