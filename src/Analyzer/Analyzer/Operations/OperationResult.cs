using System;

namespace Analyzer.Operations
{
    /// <summary>
    /// To Set failure, passin error message when constructing the object.
    /// Recipients should just make use of ISSucceess and ISError
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class OperationResult<TResult>
    {
        public OperationResult(TResult results, string errorMessage)
        {
            Results = results;
            ErrorMessage = String.IsNullOrEmpty(errorMessage) ? errorMessage : errorMessage.Trim();
        }
        public TResult Results { get; private set; }
        public string ErrorMessage { get; private set; }

        public bool IsSuccess { get { return String.IsNullOrEmpty(ErrorMessage); } }
        public bool IsError { get { return !IsSuccess; } }

        public static implicit operator bool(OperationResult<TResult> operationResult)
        {
            return operationResult.IsSuccess;
        }

        public static implicit operator OperationResult<TResult>(String errorMessage)
        {
            return new OperationResult<TResult>(default(TResult), errorMessage);
        }

        public static implicit operator OperationResult<TResult>(TResult result)
        {
            return new OperationResult<TResult>(result, null);
        }
    }
}
