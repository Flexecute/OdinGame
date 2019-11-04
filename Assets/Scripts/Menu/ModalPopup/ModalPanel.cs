using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class EventButtonDetails
{
    public string buttonTitle;
    public Sprite buttonBackground;  // Not implemented
    public UnityAction action;
}

public class EventSliderDetails
{

}

public class ModalPanelDetails
{
    public string title; // Not implemented
    public string question;
    public Sprite iconImage;
    public Sprite panelBackgroundImage; // Not implemented
    public EventButtonDetails button1Details;
    public EventButtonDetails button2Details;
    public EventButtonDetails button3Details;
    public EventButtonDetails button4Details;
    public EventSliderDetails sliderDetails;
}


public class ModalPanel : MonoBehaviour
{

    public Text question;
    public Image iconImage;
    public Button button1;
    public Button button2;
    public Button button3;

    public Text button1Text;
    public Text button2Text;
    public Text button3Text;

    public GameObject modalPanelObject;

    private static ModalPanel modalPanel;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }


    //    //    Announcement with Image:  A string, a Sprite and Cancel event;
    //    public void Choice (string question, UnityAction cancelEvent, Sprite iconImage = null) {
    //        modalPanelObject.SetActive (true);
    //        
    //        button3.onClick.RemoveAllListeners();
    //        button3.onClick.AddListener (cancelEvent);
    //        button3.onClick.AddListener (ClosePanel);
    //        
    //        this.question.text = question;
    //        if (iconImage)
    //            this.iconImage.sprite = iconImage;
    //        
    //        if (iconImage)
    //            this.iconImage.gameObject.SetActive(true);
    //        else
    //            this.iconImage.gameObject.SetActive(false);
    //        button1.gameObject.SetActive(false);
    //        button2.gameObject.SetActive(false);
    //        button3.gameObject.SetActive(true);
    //    }

    public void NewChoice(ModalPanelDetails details)
    {
        modalPanelObject.SetActive(true);

        this.iconImage.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);

        this.question.text = details.question;

        if (details.iconImage)
        {
            this.iconImage.sprite = details.iconImage;
            this.iconImage.gameObject.SetActive(true);
        }

        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(details.button1Details.action);
        button1.onClick.AddListener(ClosePanel);
        button1Text.text = details.button1Details.buttonTitle;
        button1.gameObject.SetActive(true);

        if (details.button2Details != null)
        {
            button2.onClick.RemoveAllListeners();
            button2.onClick.AddListener(details.button2Details.action);
            button2.onClick.AddListener(ClosePanel);
            button2Text.text = details.button2Details.buttonTitle;
            button2.gameObject.SetActive(true);
        }

        if (details.button3Details != null)
        {
            button3.onClick.RemoveAllListeners();
            button3.onClick.AddListener(details.button3Details.action);
            button3.onClick.AddListener(ClosePanel);
            button3Text.text = details.button3Details.buttonTitle;
            button3.gameObject.SetActive(true);
        }
    }

    //    //    Yes/No: A string, a Yes event, a No event (No Cancel Button);
    //    public void Choice (string question, UnityAction yesEvent, UnityAction noEvent) {
    //        modalPanelObject.SetActive (true);
    //
    //        button1.onClick.RemoveAllListeners();
    //        button1.onClick.AddListener (yesEvent);
    //        button1.onClick.AddListener (ClosePanel);
    //
    //        button2.onClick.RemoveAllListeners();
    //        button2.onClick.AddListener (noEvent);
    //        button2.onClick.AddListener (ClosePanel);
    //
    //        this.question.text = question;
    //        
    //        this.iconImage.gameObject.SetActive(false);
    //        button1.gameObject.SetActive(true);
    //        button2.gameObject.SetActive(true);
    //        button3.gameObject.SetActive(false);
    //    }
    //    
    //    //    Yes/No/Cancel: A string, a Yes event, a No event and Cancel event;
    //    public void Choice (string question, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent) {
    //        modalPanelObject.SetActive (true);
    //        
    //        button1.onClick.RemoveAllListeners();
    //        button1.onClick.AddListener (yesEvent);
    //        button1.onClick.AddListener (ClosePanel);
    //        
    //        button2.onClick.RemoveAllListeners();
    //        button2.onClick.AddListener (noEvent);
    //        button2.onClick.AddListener (ClosePanel);
    //        
    //        button3.onClick.RemoveAllListeners();
    //        button3.onClick.AddListener (cancelEvent);
    //        button3.onClick.AddListener (ClosePanel);
    //        
    //        this.question.text = question;
    //        
    //        this.iconImage.gameObject.SetActive(false);
    //        button1.gameObject.SetActive(true);
    //        button2.gameObject.SetActive(true);
    //        button3.gameObject.SetActive(true);
    //    }
    //    
    //    //    Yes/No with Image: A string, a Sprite, a Yes event, a No event (No Cancel Button);
    //    public void Choice (string question, Sprite iconImage, UnityAction yesEvent, UnityAction noEvent) {
    //        modalPanelObject.SetActive (true);
    //        
    //        button1.onClick.RemoveAllListeners();
    //        button1.onClick.AddListener (yesEvent);
    //        button1.onClick.AddListener (ClosePanel);
    //        
    //        button2.onClick.RemoveAllListeners();
    //        button2.onClick.AddListener (noEvent);
    //        button2.onClick.AddListener (ClosePanel);
    //        
    //        this.question.text = question;
    //        this.iconImage.sprite = iconImage;
    //        
    //        this.iconImage.gameObject.SetActive(true);
    //        button1.gameObject.SetActive(true);
    //        button2.gameObject.SetActive(true);
    //        button3.gameObject.SetActive(false);
    //    }
    //    
    //    //    Yes/No/Cancel with Image: A string, a Sprite, a Yes event, a No event and Cancel event;
    //    public void Choice (string question, Sprite iconImage, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent) {
    //        modalPanelObject.SetActive (true);
    //        
    //        button1.onClick.RemoveAllListeners();
    //        button1.onClick.AddListener (yesEvent);
    //        button1.onClick.AddListener (ClosePanel);
    //        
    //        button2.onClick.RemoveAllListeners();
    //        button2.onClick.AddListener (noEvent);
    //        button2.onClick.AddListener (ClosePanel);
    //        
    //        button3.onClick.RemoveAllListeners();
    //        button3.onClick.AddListener (cancelEvent);
    //        button3.onClick.AddListener (ClosePanel);
    //        
    //        this.question.text = question;
    //        this.iconImage.sprite = iconImage;
    //        
    //        this.iconImage.gameObject.SetActive(true);
    //        button1.gameObject.SetActive(true);
    //        button2.gameObject.SetActive(true);
    //        button3.gameObject.SetActive(true);
    //    }

    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}