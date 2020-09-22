
namespace Atto.Serialization.Converters
{
	public class IntListConverter : GenericListMonolineConverter<int>
	{
		protected override int Parse(string stringValue)
		{
			return int.Parse(stringValue);
		}
	}
}
