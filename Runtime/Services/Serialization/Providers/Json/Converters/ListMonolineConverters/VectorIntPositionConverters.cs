
using UnityEngine;

namespace Atto.Serialization.Converters
{
	public class Vector2IntPositionConverter : Vector2IntMonolineConverter
	{

		protected override string leftBracket => "(";
		protected override string rightBracket => ")";
	}

	public class Vector3IntPositionConverter : Vector3IntMonolineConverter
	{
		protected override string leftBracket => "(";
		protected override string rightBracket => ")";
	}
}