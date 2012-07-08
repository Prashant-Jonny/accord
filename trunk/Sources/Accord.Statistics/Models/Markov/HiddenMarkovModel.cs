// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
// Copyright © Guilherme Pedroso, 2009
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

namespace Accord.Statistics.Models.Markov
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Models.Markov.Learning;


    /// <summary>
    ///   Discrete-density Hidden Markov Model.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Hidden Markov Models (HMM) are stochastic methods to model temporal and sequence
    ///   data. They are especially known for their application in temporal pattern recognition
    ///   such as speech, handwriting, gesture recognition, part-of-speech tagging, musical
    ///   score following, partial discharges and bioinformatics.</para>
    /// <para>
    ///   Dynamical systems of discrete nature assumed to be governed by a Markov chain emits
    ///   a sequence of observable outputs. Under the Markov assumption, it is also assumed that
    ///   the latest output depends only on the current state of the system. Such states are often
    ///   not known from the observer when only the output values are observable.</para>
    ///   
    /// <para>
    ///   Hidden Markov Models attempt to model such systems and allow, among other things,
    ///   <list type="number">
    ///     <item><description>
    ///       To infer the most likely sequence of states that produced a given output sequence,</description></item>
    ///     <item><description>
    ///       Infer which will be the most likely next state (and thus predicting the next output),</description></item>
    ///     <item><description>
    ///       Calculate the probability that a given sequence of outputs originated from the system
    ///       (allowing the use of hidden Markov models for sequence classification).</description></item>
    ///     </list></para>
    ///     
    /// <para>     
    ///   The “hidden” in Hidden Markov Models comes from the fact that the observer does not
    ///   know in which state the system may be in, but has only a probabilistic insight on where
    ///   it should be.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       http://en.wikipedia.org/wiki/Hidden_Markov_model </description></item>
    ///     <item><description>
    ///       http://www.shokhirev.com/nikolai/abc/alg/hmm/hmm.html </description></item>
    ///     <item><description>
    ///       P396-397 “Spoken Language Processing” by X. Huang </description></item>
    ///     <item><description>
    ///       Dawei Shen. Some mathematics for HMMs, 2008. Available in:
    ///       http://courses.media.mit.edu/2010fall/mas622j/ProblemSets/ps4/tutorial.pdf </description></item>
    ///     <item><description>
    ///       http://www.stanford.edu/class/cs262/presentations/lecture7.pdf </description></item>
    ///     <item><description>
    ///       http://cs.oberlin.edu/~jdonalds/333/lecture11.html </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>The example below reproduces the same example given in the Wikipedia
    ///   entry for the Viterbi algorithm (http://en.wikipedia.org/wiki/Viterbi_algorithm).
    ///   In this example, the model's parameters are initialized manually. However, it is
    ///   possible to learn those automatically using <see cref="BaumWelchLearning"/>.</para>
    ///   
    /// <code>
    ///   // Create the transation matrix A
    ///   double[,] transition = 
    ///   {  
    ///       { 0.7, 0.3 },
    ///       { 0.4, 0.6 }
    ///   };
    ///   
    ///   // Create the emission matrix B
    ///   double[,] emission = 
    ///   {  
    ///       { 0.1, 0.4, 0.5 },
    ///       { 0.6, 0.3, 0.1 }
    ///   };
    ///   
    ///   // Create the initial probabilities pi
    ///   double[] initial =
    ///   {
    ///       0.6, 0.4
    ///   };
    ///   
    ///   // Create a new hidden Markov model
    ///   HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);
    ///   
    ///   // After that, one could, for example, query the probability
    ///   // of a sequence ocurring. We will consider the sequence
    ///   int[] sequence = new int[] { 0, 1, 2 };
    ///   
    ///   // And now we will evaluate its likelihood
    ///   double logLikelihood = hmm.Evaluate(sequence); 
    ///               
    ///   // At this point, the log-likelihood of the sequence
    ///   // ocurring within the model is -3.3928721329161653.
    ///   
    ///   // We can also get the Viterbi path of the sequence
    ///   int[] path = hmm.Decode(sequence, out logLikelihood); 
    ///               
    ///   // At this point, the state path will be 1-0-0 and the
    ///   // log-likelihood will be -4.3095199438871337
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="BaumWelchLearning">Automatic parameter learning for HMMs</seealso>
    /// <seealso cref="HiddenMarkovModel{TDistribution}">Arbitrary-density Hidden Markov Model.</seealso>
    /// 
    [Serializable]
    public class HiddenMarkovModel : BaseHiddenMarkovModel, IHiddenMarkovModel
    {

        // Model is defined as M = (A, B, pi)
        private double[,] logB; // emission probabilities

        // The other parameters are defined in HiddenMarkovModelBase
        // private double[,] A; // Transition probabilities
        // private double[] pi; // Initial state probabilities


        // Size of vocabulary
        private int symbols;



        //---------------------------------------------


        #region Constructors
        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public HiddenMarkovModel(ITopology topology, double[,] emissions, bool logarithm = false)
            : base(topology)
        {
            if (logarithm)
                logB = emissions;
            else logB = Matrix.Log(emissions);

            symbols = logB.GetLength(1);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        public HiddenMarkovModel(ITopology topology, int symbols)
            : base(topology)
        {
            this.symbols = symbols;

            // Initialize B with uniform probabilities
            logB = new double[States, symbols];
            for (int i = 0; i < States; i++)
                for (int j = 0; j < symbols; j++)
                    logB[i, j] = 1.0 / symbols;

            logB = Matrix.Log(logB);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="initial">The initial state probabilities for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public HiddenMarkovModel(double[,] transitions, double[,] emissions, double[] initial, bool logarithm = false)
            : this(new Custom(transitions, initial, logarithm), emissions, logarithm) { }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        public HiddenMarkovModel(int states, int symbols)
            : this(new Topology.Ergodic(states), symbols) { }

        #endregion


        //---------------------------------------------


        #region Public Properties
        /// <summary>
        ///   Gets the number of symbols in the alphabet of this model.
        /// </summary>
        /// 
        public int Symbols
        {
            get { return symbols; }
        }

        /// <summary>
        ///   Gets the Emission matrix (B) for this model.
        /// </summary>
        /// 
        public double[,] Emissions
        {
            get { return this.logB; }
        }
        #endregion


        //---------------------------------------------


        #region Public Methods

        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// 
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        /// 
        public int[] Decode(int[] observations)
        {
            double logLikelihood;
            return Decode(observations, out logLikelihood);
        }

        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="logLikelihood">The log-likelihood along the most likely sequence.</param>
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        /// 
        public int[] Decode(int[] observations, out double logLikelihood)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
            {
                logLikelihood = Double.NegativeInfinity;
                return new int[0];
            }


            // Viterbi-forward algorithm.
            int T = observations.Length;
            int states = States;
            int maxState;
            double maxWeight;
            double weight;

            double[] logPi = Probabilities;
            double[,] logA = Transitions;

            int[,] s = new int[states, T];
            double[,] lnFwd = new double[states, T];


            // Base
            for (int i = 0; i < states; i++)
                lnFwd[i, 0] = logPi[i] + logB[i, observations[0]];

            // Induction
            for (int t = 1; t < T; t++)
            {
                int observation = observations[t];

                for (int j = 0; j < states; j++)
                {
                    maxState = 0;
                    maxWeight = lnFwd[0, t - 1] + logA[0, j];

                    for (int i = 1; i < states; i++)
                    {
                        weight = lnFwd[i, t - 1] + logA[i, j];

                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    lnFwd[j, t] = maxWeight + logB[j, observation];
                    s[j, t] = maxState;
                }
            }


            // Find maximum value for time T-1
            maxState = 0;
            maxWeight = lnFwd[0, T - 1];

            for (int i = 1; i < states; i++)
            {
                if (lnFwd[i, T - 1] > maxWeight)
                {
                    maxState = i;
                    maxWeight = lnFwd[i, T - 1];
                }
            }


            // Trackback
            int[] path = new int[T];
            path[T - 1] = maxState;

            for (int t = T - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];


            // Returns the sequence probability as an out parameter
            logLikelihood = maxWeight;

            // Returns the most likely (Viterbi path) for the given sequence
            return path;
        }


        /// <summary>
        ///   Calculates the log-likelihood that this model has generated the given sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        ///   sequence O = {o1, o2, ..., oK}, calculate the probability that model
        ///   M has generated sequence O. This can be computed efficiently using the
        ///   either the Viterbi or the Forward algorithms.
        /// </remarks>
        /// 
        /// <param name="observations">
        ///   A sequence of observations.
        /// </param>
        /// 
        /// <returns>
        ///   The log-likelihood that the given sequence has been generated by this model.
        /// </returns>
        /// 
        public double Evaluate(int[] observations)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
                return Double.NegativeInfinity;

            // Forward algorithm
            double logLikelihood;

            // Compute forward probabilities
            ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);

            // Return the sequence probability
            return logLikelihood;
        }

        /// <summary>
        ///   Calculates the log-likelihood that this model has generated the
        ///   given observation sequence along the given state path.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. </param>
        /// <param name="path">A sequence of states. </param>
        /// 
        /// <returns>
        ///   The log-likelihood that the given sequence of observations has
        ///   been generated by this model along the given sequence of states.
        /// </returns>
        /// 
        public double Evaluate(int[] observations, int[] path)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (path == null)
                throw new ArgumentNullException("path");

            if (observations.Length == 0)
                return Double.NegativeInfinity;


            double logLikelihood = Probabilities[path[0]] + Emissions[path[0], observations[0]];

            for (int i = 1; i < observations.Length; i++)
            {
                logLikelihood = Accord.Math.Special.LogSum(logLikelihood,
                    Transitions[path[i - 1], path[i]] + Emissions[path[i], observations[i]]);
            }

            // Return the sequence probability
            return logLikelihood;
        }

        /// <summary>
        ///   Predicts next observations occurring after a given observation sequence.
        /// </summary>
        public int[] Predict(int[] observations, int next, out double logLikelihood)
        {
            double[][] logLikelihoods;
            return Predict(observations, next, out logLikelihood, out logLikelihoods);
        }

        /// <summary>
        ///   Predicts next observations occurring after a given observation sequence.
        /// </summary>
        public int[] Predict(int[] observations, int next)
        {
            double logLikelihood;
            double[][] logLikelihoods;
            return Predict(observations, next, out logLikelihood, out logLikelihoods);
        }

        /// <summary>
        ///   Predicts next observations occurring after a given observation sequence.
        /// </summary>
        public int[] Predict(int[] observations, int next, out double[][] logLikelihoods)
        {
            double logLikelihood;
            return Predict(observations, next, out logLikelihood, out logLikelihoods);
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        public int Predict(int[] observations, out double[] probabilities)
        {
            double[][] logLikelihoods;
            double logLikelihood;
            int prediction = Predict(observations, 1, out logLikelihood, out logLikelihoods)[0];
            probabilities = logLikelihoods[0];
            return prediction;
        }

        /// <summary>
        ///   Predicts the next observations occurring after a given observation sequence.
        /// </summary>
        public int[] Predict(int[] observations, int next, out double logLikelihood, out double[][] logLikelihoods)
        {
            int states = States;
            int T = next;
            double[,] lnA = Transitions;

            int[] prediction = new int[next];
            logLikelihoods = new double[next][];


            // Compute forward probabilities for the given observation sequence.
            double[,] lnFw0 = ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);

            // Create a matrix to store the future probabilities for the prediction
            // sequence and copy the latest forward probabilities on its first row.
            double[,] lnFwd = new double[T + 1, states];


            // 1. Initialization
            for (int i = 0; i < states; i++)
                lnFwd[0, i] = lnFw0[observations.Length - 1, i];

            // 2. Induction
            for (int t = 0; t < T; t++)
            {
                double[] weights = new double[symbols];
                for (int s = 0; s < symbols; s++)
                {
                    weights[s] = Double.NegativeInfinity;

                    for (int i = 0; i < states; i++)
                    {
                        double sum = Double.NegativeInfinity;
                        for (int j = 0; j < states; j++)
                            sum = Special.LogSum(sum, lnFwd[t, j] + lnA[j, i]);
                        lnFwd[t + 1, i] = sum + logB[i, s];

                        weights[s] = Special.LogSum(weights[s], lnFwd[t + 1, i]);
                    }
                }

                double sumWeight = Double.NegativeInfinity;
                for (int i = 0; i < weights.Length; i++)
                    sumWeight = Special.LogSum(sumWeight, weights[i]);
                for (int i = 0; i < weights.Length; i++)
                    weights[i] -= sumWeight;


                // Select most probable symbol
                double maxWeight = weights[0];
                prediction[t] = 0;
                for (int i = 1; i < weights.Length; i++)
                {
                    if (weights[i] > maxWeight)
                    {
                        maxWeight = weights[i];
                        prediction[t] = i;
                    }
                }

                // Recompute log-likelihood
                logLikelihoods[t] = weights;
                logLikelihood = maxWeight;
            }


            return prediction;
        }

        /// <summary>
        ///   Generates a random vector of observations from the model.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from the model.</returns>
        /// 
        public int[] Generate(int samples)
        {
            int[] path; double logLikelihood;
            return Generate(samples, out path, out logLikelihood);
        }

        /// <summary>
        ///   Generates a random vector of observations from the model.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="logLikelihood">The log-likelihood of the generated observation sequence.</param>
        /// <param name="path">The Viterbi path of the generated observation sequence.</param>
        /// 
        /// <returns>A random vector of observations drawn from the model.</returns>
        /// 
        public int[] Generate(int samples, out int[] path, out double logLikelihood)
        {
            double[] transitions = Probabilities;
            double[] emissions;

            int[] observations = new int[samples];
            logLikelihood = Double.NegativeInfinity;
            path = new int[samples];


            // For each observation to be generated
            for (int t = 0; t < observations.Length; t++)
            {
                // Navigate randomly on one of the state transitions
                int state = GeneralDiscreteDistribution.Random(Matrix.Exp(transitions));

                // Generate a sample for the state
                emissions = Emissions.GetRow(state);
                int symbol = GeneralDiscreteDistribution.Random(Matrix.Exp(emissions));

                // Store the sample
                observations[t] = symbol;
                path[t] = state;

                // Compute log-likelihood up to this point
                logLikelihood = Accord.Math.Special.LogSum(logLikelihood,
                    transitions[state] + emissions[symbol]);

                // Continue sampling
                transitions = Transitions.GetRow(state);
            }

            return observations;
        }

        /// <summary>
        ///   Converts this <see cref="HiddenMarkovModel">Discrete density Hidden Markov Model</see>
        ///   into a <see cref="HiddenMarkovModel{TDistribution}">arbitrary density model</see>.
        /// </summary>
        public HiddenMarkovModel<GeneralDiscreteDistribution> ToContinuousModel()
        {
            var transitions = (double[,])Transitions.Clone();
            var probabilities = (double[])Probabilities.Clone();

            var emissions = new GeneralDiscreteDistribution[States];
            for (int i = 0; i < emissions.Length; i++)
                emissions[i] = new GeneralDiscreteDistribution(Accord.Math.Matrix.GetRow(Emissions, i));

            return new HiddenMarkovModel<GeneralDiscreteDistribution>(transitions, emissions, probabilities);
        }

        /// <summary>
        ///   Converts this <see cref="HiddenMarkovModel">Discrete density Hidden Markov Model</see>
        ///   to a <see cref="HiddenMarkovModel{TDistribution}">Continuous density model</see>.
        /// </summary>
        public static explicit operator HiddenMarkovModel<GeneralDiscreteDistribution>(HiddenMarkovModel model)
        {
            return model.ToContinuousModel();
        }
        #endregion


        //---------------------------------------------

        #region Named constructors
        /// <summary>
        ///   Constructs a new discrete-density Hidden Markov Model.
        /// </summary>
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="probabilities">The initial state probabilities for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(double[,] transitions,
            double[,] emissions, double[] probabilities, bool logarithm = false)
        {
            ITopology topology = new Custom(transitions, probabilities, logarithm);

            if (emissions == null)
            {
                throw new ArgumentNullException("emissions");
            }

            if (emissions.GetLength(0) != topology.States)
            {
                throw new ArgumentException(
                    "The emission matrix should have the same number of rows as the number of states in the model.",
                    "emissions");
            }


            // Initialize B using a discrete distribution
            var B = new GeneralDiscreteDistribution[topology.States];
            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(Accord.Math.Matrix.GetRow(emissions, i));

            return new HiddenMarkovModel<GeneralDiscreteDistribution>(topology, B);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(ITopology topology, int symbols)
        {
            if (symbols <= 0)
            {
                throw new ArgumentOutOfRangeException("symbols",
                    "Number of symbols should be higher than zero.");
            }

            // Initialize B with a uniform discrete distribution
            GeneralDiscreteDistribution[] B = new GeneralDiscreteDistribution[topology.States];

            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(symbols);

            return new HiddenMarkovModel<GeneralDiscreteDistribution>(topology, B);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// 
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(int states, int symbols)
        {
            return CreateGeneric(new Topology.Ergodic(states), symbols);
        }
        #endregion


        //---------------------------------------------


        #region IHiddenMarkovModel implementation
        int[] IHiddenMarkovModel.Decode(Array sequence, out double logLikelihood)
        {
            return Decode((int[])sequence, out logLikelihood);
        }

        double IHiddenMarkovModel.Evaluate(Array sequence)
        {
            return Evaluate((int[])sequence);
        }
        #endregion

    }
}
