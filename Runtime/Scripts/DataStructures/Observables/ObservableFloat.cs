using System;

[Serializable]
public class ObservableFloat : Observable<float>
{
    public ObservableFloat(float initialValue = 0) : base(initialValue)
    {
    }

    public static implicit operator ObservableFloat(float initialValue)
    {
        return new ObservableFloat(initialValue);
    }
}
