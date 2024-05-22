using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugKeys : MonoBehaviour
{

    void Update()
    {
        if (Debug.isDebugBuild)
        {
            //SCENE CONTROLS
            if (Input.GetKeyDown(KeyCode.F12))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            //TIMESCALE CONTROLS
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Time.timeScale = 1f;
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale * 2f, 0.1f, 10f);
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale / 2f, 0.1f, 10f);
            }

            //DAYTIME CONTROLS
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                var dayCycle = GameObject.FindObjectOfType<DayCycle>();
                dayCycle.dayTime = dayCycle.dayDuration * 0.25f;
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                var dayCycle = GameObject.FindObjectOfType<DayCycle>();
                dayCycle.dayTime = dayCycle.dayDuration * 0.75f;
            }
        }
    }
}