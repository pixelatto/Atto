

namespace Atto.Serialization.Converters
{
	public class VectorIntListConverter : GenericListMonolineConverter<int>
	{
		protected override int Parse(string stringValue)
		{
			return int.Parse(stringValue);
		}
	}
}
