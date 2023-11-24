using System;

namespace Models
{
    /// <summary>
    /// Exception for when an ObservedInput is missing when a ObservedReport is present in file.
    /// </summary>
    public class MissingObservedInputException : Exception
    {
        /// <summary>
        /// Exception for when an ObservedInput is missing when a ObservedReport is present in file.
        /// </summary>
        public MissingObservedInputException() { }

        /// <summary>
        /// Exception for when an ObservedInput is missing when a ObservedReport is present in file.
        /// </summary>
        /// <param name="message"></param>
        public MissingObservedInputException(string message) : base(message) { }

        /// <summary>
        /// Exception for when an ObservedInput is missing when a ObservedReport is present in file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MissingObservedInputException(string message, Exception inner) : base(message, inner) { }
    }
}
