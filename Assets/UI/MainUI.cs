using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    [SerializeField] private bool shouldShowArtifact = false;
    private UIDocument _uiDocument;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        SetShouldShowArtifact(shouldShowArtifact);
    }

    public void SetShouldShowArtifact(bool value)
    {
        shouldShowArtifact = value;
        var artifactImg = _uiDocument.rootVisualElement.Q<IMGUIContainer>("Artifact");
        artifactImg.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
