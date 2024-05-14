using System;

[Serializable]
public class ObservableInt : Observable<int>
{
    public ObservableInt(int initialValue = 0) : base(initialValue)
    {
    }

    public static implicit operator ObservableInt(int initialValue)
    {
        return new ObservableInt(initialValue);
    }
}
