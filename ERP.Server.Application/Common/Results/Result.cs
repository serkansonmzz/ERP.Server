namespace ERP.Server.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public List<string>? Errors { get; }

    protected Result(bool isSuccess, string? message = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result Success(string? message = null) => new(true, message);
    public static Result Failure(string message, List<string>? errors = null) => new(false, message, errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T data, bool isSuccess, string? message = null) 
        : base(isSuccess, message)
    {
        Data = data;
    }

    private Result(bool isSuccess, string? message, List<string>? errors)
        : base(isSuccess, message, errors)
    {
        Data = default;
    }

    public static Result<T> Success(T data, string? message = null) => new(data, true, message);
    public new static Result<T> Failure(string message, List<string>? errors = null) => new(false, message, errors);
}
