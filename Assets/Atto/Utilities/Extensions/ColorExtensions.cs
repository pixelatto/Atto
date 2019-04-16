using UnityEngine;

namespace Atto
{
	public static class ColorExtensions
	{
		public static string ToHex(this Color color)
		{
			var color32 = (Color32)color;
			return "#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
		}

		public static Color HexCodeToColor(this string hexCode)
		{
			var color = Color.clear;

			try
			{
				if (hexCode.Length == 3)
				{
					byte r = byte.Parse(hexCode.Substring(0,1) + hexCode.Substring(0,1), System.Globalization.NumberStyles.HexNumber);
					byte g = byte.Parse(hexCode.Substring(1,1) + hexCode.Substring(1,1), System.Globalization.NumberStyles.HexNumber);
					byte b = byte.Parse(hexCode.Substring(2,1) + hexCode.Substring(2,1), System.Globalization.NumberStyles.HexNumber);

					color = new Color32(r, g, b, 255);
				}
				else if (hexCode.Length == 6)
				{
					byte r = byte.Parse(hexCode.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
					byte g = byte.Parse(hexCode.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
					byte b = byte.Parse(hexCode.Substring(4,2), System.Globalization.NumberStyles.HexNumber);

					color = new Color32(r, g, b, 255);
				}
				else if (hexCode.Length == 7)
				{
					color = (hexCode.Substring(1).HexCodeToColor());
				}
			}
			catch
			{
				Core.Logger.Error("Error converting HexColor {0}", hexCode);
			}

			return color;
		}
	}
}
