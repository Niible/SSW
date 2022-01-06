using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class MainUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset artifactModeAsset;
    [SerializeField] private VisualTreeAsset artifactModeArrowAsset;
    [SerializeField] private Sprite artifactModeArrowSprite;
    private UIDocument _uiDocument;
    private List<ArtifactModeInfo> _allArtifactModeInfos;
    private ScrollView _artifactModeScrollView;
    private Texture2D _artifactModeArrow;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        _artifactModeScrollView = _uiDocument.rootVisualElement.Q<ScrollView>("ArtifactModesList");
        _allArtifactModeInfos = new List<ArtifactModeInfo>();
        _allArtifactModeInfos.AddRange(Resources.LoadAll<ArtifactModeInfo>("ArtifactModeInfo"));
        var width = Mathf.RoundToInt(artifactModeArrowSprite.rect.width); 
        var height = Mathf.RoundToInt(artifactModeArrowSprite.rect.height); 
        var x = Mathf.RoundToInt(artifactModeArrowSprite.rect.x); 
        var y = Mathf.RoundToInt(artifactModeArrowSprite.rect.y); 
        _artifactModeArrow = new Texture2D(
            width,
            height
        );
        var pixels = artifactModeArrowSprite.texture.GetPixels(
            x,
            y,
            width,
            height
        );
        _artifactModeArrow.SetPixels(pixels);
        _artifactModeArrow.Apply();
    }

    public void RefreshArtifactList(Hero hero)
    {
        _artifactModeScrollView.Clear();

        var heroArtifactModeList = hero.GetArtifactModeList();
        if (heroArtifactModeList.Count == 0) return;
        
        var heroCurrentArtifactModeIndex = hero.GetArtifactModeIndex();
        for (var i = 0; i < heroArtifactModeList.Count(); i++)
        {
            var artifactMode = heroArtifactModeList[i];
            if (i > 0)
            {
                var newArrowEntry = artifactModeArrowAsset.Instantiate();
                var arrowVisualElement = newArrowEntry.Q<VisualElement>("ArtifactModeArrow");
                arrowVisualElement.style.backgroundImage = _artifactModeArrow;

                _artifactModeScrollView.Add(newArrowEntry);
            }

            var newListEntry = artifactModeAsset.Instantiate();
            var visualElement = newListEntry.Q<VisualElement>("ArtifactMode");
            var isSelected = i == heroCurrentArtifactModeIndex;
            var artifactModeInfo = _allArtifactModeInfos.Find(ami => ami.artifactMode == artifactMode);
            newListEntry.transform.rotation = Quaternion.Euler(0f, 0f, artifactModeInfo.rotation);
            if (isSelected)
            {
                visualElement.style.backgroundImage = artifactModeInfo.selectedTexture;
                visualElement.style.opacity = 1f;
            }
            else
            {
                visualElement.style.backgroundImage = artifactModeInfo.unselectedTexture;
                visualElement.style.opacity = 0.3f;
            }

            _artifactModeScrollView.Add(newListEntry);
        }
    }
}