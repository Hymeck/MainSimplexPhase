using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace MainSimplexPhase.Core
{
    internal static class SimplexMainPhaseLogic
    {
        public static SimplexResult Maximize(
            Matrix<double> conditions,
            Vector<double> objectiveFunctionComponents,
            Vector<double> constraints,
            Vector<double> initialSolution,
            SortedSet<int> basisIndices)
        {
            var phase = new MutableSimplexMainPhase(conditions, objectiveFunctionComponents, constraints,
                initialSolution, basisIndices);
            phase.Maximize();

            return ToResult(phase);
        }

        private static SimplexResult ToResult(MutableSimplexMainPhase phase)
        {
            var x = phase.Solution.ToImmutableArray();
            var indices = phase.B.ToImmutableSortedSet();
            
            var result = 0.0;
            foreach (var (a, b) in x.Zip(phase.ObjectiveFunctionComponents)) 
                result += a * b;
            
            return new SimplexResult(indices, x, result);
        }
    }
}