using UnityEngine;
using System.Collections;

public abstract class SceneService : Service
{

	public abstract SceneParams GetSceneParams();
	public abstract void LoadScene(object sceneToLoad, SceneParams newSceneParams);

}
