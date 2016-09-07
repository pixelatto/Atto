using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Atto;

namespace Atto.Services
{
	public class StandardSceneService : SceneService
	{
		public SceneParams sceneParams;

		override public SceneParams GetSceneParams()
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

		override public void LoadScene(object sceneToLoad, SceneParams sceneParams)
		{
			this.sceneParams = sceneParams;
			SceneManager.LoadScene(sceneToLoad.ToString() + "Scene");
		}

	}
}


