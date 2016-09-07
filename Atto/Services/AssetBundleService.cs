using UnityEngine;
using System.Collections.Generic;

namespace Atto.Services {

	public abstract class AssetBundleService : Service {

		public Dictionary<string, AssetBundle> currentDownloadingBundles = new Dictionary<string, AssetBundle>();
		public Dictionary<BundleType, BundleUnpacker> bundleUnpackers = new Dictionary<BundleType, BundleUnpacker>();
	
		protected abstract void UnpackBundles();

		GameObject unpackerObject;

		public AssetBundleService()
		{
			CreateUnpackerObject();
			CreateBundleUnpackers();
			UnpackBundles();
		}

		protected abstract void CreateBundleUnpackers();

		public BundleUnpacker this[BundleType bundleType]
		{
			get
			{
				return bundleUnpackers[bundleType];
			}
			set
			{
				bundleUnpackers[bundleType] = value;
			}
		}

		public float currentLoadingProgress
		{
			get
			{
				float result = 0;
				foreach (var bundleUnpacker in bundleUnpackers)
				{
					result += bundleUnpacker.Value.currentLoadingProgress;
				}
				result = result / bundleUnpackers.Count;
				return result;
			}
		}

		void CreateUnpackerObject() {
			unpackerObject = new GameObject("AssetBundle Unpacker");
			GameObject.DontDestroyOnLoad(unpackerObject);
		}

		protected void CreateBundleUnpacker<T>(BundleType bundleType) where T : BundleUnpacker
		{
			var subUnpackerObject = new GameObject(bundleType.ToString()+" Unpacker");
			subUnpackerObject.transform.SetParent(unpackerObject.transform);
			bundleUnpackers.Add(bundleType, subUnpackerObject.AddComponent<T>());
		}

		public bool IsAllDataLoaded()
		{
			bool result = true;
			foreach (var bundleUnpacker in bundleUnpackers)
			{
				result = result && bundleUnpacker.Value.isBundleDataLoaded;
			}
			return result;
		}

	}
}
