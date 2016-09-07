
// TODO: Reimplement this as a service (without exposing coroutines, only using callback functions)
/*
IEnumerator LoadTextureFromWeb(string url)
{
	WWW www = new WWW(url);

	while (!www.isDone)
	{
		yield return www;

		www.LoadImageIntoTexture(value);

		if (www.error != null)
		{
			Debug.Log(www.error);
			break;
		}
	}
}
*/
