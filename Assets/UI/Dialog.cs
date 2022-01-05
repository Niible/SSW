using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class Dialog : MonoBehaviour
{
    [SerializeField] public DialogData dialogData;
    private UIDocument _uiDocument;
    public Hero hero;
    
    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        var title = _uiDocument.rootVisualElement.Q<Label>("Title");
        var details = _uiDocument.rootVisualElement.Q<Label>("Details");
        if (dialogData.titles.Count > 0)
        {
            title.text = dialogData.titles.First();
        }
        if (dialogData.details.Count > 0)
        {
            details.text = dialogData.details.First();
        }
        Time.timeScale = 0;
        hero.isMovementDisabled = true;
    }

    /**
     * On Single Press from Input System
     */
    private void OnSinglePress()
    {
        var title = _uiDocument.rootVisualElement.Q<Label>("Title");
        var details = _uiDocument.rootVisualElement.Q<Label>("Details");
        title.text = "";
        details.text = "";
        hero.isMovementDisabled = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
