using UnityEngine;

public class Room : MonoBehaviour
{
    RoomPixelCamera cam;
    public Rect rect;
    public float cameraZoom = 1f;

    private void Awake()
    {
        cam = FindObjectOfType<RoomPixelCamera>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.gameObject.GetComponent<CameraTarget>();
        if (target != null)
        {
            cam.OnRoomEnter(this);
        }
    }
}
