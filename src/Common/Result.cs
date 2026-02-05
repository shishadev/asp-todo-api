namespace Common;

public struct Result<TResult, TException>
{
    private readonly TException? _exception;
    private readonly TResult? _result;

    public Result(TResult result)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
    }
    
    public Result(TException exception)
    {
        _exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
    
    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsSuccess => _result is not null;

    public void Match(Action<TResult> onSuccess, Action<TException> onError)
    {
        if (IsSuccess)
        {
            onSuccess.Invoke(_result!);
        }
        else
        {
            onError.Invoke(_exception!);
        }
    }

    public T Match<T>(Func<TResult, T> onSuccess, Func<TException, T> onError)
    {
        return IsSuccess ? onSuccess.Invoke(_result!) : onError.Invoke(_exception!);
    }
    
    public static implicit operator Result<TResult, TException>(TResult result)
    {
        return new Result<TResult, TException>(result);
    }
    
    public static implicit operator Result<TResult, TException>(TException exception)
    {
        return new Result<TResult, TException>(exception);
    }
}