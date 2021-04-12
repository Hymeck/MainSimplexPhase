using System.Collections.Generic;
using MainSimplexPhase.Core;
using static System.Console;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            OIExample1();
            OIExample2();
            Task12();
        }

        private static void OIExample1()
        {
            WriteLine(nameof(OIExample1));
            var conditions = new double[,]
            {
                {-1, 1, 1, 0, 0},
                {1, 0, 0, 1, 0},
                {0, 1, 0, 0, 1}
            };

            var objectiveFunctionComponents = new double[] {1, 1, 0, 0, 0};
            var constraints = new double[] {1, 3, 2};
            var initialSolution = new double[] {0, 0, 1, 3, 2};
            var basisIndices = new SortedSet<int>(new[] {2, 3, 4});

            var simplexService = new SimplexMainPhaseService(conditions, objectiveFunctionComponents,
                initialSolution, basisIndices);

            var result = simplexService.Maximize();
            WriteLine(result);
            WriteLine();
        }
        
        private static void OIExample2()
        {
            WriteLine(nameof(OIExample2));
            var conditions = new double[,]
            {
                {1, -1, 1, 0},
                {-1, 1, 0, 1}
            };

            var objectiveFunctionComponents = new double[] {1, 0, 0, 0};
            var constraints = new double[] {1, 2};
            var initialSolution = new double[] {1, 0, 0, 3};
            var basisIndices = new SortedSet<int>(new[] {0, 3});

            var simplexService = new SimplexMainPhaseService(conditions, objectiveFunctionComponents,
                initialSolution, basisIndices);

            var result = simplexService.Maximize();
            WriteLine(result);
            WriteLine();
        }
        
        private static void Task12()
        {
            WriteLine(nameof(Task12));
            var conditions = new double[,]
            {
                {5, 4, 1, 0, 0, 0},
                {1, 2, 0, 1, 0, 0},
                {1, 0, 0, 0, 1, 0},
                {0, 1, 0, 0, 0, 1}
            };

            var objectiveFunctionComponents = new double[] {21, 25, 0, 0, 1, 0};
            var constraints = new double[] {50, 16, 9, 7};
            var initialSolution = new double[] {0, 0, 50, 16, 9, 7};
            var basisIndices = new SortedSet<int>(new[] {2, 3, 4, 5});

            var simplexService = new SimplexMainPhaseService(conditions, objectiveFunctionComponents,
                initialSolution, basisIndices);

            var result = simplexService.Maximize();
            WriteLine(result);
            WriteLine();
        }
    }
}
