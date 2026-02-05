using UnityEngine;

public class CameraDebug : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float height = cam.orthographicSize * 2;
        float width = height * cam.aspect;
        
        Debug.Log($"Camera bounds - Width: {width}, Height: {height}");
        Debug.Log($"Top: {height/2}, Bottom: {-height/2}");
        Debug.Log($"Right: {width/2}, Left: {-width/2}");
    }
}