using System;

namespace COILib.Extensions {

    public static class NumberExtention {

        public static int Between(this int x, int min, int max) => Math.Max(min, Math.Min(max, x));

        public static float Between(this float x, float min, float max) => Math.Max(min, Math.Min(max, x));
    }
}