using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
	private void Update()
	{
		if (Camera.main != null)
		{
			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		}
	}
}
