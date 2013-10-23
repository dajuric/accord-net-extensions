using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Vision
{
    /// <summary>
    /// Termination criteria.
    /// </summary>
    public class TermCriteria
    {
        /// <summary>
        /// Gets or sets max number of iterations.
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Gets or sets minimal error.
        /// </summary>
        public double MinError { get; set; }

        /// <summary>
        /// Initializes structure.
        /// </summary>
        public TermCriteria()
        {
            this.MaxIterations = 1;
            this.MinError = 0;
        }

        /// <summary>
        /// Returns whether an procedure should terminate or not.
        /// </summary>
        /// <param name="numOfIterations">Number of executed iterations.</param>
        /// <param name="error">Current error.</param>
        /// <returns>True if an procedure should terminate.</returns>
        public bool ShouldTerminate(int numOfIterations, double error)
        {
            if (numOfIterations >= MaxIterations || error <= MinError)
                return true;
            else
                return false;
        }
    }
}
