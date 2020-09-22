
namespace Atto.Serialization.Converters
{
	public class StringListConverter : GenericListMonolineConverter<string>
	{
		protected override string Parse(string stringValue)
		{
			return stringValue;
		}
	}
}