using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mount : MonoBehaviour
{
    public Vector2 riderOffset;
    public Vector2 riderOffsetSit;

    Actor mountCharacter;
    Actor currentRider;

    void Start()
    {
        mountCharacter = GetComponentInParent<Actor>();
    }

    void Update()
    {
        if (currentRider != null)
        {
            if (mountCharacter.currentState == ActorStates.Crawling)
            {
                currentRider.transform.position = transform.position + (Vector3)riderOffsetSit;
            }
            else
            {
                currentRider.transform.position = transform.position + (Vector3)riderOffset;
            }
            currentRider.facing = mountCharacter.facing;

            if (mountCharacter.wantsToRideDown)
            {
                EndRide();
                mountCharacter.wantsToRideDown = false;
            }

        }

        mountCharacter.cameraTarget.enabled = currentRider != null && currentRider.isCameraTarget;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherCharacter = other.GetComponent<Actor>();
        if (otherCharacter != null && !otherCharacter.wantsToRideDown)
        {
            StartRide(otherCharacter);
        }
    }

    void StartRide(Actor otherCharacter)
    {
        var otherController = otherCharacter.GetComponent<Controller>();
        if (otherController != null)
        {
            otherController.OverrideControl(mountCharacter);
            currentRider = otherCharacter;
            currentRider.currentRide = mountCharacter.gameObject;
            currentRider.rb.simulated = false;
            if (currentRider.isCameraTarget)
            {
                currentRider.cameraTarget.enabled = false;
            }
            currentRider.transform.SetParent(transform);
        }
    }

    void EndRide()
    {
        currentRider.GetComponent<Controller>().RestoreControl();
        currentRider.currentRide = null;
        currentRider.rb.simulated = true;
        if (currentRider.isCameraTarget)
        {
            currentRider.cameraTarget.enabled = true;
        }
        currentRider.transform.SetParent(null);
        currentRider = null;
        mountCharacter.SetControl(null);
    }
}
