using UnityEngine;

public class Flammable : MonoBehaviour
{
    public bool isBurning = false;
    public float remainingFuel = 100;

    private void Update()
    {
        if (isBurning)
        {
            remainingFuel -= Time.deltaTime;
        }
    }
}