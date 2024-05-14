using System;
using UnityEngine;

[Serializable]
public class Observable<T>
{
    public event Action<T> OnValueChanged;

    [SerializeField] private T value;

    public T Value
    {
        get { return value; }
        set
        {
            if (!Equals(this.value, value))
            {
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }
    }

    public Observable(T initialValue = default)
    {
        value = initialValue;
    }
}
