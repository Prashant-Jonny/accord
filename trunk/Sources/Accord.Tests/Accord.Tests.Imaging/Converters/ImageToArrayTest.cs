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

namespace Accord.Tests.Imaging
{
    using System.Drawing;
    using Accord.Imaging.Converters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Controls.Imaging;
    using AForge.Imaging.Filters;
    using System.Windows.Forms;
    using Accord.Math;

    [TestClass()]
    public class ImageToArrayTest
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
        public void ImageToArrayConstructorTest()
        {
            ImageToArray target = new ImageToArray();

            Assert.AreEqual(0, target.Min);
            Assert.AreEqual(1, target.Max);
            Assert.AreEqual(0, target.Channel);
        }

        [TestMethod()]
        public void ImageToArrayConstructorTest1()
        {
            double min = -10;
            double max = +10;
            int channel = 2;

            ImageToArray target = new ImageToArray(min, max, channel);

            Assert.AreEqual(min, target.Min);
            Assert.AreEqual(max, target.Max);
            Assert.AreEqual(channel, target.Channel);
        }


        [TestMethod()]
        public void ConvertTest3()
        {
            double[] pixels = 
            {
                 0, 0, 0, 0,
                 0, 1, 1, 0,
                 0, 1, 1, 0,
                 0, 0, 0, 0,
            };

            ArrayToImage conv1 = new ArrayToImage(width: 4, height: 4);
            Bitmap image;
            conv1.Convert(pixels, out image);
            image = new ResizeNearestNeighbor(16, 16).Apply(image);


            // Obtain a 16x16 bitmap image
            // Bitmap image = ...

            // Show on screen
            // ImageBox.Show(image, PictureBoxSizeMode.Zoom);

            // Create the converter to convert the image to an
            //   array containing only values between 0 and 1 
            ImageToArray conv = new ImageToArray(min: 0, max: 1);

            // Convert the image and store it in the array
            double[] array; conv.Convert(image, out array);

            // Show the array on screen
            // ImageBox.Show(array, 16, 16, PictureBoxSizeMode.Zoom);

            Assert.AreEqual(0, array.Min());
            Assert.AreEqual(1, array.Max());
            Assert.AreEqual(16 * 16, array.Length);
        }


    }
}
