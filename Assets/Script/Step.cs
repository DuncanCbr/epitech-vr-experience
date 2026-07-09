using UnityEngine;

[System.Serializable]
public class Step
{
    public string stepName;

    [TextArea]
    public string objective;

    public GameObject interactionObject;
}