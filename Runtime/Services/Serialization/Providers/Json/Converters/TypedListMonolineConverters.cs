
public class IntListConverter : GenericListMonolineConverter<int>
{
    protected override int Parse(string stringValue)
    {
        return int.Parse(stringValue);
    }
}

public class FloatListConverter : GenericListMonolineConverter<float>
{
    protected override float Parse(string stringValue)
    {
        return float.Parse(stringValue);
    }
}

public class StringListConverter : GenericListMonolineConverter<string>
{
    protected override string Parse(string stringValue)
    {
        return stringValue;
    }
}
