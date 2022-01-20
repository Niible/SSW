using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Hero player;

    private RespawnPoint _lastRespawnPoint;
    private GameObject _playerPrefab;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        _playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
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
        _lastRespawnPoint = respawnPoint;
    }

    public void Respawn()
    {
        if (!player) return;
        StartCoroutine(RoomController_ColliderController.LerpFromToPosition(_mainCamera.transform.position,
            _lastRespawnPoint.cameraPosition.position, 0.9f, _mainCamera));
        StartCoroutine(RoomController_ColliderController.LerpFromToSize(_mainCamera.orthographicSize,
            _lastRespawnPoint.cameraPosition.size, 0.9f, _mainCamera));
        player.transform.position = _lastRespawnPoint.transform.position;
    }
}