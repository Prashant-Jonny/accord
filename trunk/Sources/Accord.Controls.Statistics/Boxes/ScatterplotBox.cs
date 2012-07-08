﻿// Accord Statistics Controls Library
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

namespace Accord.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using Accord.Statistics.Visualizations;
    using ZedGraph;

    /// <summary>
    ///   Scatterplot Box for quickly displaying a form with a scatterplot on it
    ///   in the same spirit as System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    public partial class ScatterplotBox : Form
    {

        private GraphPane pane;
        private Thread formThread;

        private ScatterplotBox()
        {
            InitializeComponent();

            pane = scatterplotView1.Graph.GraphPane;
        }

        /// <summary>
        ///   Blocks the caller until the form is closed.
        /// </summary>
        /// 
        public void WaitForClose()
        {
            if (Thread.CurrentThread != formThread)
                formThread.Join();
        }

        /// <summary>
        ///   Gets the <see cref="ScatterplotView"/>
        ///   control used to display data in this box.
        /// </summary>
        /// 
        /// <value>The scatterplot view control.</value>
        /// 
        public ScatterplotView ScatterplotView
        {
            get { return scatterplotView1; }
        }

        /// <summary>
        ///   Sets the size of the symbols in the scatterplot.
        /// </summary>
        /// 
        /// <param name="size">The desired symbol size.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public ScatterplotBox SetSymbolSize(float size)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetSymbolSize(size)));
                return this;
            }

            foreach (LineItem item in pane.CurveList)
                 item.Symbol.Size = size;

            Refresh();

            return this;
        }

        /// <summary>
        ///   Sets size of the scatterplot window.
        /// </summary>
        /// 
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public ScatterplotBox SetSize(int width, int height)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetSize(width, height)));
                return this;
            }

            this.Width = width;
            this.Height = height;

            Refresh();

            return this;
        }

        /// <summary>
        ///   Displays a scatterplot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">The x-values for the data.</param>
        /// <param name="y">The y-values for the data.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static ScatterplotBox Show(double[] x, double[] y, int[] z = null, bool nonBlocking = false)
        {
            return Show("Scatterplot", x, y, z, nonBlocking);
        }

        /// <summary>
        ///   Displays a scatterplot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">The x-values for the data.</param>
        /// <param name="y">The y-values for the data.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static ScatterplotBox Show(string title, double[] x, double[] y, int[] z = null, bool nonBlocking = false)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, y, z);

            return show(scatterplot, nonBlocking);
        }

        /// <summary>
        ///   Displays a scatterplot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static ScatterplotBox Show(double[,] x, int[] z = null, bool nonBlocking = false)
        {
            return Show("Scatterplot", x, z, nonBlocking);
        }

        /// <summary>
        ///   Displays a scatterplot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static ScatterplotBox Show(string title, double[,] x, int[] z = null, bool nonBlocking = false)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, z);

            return show(scatterplot, nonBlocking);
        }

        /// <summary>
        ///   Displays a scatterplot with the specified data.
        /// </summary>
        /// 
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static ScatterplotBox Show(double[][] x, int[] z = null, bool nonBlocking = false)
        {
            return Show("Scatterplot", x, z, nonBlocking);
        }

        /// <summary>
        ///   Displays a scatterplot with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the plot window.</param>
        /// <param name="x">A two column matrix containing the (x,y) data pairs as rows.</param>
        /// <param name="z">The corresponding labels for the (x,y) pairs.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static ScatterplotBox Show(string title, double[][] x, int[] z = null, bool nonBlocking = false)
        {
            Scatterplot scatterplot = new Scatterplot(title);

            scatterplot.Compute(x, z);

            return show(scatterplot, nonBlocking);
        }

        private static ScatterplotBox show(Scatterplot scatterplot, bool hold)
        {
            ScatterplotBox form = null;
            Thread formThread = null;

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Show control in a form
                form = new ScatterplotBox();
                form.Text = scatterplot.Title;
                form.formThread = formThread;
                form.scatterplotView1.Scatterplot = scatterplot;

                stopWaitHandle.Set();

                Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);

            formThread.Start();

            stopWaitHandle.WaitOne();

            if (!hold)
                formThread.Join();

            return form;
        }

    }
}
