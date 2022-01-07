using System.Collections;
using UnityEngine;

public class RoomController_ColliderController : Triggerable
{
    public Camera mainCamera;
    public CameraPosition cameraPosition;

    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        base.OnTrigger(other, hero);
        StartCoroutine(LerpFromToPosition(mainCamera.transform.position, cameraPosition.position, 0.9f));
        StartCoroutine(LerpFromToSize(mainCamera.orthographicSize, cameraPosition.size, 0.9f));
    }

    private IEnumerator LerpFromToPosition(Vector3 initialPos, Vector3 newPos, float duration)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            mainCamera.transform.position = Vector3.Lerp(initialPos, newPos, t / duration);
            yield return 0;
        }

        transform.position = newPos;
    }

    private IEnumerator LerpFromToSize(float initialSize, float newSize, float duration)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            mainCamera.orthographicSize = Mathf.Lerp(initialSize, newSize, t / duration);
            yield return 0;
        }

        mainCamera.orthographicSize = newSize;
    }
}