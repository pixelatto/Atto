using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BundleUnpacker : MonoBehaviour
{

	public bool isBundleDataLoaded = false;
	public List<GameObject> bundlePrefabs = new List<GameObject>();

	protected AssetBundle currentBundle;
	protected BundleType bundleType = BundleType.Module;
	protected const string serverURL = "http://pixelatto.com/warmachines";
	protected const string platformName = "android";
	protected string bundleTypeName { get { return bundleType.ToString().ToLowerInvariant(); } }
	protected string packURL { get { return serverURL + "/" + platformName + "/"; } }
	protected string subpackURL { get { return packURL + bundleTypeName + "/"; } }

	protected string assetBundleURI { get { return Application.dataPath.Replace("/Assets", "/AssetBundles"); } }
	protected string packURI { get { return assetBundleURI + "/" + platformName + "/"; } }
	protected string subpackURI { get { return packURI + bundleTypeName + "/"; } }

	abstract protected void SetBundleType();
	abstract protected void ExtractAssetBundle(AssetBundle bundle);

	int elementCount = 0;

	static AssetBundleManifest manifestBundle;
	static string[] manifestBundleNames;
	static bool isManifestLoaded = false;

	public float currentLoadingProgress=0;

	public IEnumerator LoadBundleLocally()
	{
		SetBundleType();
		yield return CountElementsFromManifest();
		
		for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
		{
			string bundlePath = subpackURI + bundleTypeName + elementIndex.ToString();
			currentBundle=AssetBundle.LoadFromFile(bundlePath);
			ExtractAssetBundle(currentBundle);
			UnloadCurrentAssetBundle();
			currentLoadingProgress += 1f / elementCount;
		}
		isBundleDataLoaded = true;
	}

	public IEnumerator DownloadBundle()
	{
		SetBundleType();

		yield return CountElementsFromManifest();

		for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
		{
			string bundleURL = subpackURL + bundleTypeName + elementIndex.ToString();
			yield return DownloadAssetBundle(bundleURL);
			ExtractAssetBundle(currentBundle);
			UnloadCurrentAssetBundle();
			currentLoadingProgress += 1f / elementCount;
		}
		isBundleDataLoaded = true;
	}

	IEnumerator CountElementsFromManifest()
	{

		if (!isManifestLoaded)
		{
			isManifestLoaded = true;
			yield return DownloadAssetBundle(packURL + platformName);
			manifestBundle = (AssetBundleManifest)currentBundle.LoadAsset("AssetBundleManifest");
			manifestBundleNames = manifestBundle.GetAllAssetBundles();
		}

		while (manifestBundle == null)
		{
			yield return null;
		}

		for (int i = 0; i < manifestBundleNames.Length; i++)
		{
			if (manifestBundleNames[i].Contains(bundleType.ToString().ToLowerInvariant()))
			{
				elementCount++;
			}
		}
	}

	IEnumerator DownloadAssetBundle(string bundleURL, int version = 1)
	{
		while (!Caching.ready)
		{
			yield return null;
		}

		using (WWW www = WWW.LoadFromCacheOrDownload(bundleURL, version))
		{
			yield return www;
			if (www.error != null)
			{
				throw new System.Exception("WWW download had an error:" + www.error);
			}
			currentBundle = www.assetBundle;
		}
	}

	void UnloadCurrentAssetBundle()
	{
		if (currentBundle != null) {
			currentBundle.Unload(false);
			currentBundle = null;
		}
	}

	public GameObject GetPrefabFromName(string name)
	{
		return FindByName<GameObject>(name, bundlePrefabs);
	}

	T FindByName<T>(string name, List<T> objects) where T : Object
	{
		if (name == "None" || name == "")
		{
			return default(T);
		}

		T result = default(T);
		foreach (var module in objects)
		{
			if (name == module.name)
			{
				result = module;
			}
		}
		
		if (result == null) {
			Debug.LogWarning("Couldn't find '" + name + "' in the " + name + " Bundle Unpacker", gameObject);
		}

		return result;
	}
}

public enum BundleType { Unknown, Module, Bullet, Machine }
