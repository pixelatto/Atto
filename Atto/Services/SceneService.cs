
namespace Atto.Services
{
	public abstract class SceneService : Service
	{
		public abstract SceneParams GetSceneParams();
		public abstract void LoadScene(object sceneToLoad, SceneParams newSceneParams);
	}
}
