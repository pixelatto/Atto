
namespace Atto.Services
{ 
	public abstract class CloudAssetBundleService : AssetBundleService
	{
		public CloudAssetBundleService() : base() { }

		override protected void UnpackBundles() {
			foreach (var bundleUnpacker in bundleUnpackers)
			{
				bundleUnpacker.Value.StartCoroutine(bundleUnpacker.Value.DownloadBundle());
			}
		}
	}
}
