using System.Collections;
using UnityEngine;

public class RoomController_ColliderController : Triggerable
{
    public Camera mainCamera;
    public CameraPosition cameraPosition;

    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        base.OnTrigger(other, hero);
        StartCoroutine(LerpFromToPosition(mainCamera.transform.position, cameraPosition.position, 0.5f, mainCamera));
        StartCoroutine(LerpFromToSize(mainCamera.orthographicSize, cameraPosition.size, 0.5f, mainCamera));
    }

    public static IEnumerator LerpFromToPosition(Vector3 initialPos, Vector3 newPos, float duration, Camera camera)
    {
        var currentCameraPosition = camera.transform.position;
        if(Mathf.Abs(currentCameraPosition.x - newPos.x) < Mathf.Epsilon && Mathf.Abs(currentCameraPosition.y - newPos.y) < Mathf.Epsilon)
        {
            yield break;
        }
        
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            camera.transform.position = Vector3.Lerp(initialPos, newPos, t / duration);
            yield return 0;
        }
        
        camera.transform.position = newPos;
    }

    public static IEnumerator LerpFromToSize(float initialSize, float newSize, float duration, Camera camera)
    {
        if (Mathf.Abs(camera.orthographicSize - newSize) < Mathf.Epsilon)
        {
            yield break;
        }
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            camera.orthographicSize = Mathf.Lerp(initialSize, newSize, t / duration);
            yield return 0;
        }

        camera.orthographicSize = newSize;
    }
    
    
}