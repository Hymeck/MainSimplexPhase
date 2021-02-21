using System;
using System.Collections.Immutable;

namespace MainSimplexPhase.Core
{
    public class SimplexResult
    {
        public ImmutableArray<double> Solution { get; init; }
        public double Value { get; init; }

        public SimplexResult(ImmutableArray<double> solution, double value) =>
            (Solution, Value) = (solution, value);

        public override string ToString() =>
            $"F = {Value}{Environment.NewLine}X = ({string.Join("; ", Solution)})";
    }
}