using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class RoomController : Triggerable
{
    [SerializeField] [CanBeNull] private CameraPosition leftCameraPosition;
    [SerializeField] [CanBeNull] private CameraPosition rightCameraPosition;
    [SerializeField] [CanBeNull] private CameraPosition topCameraPosition;
    [SerializeField] [CanBeNull] private CameraPosition bottomCameraPosition;
    private Camera _mainCamera;
    [CanBeNull] private Collider2D _leftCollider;
    [CanBeNull] private Collider2D _rightCollider;
    [CanBeNull] private Collider2D _topCollider;
    [CanBeNull] private Collider2D _bottomCollider;

    // Start is called before the first frame update
    private void Start()
    {
        _mainCamera = Camera.main;
        if (leftCameraPosition)
        {
            var newGameObject = new GameObject("LeftCollider");
            newGameObject.transform.parent = gameObject.transform;
            newGameObject.transform.localScale = new Vector3(0.1f, 1);
            newGameObject.transform.localPosition = new Vector3(-(0.9f / 2), 0);
            var newCollider = newGameObject.AddComponent<BoxCollider2D>();
            newCollider.isTrigger = true;
            _leftCollider = newCollider;
            var colliderHandler = newGameObject.AddComponent<RoomController_ColliderController>();
            colliderHandler.cameraPosition = leftCameraPosition;
            colliderHandler.mainCamera = _mainCamera;
        }

        if (rightCameraPosition)
        {
            var newGameObject = new GameObject("RightCollider");
            newGameObject.transform.parent = gameObject.transform;
            newGameObject.transform.localScale = new Vector3(0.1f, 1);
            newGameObject.transform.localPosition = new Vector3(0.9f / 2, 0);
            var newCollider = newGameObject.AddComponent<BoxCollider2D>();
            newCollider.isTrigger = true;
            _rightCollider = newCollider;
            var colliderHandler = newGameObject.AddComponent<RoomController_ColliderController>();
            colliderHandler.cameraPosition = rightCameraPosition;
            colliderHandler.mainCamera = _mainCamera;
        }

        if (topCameraPosition)
        {
            var newGameObject = new GameObject("TopCollider");
            newGameObject.transform.parent = gameObject.transform;
            newGameObject.transform.localScale = new Vector3(1, 0.1f);
            newGameObject.transform.localPosition = new Vector3(0, 0.9f / 2);
            var newCollider = newGameObject.AddComponent<BoxCollider2D>();
            newCollider.isTrigger = true;
            _topCollider = newCollider;
            var colliderHandler = newGameObject.AddComponent<RoomController_ColliderController>();
            colliderHandler.cameraPosition = topCameraPosition;
            colliderHandler.mainCamera = _mainCamera;
        }

        if (bottomCameraPosition)
        {
            var newGameObject = new GameObject("BottomCollider");
            newGameObject.transform.parent = gameObject.transform;
            newGameObject.transform.localScale = new Vector3(1, 0.1f);
            newGameObject.transform.localPosition = new Vector3(0, -(0.9f / 2));
            var newCollider = newGameObject.AddComponent<BoxCollider2D>();
            newCollider.isTrigger = true;
            _bottomCollider = newCollider;
            var colliderHandler = newGameObject.AddComponent<RoomController_ColliderController>();
            colliderHandler.cameraPosition = bottomCameraPosition;
            colliderHandler.mainCamera = _mainCamera;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Display the bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}