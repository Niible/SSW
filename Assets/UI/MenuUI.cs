using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUI : DynamicTextSizeUI
{
    [SerializeField] private string firstGameScene;
    [SerializeField] private float startButtonPercentageSize;
    [SerializeField] private float titlePercentageSize;
    [SerializeField] private float detailsPercentageSize;
    private Button _startButton;
    
    // Start is called before the first frame update
    private void Start()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        _startButton = rootVisualElement.Q<Button>("Start");
        _startButton.clicked += StartGame;
        var title = rootVisualElement.Q<Label>("Title");
        var credits = rootVisualElement.Q<Label>("Credits");
        _startButton.parent.RegisterCallback<GeometryChangedEvent>((geometryEvent) => AdaptButtonSize(_startButton, geometryEvent.newRect.height));
        title.parent.RegisterCallback<GeometryChangedEvent>((geometryEvent) => AdaptTitleSize(title, geometryEvent.newRect.height));
        credits.parent.RegisterCallback<GeometryChangedEvent>((geometryEvent) => AdaptCreditsSize(credits, geometryEvent.newRect.height));
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

    private void AdaptButtonSize(Button button, float height)
    {
        button.parent.style.fontSize = GetFontSizeFromContainerSize(height, startButtonPercentageSize);
    }

    private void AdaptTitleSize(Label title, float height)
    {
        title.parent.style.fontSize = GetFontSizeFromContainerSize(height, titlePercentageSize);
    }

    private void AdaptCreditsSize(Label credits, float height)
    {
        credits.parent.style.fontSize = GetFontSizeFromContainerSize(height, detailsPercentageSize);
    }
}
