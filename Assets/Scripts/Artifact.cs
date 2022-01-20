using System.Collections.Generic;
using UnityEngine;

public class Artifact : Triggerable
{
    private GameObject _dialogUI;
    private Dialog _dialogComponent;
    [SerializeField] private DialogData dialogData;
    [SerializeField] private List<ArtifactMode> artifactModes;
    [SerializeField] private int artifactModeIndex;

    // Start is called before the first frame update
    private void Start()
    {
        var uiGameObject = TriggersUI.FindMainUIGameObject();
        _dialogUI = uiGameObject.transform.Find("DialogUI").gameObject;
        _dialogComponent = _dialogUI.GetComponent<Dialog>();
    }

    protected override void OnTrigger(Collider2D other, Hero hero)
    {
        base.OnTrigger(other, hero);
        var heroArtifactModeList = new List<ArtifactMode>();
        heroArtifactModeList.AddRange(artifactModes);
        hero.SetArtifactModeList(heroArtifactModeList);
        hero.SetArtifactModeIndex(artifactModeIndex);
        _dialogComponent.hero = hero;
        _dialogComponent.dialogData = dialogData;
        _dialogUI.SetActive(true);
        Destroy(gameObject);
    }
}