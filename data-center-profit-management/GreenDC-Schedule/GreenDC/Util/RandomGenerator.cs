using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot
{
    class RandomGenerator
    {
        public static Random rnd = new Random((int)DateTime.Now.Ticks);

        public static void SetSeed(int seed)
        {
            rnd = new Random(seed);
        }

        /// <summary>
        /// [)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInteger(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static double GetRandomDouble()
        {
            return rnd.NextDouble();
        }
    }
}
