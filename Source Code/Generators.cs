using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    // Holds any number generators for the class
    // Static variable for consistency
    class Generators
    {
        #region Members
        private static Generators instance;

        Random random_number_generator;         // The only random object in the program
        int unique_ID_generator;                // Used to identify entities

        #endregion

        #region Constructors

        private Generators()
        {
            // TODO: Give a seed value
            Initialize(Environment.TickCount);
        }

        private void Initialize(int seed)
        {
            random_number_generator = new Random(seed);
            unique_ID_generator = 0;
        }

        #endregion

        #region Methods

        // Get the instance of the Graphics class. Creates a new one on the first access
        public static Generators Instance()
        {
            if (instance == null)
                instance = new Generators();
            return instance;
        }

        // Get a random number
        public int NextRandom()
        {
            return random_number_generator.Next();
        }

        // Get a random number. min <= number <= max
        // Pre: max must be greater than min
        public int NextRandom(int min, int max)
        {
            return random_number_generator.Next(min, max + 1);
        }

        // Provides a unique ID. After called MAX_INT * 2 times, the ID repeats
        public int NextUniqueID()
        {
            unique_ID_generator++;
            return unique_ID_generator;
        }

        #endregion
    }
}
