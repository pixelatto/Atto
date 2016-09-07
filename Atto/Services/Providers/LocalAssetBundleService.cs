using UnityEngine;
using System.Collections.Generic;
using System;
using Atto.Services;

public abstract class LocalAssetBundleService : AssetBundleService {

	public LocalAssetBundleService() : base() { }

	override protected void UnpackBundles() {
		foreach (var bundleUnpacker in bundleUnpackers)
		{
			bundleUnpacker.Value.StartCoroutine(bundleUnpacker.Value.LoadBundleLocally());
		}
	}

}
