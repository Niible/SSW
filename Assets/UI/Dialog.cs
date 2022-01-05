using UnityEngine;
using UnityEngine.UIElements;

public class Dialog : MonoBehaviour
{
    [SerializeField] public DialogData dialogData;
    private UIDocument _uiDocument;
    public Hero hero;
    private int _currentStepIndex;

    private void OnEnable()
    {
        _currentStepIndex = 0;
        _uiDocument = GetComponent<UIDocument>();
        SetCurrentStep();
        Time.timeScale = 0;
        hero.isMovementDisabled = true;
    }

    /**
     * On Single Press from Input System
     */
    private void OnSinglePress()
    {
        _currentStepIndex += 1;
        if (_currentStepIndex > dialogData.titles.Count - 1 && _currentStepIndex > dialogData.details.Count - 1)
        {
            var title = _uiDocument.rootVisualElement.Q<Label>("Title");
            var details = _uiDocument.rootVisualElement.Q<Label>("Details");
            title.text = "";
            details.text = "";
            hero.isMovementDisabled = false;
            Time.timeScale = 1;
            gameObject.SetActive(false);
            return;
        }

        SetCurrentStep();
    }

    private void SetCurrentStep()
    {
        var title = _uiDocument.rootVisualElement.Q<Label>("Title");
        var details = _uiDocument.rootVisualElement.Q<Label>("Details");
        var format = dialogData.formats.Count - 1 >= _currentStepIndex
            ? dialogData.formats[_currentStepIndex]
            : TextType.Normal;
        if (dialogData.titles.Count - 1 >= _currentStepIndex)
        {
            if (format == TextType.Caption)
            {
                title.text =   "<i>" + dialogData.titles[_currentStepIndex] + "</i>";
                title.style.opacity = 0.5f;
            }
            else
            {
                title.text = dialogData.titles[_currentStepIndex];
                title.style.opacity = 1f;
            }
        }
        else
        {
            title.text = "";
        }
        if (dialogData.details.Count - 1 >= _currentStepIndex)
        {
            if (format == TextType.Caption)
            {
                details.text =   "<i>" + dialogData.details[_currentStepIndex] + "</i>";
                details.style.opacity = 0.5f;
            }
            else
            {
                details.text = dialogData.details[_currentStepIndex];
                details.style.opacity = 1f;
            }
        }
        else
        {
            details.text = "";
        }
    }
}