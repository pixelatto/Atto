
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneService
{
    SceneParams GetSceneParams();
    void SetSceneParams(SceneParams sceneParams);
    void LoadScene(object sceneToLoad, SceneParams sceneParams);
}
