using UnityEngine;
using UnityEditor;
using System;

[Serializable]
[CreateAssetMenu(menuName = "Abilities/Powerup")]
public class AbilityPowerup : ScriptableObject
{
    public string title;
    public string[] properties;
    public float[] values;
    public Sprite sprite;
    public string infoText;

    //public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }

}