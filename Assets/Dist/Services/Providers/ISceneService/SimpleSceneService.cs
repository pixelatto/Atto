using UnityEngine.SceneManagement;

public class SimpleSceneService : ISceneService
{
	SceneParams sceneParams;

	public SceneParams GetSceneParams()
	{
		if (sceneParams != null)
		{
			return sceneParams;
		}
		else
		{
			SceneParams newParams = new SceneParams();
			newParams.UseMockValues();
			sceneParams = newParams;
			return sceneParams;
		}
	}

    public void SetSceneParams(SceneParams sceneParams)
    {
        this.sceneParams = sceneParams;
    }

    public void LoadScene(object sceneToLoad, SceneParams sceneParams)
	{
		this.sceneParams = sceneParams;
		SceneManager.LoadScene(sceneToLoad.ToString());
	}
}
