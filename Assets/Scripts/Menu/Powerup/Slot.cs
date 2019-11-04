using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text infoText;
    public Text infoTitle;    
    public Ability ability;

    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }

            return null;
        }
    }
    /*
    public void Start()
    {
        realInfoText = infoText.GetComponent<Text>();
    }
    */

    public void OnDrop(PointerEventData eventData)
    {
        if (!item)
        {
            PowerupPointer pp = DragHandler.item.GetComponent<PowerupPointer>();
            AbilityPowerup powerup = pp.powerup;
            // Can this ability take this powerup?
            if (ability == null || ability.CanUsePowerup(powerup))
            {
                DragHandler.item.transform.SetParent(transform);
            } else
            {
                infoText.text = "Unable to power up " + ability.name + " with " + powerup.name;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.item != null)
        {
            AbilityPowerup powerup = item.GetComponent<PowerupPointer>().powerup;
            if (powerup != null)
            {
                infoText.text = powerup.infoText;
                infoTitle.text = powerup.title;
            }
        }
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        //Debug.Log("Mouse is no longer on GameObject.");
    }
}