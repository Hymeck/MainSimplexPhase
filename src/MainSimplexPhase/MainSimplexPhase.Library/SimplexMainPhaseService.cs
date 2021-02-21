using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace MainSimplexPhase.Core
{
    public class SimplexMainPhaseService
    {
        public readonly double[,] Conditions;
        public readonly double[] ObjectiveFunctionComponents;
        public readonly double[] Constraints;
        public readonly double[] InitialSolution;
        public readonly ISet<int> BasisIndices;

        public SimplexMainPhaseService(
            [NotNull] double[,] conditions,
            [NotNull] double[] objectiveFunctionComponents,
            [NotNull] double[] constraints,
            [NotNull] double[] initialSolution,
            [NotNull] ISet<int> basisIndices
        )
        {
            var condRowCount = conditions.GetLength(0);
            var condColumnCount = conditions.GetLength(1);

            if (constraints.Length != condRowCount)
                throw new ArgumentException("Dimension of constraints does not fit with condition matrix.",
                    nameof(constraints));
            if (objectiveFunctionComponents.Length != condColumnCount)
                throw new ArgumentException(
                    "Dimension of objective function components does not fit with condition matrix.",
                    nameof(objectiveFunctionComponents));
            if (initialSolution.Length != condColumnCount)
                throw new ArgumentException("Dimension of initial solution does not fit with condition matrix.",
                    nameof(initialSolution));
            if (basisIndices.Count == 0)
                throw new ArgumentException("Basis indices set cannot be empty.", nameof(basisIndices));

            Conditions = conditions;
            ObjectiveFunctionComponents = objectiveFunctionComponents;
            Constraints = constraints;
            InitialSolution = initialSolution;
            BasisIndices = basisIndices;
        }

        private static Vector<double> ToVector(double[] source) =>
            Vector<double>.Build.SparseOfArray(source);

        private static SortedSet<int> ToSortedSet(IEnumerable<int> source) => new(source);

        public SimplexResult Maximize()
        {
            var n = new SortedSet<int>(Enumerable.Range(0, ObjectiveFunctionComponents.Length));
            n.ExceptWith(BasisIndices);

            var A = Matrix<double>.Build.SparseOfArray(Conditions);
            var c = ToVector(ObjectiveFunctionComponents);
            var b = ToVector(Constraints);
            var x = ToVector(InitialSolution);
            var Jb = ToSortedSet(BasisIndices);

            return SimplexMainPhaseLogic.Maximize(A, c, b, x, Jb);
        }
    }
}