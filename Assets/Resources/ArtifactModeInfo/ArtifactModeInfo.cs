using UnityEngine;

[CreateAssetMenu]
public class ArtifactModeInfo : ScriptableObject
{
    public Texture2D selectedTexture;
    public Texture2D unselectedTexture;
    public ArtifactMode artifactMode;
    public float rotation;
}