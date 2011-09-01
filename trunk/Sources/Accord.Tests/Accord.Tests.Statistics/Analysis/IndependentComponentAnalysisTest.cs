﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using Accord.Statistics.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
using System;
using Accord.Math.Formats;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for PrincipalComponentAnalysisTest and is intended
    ///to contain all PrincipalComponentAnalysisTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IndependentComponentAnalysisTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void ComputeTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X, IndependentComponentAlgorithm.Parallel);

            Assert.AreEqual(IndependentComponentAlgorithm.Parallel, ica.Algorithm);

            ica.Compute(2);

            var result = ica.Result;
            var mixingMatrix = ica.MixingMatrix;
            var revertMatrix = ica.DemixingMatrix;

            // Verify mixing matrix
            mixingMatrix = mixingMatrix.Divide(mixingMatrix.Sum().Sum());
            Assert.IsTrue(A.IsEqual(mixingMatrix, 0.008));


            // Verify demixing matrix
            double[,] expected =
            {
                { 0.75, -0.25 },        
                { 0.25,  0.25 },
            };

            revertMatrix = revertMatrix.Divide(revertMatrix.Sum().Sum());
            Assert.IsTrue(expected.IsEqual(revertMatrix, 0.008));
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void ComputeTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  1, 1 },
                { -1, 3 },    
            };

            A = A.Divide(Norm.Norm1(A));

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X, IndependentComponentAlgorithm.Deflation);

            Assert.AreEqual(IndependentComponentAlgorithm.Deflation, ica.Algorithm);

            ica.Compute(2);

            var result = ica.Result;
            var mixingMatrix = ica.MixingMatrix;
            var revertMatrix = ica.DemixingMatrix;

            // Verify mixing matrix
            mixingMatrix = mixingMatrix.Divide(Norm.Norm1(mixingMatrix));
            Assert.IsTrue(A.IsEqual(mixingMatrix, 0.05));


            // Verify demixing matrix
            double[,] expected =
            {
                { 3, -1 },        
                { 1,  1 },
            };

            expected = expected.Divide(Norm.Norm1(expected));

            revertMatrix = revertMatrix.Divide(Norm.Norm1(revertMatrix));
            Assert.IsTrue(expected.IsEqual(revertMatrix, 0.05));



            var reverted = Accord.Statistics.Tools.ZScores(result).Abs();
            var original = Accord.Statistics.Tools.ZScores(S).Abs();

            Assert.IsTrue(reverted.IsEqual(original, 0.1));
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void SeparateTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);


            ica.Compute(2);

            var expected = ica.Result;
            var actual = ica.Separate(X);

            Assert.IsTrue(expected.IsEqual(actual, 1e-4));
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void CombineTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);


            ica.Compute(2);

            var result = ica.Result;

            var expected = Accord.Statistics.Tools.ZScores(X);
            var actual = Accord.Statistics.Tools.ZScores(ica.Combine(result));

            Assert.IsTrue(expected.IsEqual(actual, 1e-4));
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void CombineTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);

            ica.Compute(2);

            double[,] result = ica.Result;


            float[][] expected = ica.Combine(result).ToSingle().ToArray(true);
            float[][] actual = ica.Combine(result.ToSingle().ToArray(true));

            Assert.IsTrue(expected.IsEqual(actual, 1e-4f));
        }


        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void SeparateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[,] S = Matrix.Random(5000, 2);

            double[,] A =
            {
                {  0.25, 0.25 },
                { -0.25, 0.75 },    
            };

            double[,] X = S.Multiply(A);

            IndependentComponentAnalysis ica = new IndependentComponentAnalysis(X);


            ica.Compute(2);

            var expected = ica.Result.ToSingle().ToArray(true);
            var actual = ica.Separate(X.ToSingle().ToArray(true));

            Assert.IsTrue(expected.IsEqual(actual, 1e-4f));
        }


    }
}
