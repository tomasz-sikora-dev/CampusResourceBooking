namespace CampusResourceBooking.Dtos;

public sealed class ServiceResult
{
    public bool IsSuccess { get; }
    public string Message { get; }

    private ServiceResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static ServiceResult Success(string message = "Operacja zakończona powodzeniem.") => new(true, message);
    public static ServiceResult Failure(string message) => new(false, message);
}

public sealed class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public T? Value { get; }

    private ServiceResult(bool isSuccess, string message, T? value)
    {
        IsSuccess = isSuccess;
        Message = message;
        Value = value;
    }

    public static ServiceResult<T> Success(T value, string message = "Operacja zakończona powodzeniem.") => new(true, message, value);
    public static ServiceResult<T> Failure(string message) => new(false, message, default);
}
