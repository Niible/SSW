using UnityEngine;

public class DynamicTextSizeUI : MonoBehaviour
{
    protected int GetFontSizeFromContainerSize(float containerSize, float percentage)
    {
        return Mathf.RoundToInt(containerSize * percentage / 100);
    }
}
