namespace Sisprenic.Api.Common;

/// <summary>
/// Similar to Nullable<T>, but works with any T and keeps assignment state 
/// separate from the value itself. This allows distinguishing an unassigned 
/// value from one explicitly assigned as null.
/// </summary>
public readonly struct Optional<T>
{
    public bool HasValue { get; }
    public T? Value { get; }

    private Optional(bool hasValue, T? value)
    {
        HasValue = hasValue;
        Value = value;
    }

    public static Optional<T> None => new(false, default);
    public static Optional<T> Some(T? value) => new(true, value);
}
