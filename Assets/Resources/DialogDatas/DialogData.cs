using System.Collections.Generic;
using UnityEngine;

public enum TextType
{
    Caption,
    Normal,
}

[CreateAssetMenu]
public class DialogData : ScriptableObject
{
    public List<string> titles;
    public List<string> details;
    public List<TextType> formats;
}
