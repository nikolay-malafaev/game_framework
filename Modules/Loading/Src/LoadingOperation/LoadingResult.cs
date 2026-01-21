namespace GameFramework.Loading
{
    public struct LoadingResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
     
        public LoadingResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public static LoadingResult Success(string message = "") => new(true, message);
        public static LoadingResult Error(string error = "") => new(false, error);
    }
}