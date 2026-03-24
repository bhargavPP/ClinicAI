namespace ClinicAI.Application.common.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }= string.Empty;
        public T? Data { get; set; }

        public static Result<T> Success(T data, string message = "")
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }
        public static Result<T> Failure(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
