
namespace Atto.Serialization.Converters
{
	public class FloatListConverter : GenericListMonolineConverter<float>
	{
		protected override float Parse(string stringValue)
		{
			return float.Parse(stringValue);
		}
	}
}
