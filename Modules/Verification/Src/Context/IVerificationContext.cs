using System;
using System.Runtime.CompilerServices;
using GameFramework.DI;

namespace GameFramework.Verification
{
    public interface IVerificationContext : IContext
    {
        /// <summary>
        /// Checks if a given boolean condition is true. 
        /// If false, it sends a verification message with context and source information.
        /// Used for quick runtime validation and debugging.
        /// </summary>
        /// <param name="condition">Condition to check.</param>
        /// <param name="message">Optional custom message for the failure report.</param>
        /// <param name="context">Optional Unity object related to the check.</param>
        /// <param name="sourceFilePath">Auto-filled file path.</param>
        /// <param name="sourceLineNumber">Auto-filled line number.</param>
        /// <param name="memberName">Auto-filled member name.</param>
        /// <returns>True if the condition is true, otherwise false.</returns>
        bool Verify(bool condition, string message = null, UnityEngine.Object context = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "");
        
        /// <summary>
        /// Executes a function that returns a boolean value and checks the result.
        /// If the result is false, it sends a verification message with context and source information.
        /// Useful for delayed or computed checks.
        /// </summary>
        /// <param name="condition">Function returning a boolean condition to check.</param>
        /// <param name="message">Optional custom message for the failure report.</param>
        /// <param name="context">Optional Unity object related to the check.</param>
        /// <param name="sourceFilePath">Auto-filled file path.</param>
        /// <param name="sourceLineNumber">Auto-filled line number.</param>
        /// <param name="memberName">Auto-filled member name.</param>
        /// <returns>True if the condition returns true, otherwise false.</returns>
        bool Verify(Func<bool> condition, string message = null, UnityEngine.Object context = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "");
    }
}