using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private string firstGameScene;
    private Button _startButton;
    
    // Start is called before the first frame update
    private void Start()
    {
        _startButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("Start");
        _startButton.clicked += StartGame;
    }

    private void StartGame()
    {
        SceneManager.LoadSceneAsync(firstGameScene);
    }

    private void OnDestroy()
    {
        if (_startButton != null)
        {
            _startButton.clicked -= StartGame;
        }
    }
}
