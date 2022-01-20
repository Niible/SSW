using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfDemo : Triggerable
{
    private GameObject _dialogUI;
    private Dialog _dialogComponent;
    [SerializeField] private DialogData dialogData;
    
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
        _dialogComponent.hero = hero;
        _dialogComponent.dialogData = dialogData;
        _dialogUI.SetActive(true);
        StartCoroutine(HandleEndOfDemo());
    }
    
    private IEnumerator HandleEndOfDemo()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadSceneAsync("Scenes/MenuScene");
    }
}