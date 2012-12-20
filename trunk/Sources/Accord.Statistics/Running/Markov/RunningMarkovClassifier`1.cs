﻿// Accord Statistics Library
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

namespace Accord.Statistics.Running
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov;

    /// <summary>
    ///   Hidden Markov Classifier filter for general state distributions.
    /// </summary>
    /// 
    /// <seealso cref="HiddenMarkovClassifier{TDistribution}"/>
    /// <seealso cref="RunningMarkovClassifier"/>
    /// 
    [Serializable]
    public class RunningMarkovClassifier<TDistribution> : IRunning<double[]>
        where TDistribution : IDistribution
    {

        private RunningMarkovStatistics<TDistribution>[] models;

        /// <summary>
        ///   Gets the <see cref="HiddenMarkovClassifier{TDistribution}"/> used in this filter.
        /// </summary>
        /// 
        public HiddenMarkovClassifier<TDistribution> Classifier { get; private set; }

        /// <summary>
        ///   Gets the class response probabilities measuring
        ///   the likelihood of the current sequence belonging
        ///   to each of the classes.
        /// </summary>
        /// 
        public double[] Responses { get; private set; }

        /// <summary>
        ///   Gets the current classification label for
        ///   the sequence up to the current observation.
        /// </summary>
        /// 
        public int Classification { get; private set; }


        /// <summary>
        ///   Creates a new <see cref="RunningMarkovClassifier&lt;TDistribution&gt;"/>.
        /// </summary>
        /// 
        /// <param name="model">The hidden Markov classifier model.</param>
        ///         
        public RunningMarkovClassifier(HiddenMarkovClassifier<TDistribution> model)
        {
            this.Classifier = model;

            this.Responses = new double[model.Classes];
            this.models = new RunningMarkovStatistics<TDistribution>[model.Classes];
            for (int i = 0; i < model.Classes; i++)
                this.models[i] = new RunningMarkovStatistics<TDistribution>(model[i]);
        }


        /// <summary>
        ///   Registers the occurance of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param>
        /// 
        public void Push(params double[] value)
        {
            Classification = -1;
            double max = Double.NegativeInfinity;

            for (int i = 0; i < models.Length; i++)
            {
                models[i].Push(value);
                double prior = Math.Log(Classifier.Priors[i]);
                Responses[i] = prior + models[i].LogForward;

                if (Responses[i] > max)
                {
                    max = Responses[i];
                    Classification = i;
                }
            }
        }

        /// <summary>
        ///   Checks the classification after the insertion
        ///   of a new value without registering this value.
        /// </summary>
        /// 
        /// <param name="logLikelihood">
        ///   The next log-likelihood if the occurance of 
        ///   <paramref name="value"/> is registered.
        /// </param>
        /// <param name="value">The value to be checked.</param>
        /// 
        public int Peek(out double logLikelihood, params double[] value)
        {
            int imax = -1;
            logLikelihood = Double.NegativeInfinity;

            for (int i = 0; i < models.Length; i++)
            {
                double peek = models[i].Peek(value);
                double prior = Math.Log(Classifier.Priors[i]);
                Responses[i] = prior + peek;

                if (Responses[i] > logLikelihood)
                {
                    logLikelihood = Responses[i];
                    imax = i;
                }
            }

            return imax;
        }

        /// <summary>
        ///   Clears all measures previously computed.
        /// </summary>
        /// 
        public void Clear()
        {
            for (int i = 0; i < models.Length; i++)
                models[i].Clear();

            for (int i = 0; i < Responses.Length; i++)
                Responses[i] = 0;

            Classification = -1;
        }

    }
}
