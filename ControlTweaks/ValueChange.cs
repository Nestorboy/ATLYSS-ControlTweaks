namespace Nessie.ATLYSS.ControlTweaks;

public struct ValueChange<T>(T value = default)
{
    public bool Changed { get; private set; }

    private T _value = value;

    public void SetValue(T value)
    {
        Changed = !_value.Equals(value);
        _value = value;
    }

    public static implicit operator T(ValueChange<T> value)
    {
        return value._value;
    }
}