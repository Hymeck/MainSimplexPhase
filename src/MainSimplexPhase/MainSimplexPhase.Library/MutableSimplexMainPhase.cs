using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace MainSimplexPhase.Core
{
    internal class MutableSimplexMainPhase
    {
        public Matrix<double> Conditions;
        public Vector<double> ObjectiveFunctionComponents;
        public Vector<double> Solution;
        public SortedSet<int> B;
        public SortedSet<int> N;

        public MutableSimplexMainPhase(Matrix<double> conditions,
            Vector<double> objectiveFunctionComponents,
            Vector<double> solution,
            SortedSet<int> b)
        {
            Conditions = conditions;
            ObjectiveFunctionComponents = objectiveFunctionComponents;
            Solution = solution;
            B = b;
            var n = new SortedSet<int>(Enumerable.Range(0, objectiveFunctionComponents.Count));
            n.ExceptWith(B);
            N = new SortedSet<int>(n);
        }

        private static Vector<double> GetComponents(SortedSet<int> indices, Vector<double> source)
        {
            var accumulator = new Queue<double>(indices.Count);
            for (var i = 0; i < source.Count; i++)
                if (indices.Contains(i))
                    accumulator.Enqueue(source[i]);

            return Vector<double>.Build.SparseOfEnumerable(accumulator);
        }

        private Matrix<double> GetMatrixFromConditions(SortedSet<int> indices)
        {
            var accumulator = new Queue<Vector<double>>(indices.Count);
            for (var i = 0; i < Conditions.ColumnCount; i++)
                if (indices.Contains(i))
                    accumulator.Enqueue(Conditions.Column(i));

            return Matrix<double>.Build.SparseOfColumns(accumulator);
        }

        public Vector<double> NonBasicObjectiveComponents => GetComponents(N, ObjectiveFunctionComponents);
        public Vector<double> BasicObjectiveComponents => GetComponents(B, ObjectiveFunctionComponents);

        // public Vector<double> NonBasicOptimalComponents => GetComponents(N, Solution);
        public Vector<double> BasicOptimalComponents => GetComponents(B, Solution);
        public Matrix<double> NonBasicMatrix => GetMatrixFromConditions(N);
        public Matrix<double> InversedBasicMatrix => GetMatrixFromConditions(B).Inverse();

        public Vector<double> GetDelta()
        {
            var cN = NonBasicObjectiveComponents;
            Debug.WriteLine(cN);
            var cB = BasicObjectiveComponents;
            Debug.WriteLine(cB);
            var aN = NonBasicMatrix;
            Debug.WriteLine(aN);
            var invAb = InversedBasicMatrix;
            Debug.WriteLine(invAb);
            return cN - cB * (invAb * aN);
        }

        public int GetNonBasisIndex(Vector<double> delta)
        {
            foreach (var (component, index) in delta.Zip(N))
                if (component > 0)
                    return index;

            return -1;
        }

        public (int removeFromBasisIndex, double maxTheta) GetTheta(Vector<double> nonBasis)
        {
            var invAb = InversedBasicMatrix;
            var product = invAb * nonBasis;

            var accumulator = new Queue<(double, int)>(product.Count);
            foreach (var (vectorComponent, basicIndex) in product.Zip(B))
                if (vectorComponent > 0)
                    accumulator.Enqueue((vectorComponent, basicIndex));

            if (accumulator.Count == 0)
                return (-1, 0);

            var firstTheta = accumulator.Dequeue();

            var thetaValue = Solution[firstTheta.Item2] / firstTheta.Item1;
            var removeFromBasisIndex = firstTheta.Item2;
            foreach (var (vectorComponent, basicIndex) in accumulator)
            {
                // var nextThetaValue = Convert.ToInt32(Solution[basicIndex] / vectorComponent);
                var nextThetaValue = Solution[basicIndex] / vectorComponent;
                if (nextThetaValue < thetaValue)
                {
                    removeFromBasisIndex = basicIndex;
                    thetaValue = nextThetaValue;
                }
            }

            return (removeFromBasisIndex, thetaValue);
        }

        public Vector<double> GetNewBasisVector(Vector<double> vN, double maxTheta)
        {
            var invAb = InversedBasicMatrix;
            var deltaVector = (invAb * vN) * maxTheta;
            return BasicOptimalComponents - deltaVector;
        }

        public bool MutateSolutionAndSets(int indexToAddToBasis)
        {
            var vN = Conditions.Column(indexToAddToBasis);
            var (removeFromBasisIndex, maxTheta) = GetTheta(vN);
            // task is not bounded
            if (removeFromBasisIndex == -1)
                return false;

            var newBasisVector = GetNewBasisVector(vN, maxTheta);

            // set new basis vector components
            foreach (var (vectorComponent, index) in newBasisVector.Zip(B))
                Solution[index] = vectorComponent;

            // x[n] = maxTheta
            Solution[indexToAddToBasis] = maxTheta;

            // remove index that goes to basis
            N.Remove(indexToAddToBasis);
            // set zeros to non basis vector components
            foreach (var nonBasisIndex in N)
                Solution[nonBasisIndex] = 0d;
            // add index that will be removed from basis
            N.Add(removeFromBasisIndex);

            B.Remove(removeFromBasisIndex);
            B.Add(indexToAddToBasis);

            return true;
        }

        public void Maximize()
        {
            var delta = GetDelta();
            var index = GetNonBasisIndex(delta);

            if (index == -1)
                return;

            while (index != -1)
            {
                if (MutateSolutionAndSets(index))
                {
                    delta = GetDelta();
                    index = GetNonBasisIndex(delta);
                }
                else
                    break;
            }
        }
    }
}