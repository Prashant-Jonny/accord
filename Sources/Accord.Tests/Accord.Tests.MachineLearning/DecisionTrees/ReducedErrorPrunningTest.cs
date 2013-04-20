﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.DecisionTrees.Prunning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.MachineLearning.DecisionTrees;
    using System.Data;
    using Accord.Statistics.Filters;
    using Accord.Math;
    using Accord.Tests.MachineLearning.Properties;
    using Accord.MachineLearning.DecisionTrees.Learning;


    [TestClass()]
    public class ReducedErrorPrunningTest
    {


        private TestContext testContextInstance;

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


        [TestMethod()]
        public void RunTest()
        {
            double[][] inputs;
            int[] outputs;

            int training = 6000;
            DecisionTree tree = createNurseryExample(out inputs, out outputs, training);

            int nodeCount = 0;
            foreach (var node in tree)
                nodeCount++;

            var prunningInputs = inputs.Submatrix(training, inputs.Length - 1);
            var prunningOutputs = outputs.Submatrix(training, inputs.Length - 1);
            var prune = new ReducedErrorPrunning(tree, prunningInputs, prunningOutputs);

            double lastError, error = Double.PositiveInfinity;
            do
            {
                lastError = error;
                error = prune.Run();
            } while (error <= lastError);

            int nodeCount2 = 0;
            foreach (var node in tree)
                nodeCount2++;

            Assert.AreEqual(0.19454022988505748, error);
            Assert.AreEqual(447, nodeCount);
            Assert.AreEqual(4, nodeCount2);
        }

        public static DecisionTree createNurseryExample(out double[][] inputs, out int[] outputs, int first)
        {
            string nurseryData = Resources.nursery;

            string[] inputColumns = 
            {
                "parents", "has_nurs", "form", "children",
                "housing", "finance", "social", "health"
            };

            string outputColumn = "output";

            DataTable table = new DataTable("Nursery");
            table.Columns.Add(inputColumns);
            table.Columns.Add(outputColumn);

            string[] lines = nurseryData.Split(
                new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
                table.Rows.Add(line.Split(','));

            Codification codebook = new Codification(table);
            DataTable symbols = codebook.Apply(table);
            inputs = symbols.ToArray(inputColumns);
            outputs = symbols.ToArray<int>(outputColumn);

            var attributes = DecisionVariable.FromCodebook(codebook, inputColumns);
            var tree = new DecisionTree(attributes, outputClasses: 5);

            C45Learning c45 = new C45Learning(tree);
            double error = c45.Run(inputs.Submatrix(first), outputs.Submatrix(first));

            Assert.AreEqual(0, error);

            return tree;
        }
    }
}
