//Rect extension methods for manipulating rects. Specially useful for Unity custom editor scripts
//Example:
//new Rect(0,0,16,16).MoveRight().MoveDown().Margin(1);
//Makes the same rect as: new Rect(17,17,14,14);

using UnityEngine;

public static class RectExtensions
{
	public static Rect Margin(this Rect inputRect, float margin)
	{
		return new Rect(inputRect.x + margin, inputRect.y + margin, inputRect.width - 2 * margin, inputRect.height - 2 * margin);
	}

	public static Rect MoveRight(this Rect inputRect, float distance=-1)
	{
		if (distance == -1) { distance = inputRect.width; }
		return new Rect(inputRect.x + distance, inputRect.y, inputRect.width, inputRect.height);
	}

	public static Rect MoveLeft(this Rect inputRect, float distance = -1)
	{
		if (distance == -1) { distance = inputRect.width; }
		return new Rect(inputRect.x - distance, inputRect.y, inputRect.width, inputRect.height);
	}

	public static Rect MoveDown(this Rect inputRect, float distance = -1)
	{
		if (distance == -1) { distance = inputRect.height; }
		return new Rect(inputRect.x, inputRect.y + distance, inputRect.width, inputRect.height);
	}

	public static Rect MoveUp(this Rect inputRect, float distance = -1)
	{
		if (distance == -1) { distance = inputRect.height; }
		return new Rect(inputRect.x, inputRect.y - distance, inputRect.width, inputRect.height);
	}

	public static Rect SetWidth(this Rect inputRect, float width)
	{
		return new Rect(inputRect.x, inputRect.y, width, inputRect.height);
	}

	public static Rect SetHeight(this Rect inputRect, float height)
	{
		return new Rect(inputRect.x, inputRect.y, inputRect.width, height);
	}

	public static Rect ReduceWidth(this Rect inputRect, float amount)
	{
		return new Rect(inputRect.x, inputRect.y, inputRect.width - amount, inputRect.height);
	}

	public static Rect ReduceHeight(this Rect inputRect, float amount)
	{
		return new Rect(inputRect.x, inputRect.y, inputRect.width, inputRect.height - amount);
	}

	public static Rect ScaleWidth(this Rect inputRect, float factor)
	{
		return new Rect(inputRect.x, inputRect.y, inputRect.width * factor, inputRect.height);
	}

	public static Rect ScaleHeight(this Rect inputRect, float factor)
	{
		return new Rect(inputRect.x, inputRect.y, inputRect.width, inputRect.height * factor);
	}
}
