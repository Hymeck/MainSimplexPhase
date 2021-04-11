using System;
using System.Collections.Immutable;

namespace MainSimplexPhase.Core
{
    public class SimplexResult
    {
        public ImmutableSortedSet<int> BasisIndices { get; init; }
        public ImmutableArray<double> Solution { get; init; }
        public double Value { get; init; }

        public SimplexResult(ImmutableSortedSet<int> basisIndices, ImmutableArray<double> solution, double value)
        {
            BasisIndices = basisIndices;
            Solution = solution;
            Value = value;
        }

        public override string ToString() =>
            $"F = {Value}{Environment.NewLine}X = ({string.Join("; ", Solution)})";
    }
}
