using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections.Generic;

[Serializable]
public class Ability : ScriptableObject
{
    public string aName = "New Ability";
    public Sprite aSprite;
    public SimpleAudioEvent audioEvent;
    public float aBaseCooldown = 1f;
    public float tellTime;
    public float cameraRange=1f;
    
    // Abstract methods (have swapped to virtual though due to seriazliation issues with abstract classes)
    public virtual void Initialise(GameObject obj, List<AbilityPowerup> powerups) { }
    public virtual void TriggerAbility(Vector3 direction) { }

    //public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }HideFlags.DontUnloadUnusedAsset

    public bool CanUsePowerup(AbilityPowerup powerup)
    {
        // Check if ability has the properties required to be upgraded by the given powerup
        for (int i=0;i<powerup.properties.Length;i++)
        {
            var property = this.GetType().GetField(powerup.properties[i]);
            if (property == null)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns the total extra powerups associated with a list of powerups to a given property (matching on name)
    /// </summary>
    /// <param name="powerups"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public float PowerupThisProperty(string property, List<AbilityPowerup> powerups)
    {
        if (powerups == null)
            return 0;
        float tmp = 0;
        for (int i=0;i<powerups.Count;i++)
        {
            for (int j = 0; j < powerups[i].properties.Length; j++)
            {
                if (property.Equals(powerups[i].properties[j]))
                    tmp = tmp + powerups[i].values[j];
            }
        }
        return tmp;
    }

    /// <summary>
    /// Returns the total extra powerups associated with a list of powerups to a given property (matching on name)
    /// Assumes the powerups are multiplicative (ie 1 = no change, 2 = double)
    /// </summary>
    /// <param name="powerups"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public float PowerupThisPropertyMultiply(string property, List<AbilityPowerup> powerups)
    {
        if (powerups == null)
            return 1;
        float tmp = 1;
        for (int i = 0; i < powerups.Count; i++)
        {
            for (int j = 0; j < powerups[i].properties.Length; j++)
            {
                if (property.Equals(powerups[i].properties[j]))
                    tmp = tmp * powerups[i].values[j];
            }
        }
        return tmp;
    }



    /*
    public void Powerup(AbilityPowerup powerup)
    {
        // Check if ability has the properties required to be upgraded by the given powerup
        for (int i = 0; i < powerup.properties.Length; i++)
        {
            string property = powerup.properties[i];
            var value = powerup.values[i];
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty prop = serializedObject.FindProperty(property);
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.SetValue((int)value);
                    break;
            }
            serializedObject.ApplyModifiedProperties();

            var val = Convert.ChangeType(value, prop.propertyType);
            if (prop == null)
                return;
            prop.SetValue();

            //PropertyInfo prop = this.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo prop = this.GetType().GetProperty(property);
            if (null != prop && prop.CanWrite)
            {
                var val = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(this, val, null);
            }
        }
    }
    */
}