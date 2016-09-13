using System;
using System.Collections;
using UnityEngine;

namespace Atto.Services
{
	public class StandarWebService : WebService
	{
		private sealed class WebDownloader : MonoBehaviour
		{
		}

		public override void LoadTexture(string url, Action<Texture2D> onComplete)
		{
			var gameObject = new GameObject("Temp_LoadTexture");
			
			// TODO: Remove coroutine usage (temporal)
			gameObject.AddComponent<WebDownloader>().StartCoroutine(LoadTextureFromWeb(url, (texture) =>
			{
				onComplete(texture);
				UnityEngine.Object.Destroy(gameObject);
			}));
		}

		IEnumerator LoadTextureFromWeb(string url, Action<Texture2D> onComplete)
		{
			var texture = Texture2D.whiteTexture;
			var www = new WWW(url);

			while (!www.isDone)
			{
				yield return www;

				www.LoadImageIntoTexture(texture);

				if (www.error != null)
				{
					Core.Log.Error(www.error);
					break;
				}
			}

			onComplete(texture);
		}
	}
}
