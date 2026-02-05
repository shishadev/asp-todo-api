namespace Common;

public readonly struct Option<T> where T : class
{
    public Option(T value)
    {
        Value = value;
    }
    
    public T Value { get; }
    
    public bool IsSuccess => Value is not null;
    
    public static implicit operator Option<T>(T result)
    {
        return new Option<T>(result);
    }
}