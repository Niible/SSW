using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Hero player;
    [SerializeField] private RespawnPoint lastRespawnPoint;
    
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        var playerChild = transform.Find("Player");
        if (!playerChild) return;
        player = playerChild.GetComponent<Hero>();
        if (player)
        {
            player.playerController = this;
        }
    }

    public void SetLastRespawnPoint(RespawnPoint respawnPoint)
    {
        lastRespawnPoint = respawnPoint;
    }

    public void Respawn()
    {
        if (!player) return;
        player.SetCurrentHorizontalDirection(lastRespawnPoint.facingDirection);
        player.SetNotGrounded();
        player.transform.position = lastRespawnPoint.transform.position;
        StartCoroutine(RoomController_ColliderController.LerpFromToPosition(_mainCamera.transform.position,
            lastRespawnPoint.cameraPosition.position, 0.9f, _mainCamera));
        StartCoroutine(RoomController_ColliderController.LerpFromToSize(_mainCamera.orthographicSize,
            lastRespawnPoint.cameraPosition.size, 0.9f, _mainCamera));
    }
}