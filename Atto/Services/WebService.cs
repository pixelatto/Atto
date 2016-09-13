using System;
using UnityEngine;

namespace Atto.Services
{
	public abstract class WebService : Service
	{
		public abstract void LoadTexture(string url, Action<Texture2D> onComplete);
	}
}
