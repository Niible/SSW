using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]
public class Artifact : Triggerable
{
    private GameObject _dialogUI;
    private Dialog _dialogComponent;
    [SerializeField] private DialogData dialogData;

    // Start is called before the first frame update
    private void Start()
    {
        var component = TriggersUI.FindMainUIGameObject();
        _dialogUI = component.transform.Find("DialogUI").gameObject;
        _dialogComponent = _dialogUI.GetComponent<Dialog>();
    }

    protected override void OnTrigger(Hero hero)
    {
        base.OnTrigger(hero);
        hero.HasArtifact = true;
        _dialogComponent.hero = hero;
        _dialogComponent.dialogData = dialogData;
        _dialogUI.SetActive(true);
        Destroy(gameObject);
    }
}
