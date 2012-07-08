﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
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

namespace Accord.Tests.Statistics
{

    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class NoncentralTDistributionTest
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
        public void DistributionFunctionTest()
        {
            double[,] table = 
            {
                //   x    d     df      expected
                {  3.00,  0.0,  1,    0.8975836176504333 },
                {  3.00,  0.0,  2,    0.9522670169       },
                {  3.00,  0.0,  3,    0.9711655571887813 },
                {  3.00,  0.5,  1,    0.8231218863999999 },
                {  3.00,  0.5,  2,    0.904902151        },
                {  3.00,  0.5,  3,    0.9363471834       },
                {  3.00,  1.0,  1,    0.7301025986       },
                {  3.00,  1.0,  2,    0.8335594263       },
                {  3.00,  1.0,  3,    0.8774010255       },
                {  3.00,  2.0,  1,    0.5248571617       },
                {  3.00,  2.0,  2,    0.6293856597       },
                {  3.00,  2.0,  3,    0.6800271741       },
                {  3.00,  4.0,  1,    0.20590131975      },
                {  3.00,  4.0,  2,    0.2112148916       },
                {  3.00,  4.0,  3,    0.2074730718       },
                { 15.00,  7.0, 15,    0.9981130072       },
                { 15.00,  7.0, 20,    0.999487385        },
                { 15.00,  7.0, 25,    0.9998391562       },
                {  0.05,  1.0,  1,    0.168610566972     },
                {  0.05,  1.0,  2,    0.16967950985      },
                {  0.05,  1.0,  3,    0.1701041003       },
                {  4.00,  2.0, 10,    0.9247683363       },
                {  4.00,  3.0, 10,    0.7483139269       },
                {  4.00,  4.0, 10,    0.4659802096       },
                {  5.00,  2.0, 10,    0.9761872541       },
                {  5.00,  3.0, 10,    0.8979689357       },
                {  5.00,  4.0, 10,    0.7181904627       },
                {  6.00,  2.0, 10,    0.9923658945       },
                {  6.00,  3.0, 10,    0.9610341649       },
                {  6.00,  4.0, 10,    0.868800735        },
            };

            for (int i = 0; i < table.GetLength(0); i++)
            {
                double x = table[i, 0];
                double delta = table[i, 1];
                double df = table[i, 2];

                var target = new NoncentralTDistribution(df, delta);

                double expected = table[i, 3];
                double actual = target.DistributionFunction(x);

                Assert.AreEqual(expected, actual, 1e-10);
            }
        }

        [TestMethod()]
        public void MeanTest()
        {
            NoncentralTDistribution target;

            target = new NoncentralTDistribution(3, 5);
            Assert.AreEqual(6.90988298942671, target.Mean);
            Assert.AreEqual(30.2535170724314, target.Variance);

            target = new NoncentralTDistribution(1.1, 5);
            Assert.AreEqual(44.672931414521223, target.Mean);
            Assert.IsTrue(Double.IsNaN(target.Variance));

            target = new NoncentralTDistribution(4.2, -2);
            Assert.AreEqual(-2.4746187622053673, target.Mean);
            Assert.AreEqual(3.42171652719572, target.Variance);

            target = new NoncentralTDistribution(0.047, 55);
            Assert.IsTrue(Double.IsNaN(target.Mean));
            Assert.IsTrue(Double.IsNaN(target.Variance));

            target = new NoncentralTDistribution(2.1, -0.42);
            Assert.AreEqual(-0.71446479359810855, target.Mean);
            Assert.AreEqual(24.19394005870879, target.Variance);

            target = new NoncentralTDistribution(5.97, -42);
            Assert.AreEqual(-48.390832208385575, target.Mean);
            Assert.AreEqual(312.49612392294557, target.Variance);
        }

    }
}
