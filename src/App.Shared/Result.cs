namespace App.Shared;

public sealed record Error(string Code, string Message);

public sealed class Result
{
    public static Result Success() => new(true, null);

    public static Result Fail(string code, string message) => new(false, new Error(code, message));

    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public Error? Error { get; }
}

public sealed class Result<T>
{
    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Fail(string code, string message) => new(false, default, new Error(code, message));

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public Error? Error { get; }
}
