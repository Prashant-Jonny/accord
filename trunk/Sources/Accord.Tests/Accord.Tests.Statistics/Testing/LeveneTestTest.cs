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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    
    [TestClass()]
    public class LeveneTestTest
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
        public void LeveneTestConstructorTest()
        {
            // Example from NIST/SEMATECH e-Handbook of Statistical Methods,
            // http://www.itl.nist.gov/div898/handbook/eda/section3/eda35a.htm

            double[][] samples = BartlettTestTest.samples;
            LeveneTest target = new LeveneTest(samples, median: true);

            Assert.AreEqual(9, target.DegreesOfFreedom1);
            Assert.AreEqual(90, target.DegreesOfFreedom2);
            Assert.AreEqual(1.7059176930008935, target.Statistic, 1e-10);
            Assert.IsFalse(double.IsNaN(target.Statistic));
        }

        [TestMethod()]
        public void LeveneTestConstructorTest2()
        {
            double[][] samples =
            {
                new double[]  { 250, 260, 230, 270 },
                new double[]  { 310, 330, 280, 360 },
                new double[]  { 250, 230, 220, 260 },
                new double[]  { 340, 270, 300, 320 },
                new double[]  { 250, 240, 270, 290 }
            };

            LeveneTest result = new LeveneTest(samples, true);

            Assert.AreEqual(0.7247191011235955, result.Statistic);
            Assert.AreEqual(0.58857793222910693, result.PValue);
        }
    }
}
