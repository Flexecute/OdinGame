using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TestModalPanel : MonoBehaviour
{

    public Sprite icon;
    public Transform spawnPoint;
    public GameObject thingToSpawn;
    public DisplayManager displayManager;

    private ModalPanel modalPanel;

    void Awake()
    {
        modalPanel = ModalPanel.Instance();
    }

    //    Send to the Modal Panel to set up the Buttons and functions to call
    public void TestC()
    {
        ModalPanelDetails modalPanelDetails = new ModalPanelDetails();
        modalPanelDetails.question = "This is an announcement!\nIf you don't like it, shove off!";
        modalPanelDetails.button1Details = new EventButtonDetails();
        modalPanelDetails.button1Details.buttonTitle = "Gotcha!";
        modalPanelDetails.button1Details.action = TestCancelFunction;

        modalPanel.NewChoice(modalPanelDetails);
    }
    public void TestCI()
    {
        ModalPanelDetails modalPanelDetails = new ModalPanelDetails { question = "This is an announcement!\nIf you don't like it, shove off!", iconImage = icon };
        modalPanelDetails.button1Details = new EventButtonDetails { buttonTitle = "Gotcha!", action = TestCancelFunction };

        modalPanel.NewChoice(modalPanelDetails);
    }

    //    public void TestYN () {
    //        modalPanel.Choice ("Cheese on your burger?", TestYesFunction, TestNoFunction);
    //    }
    //    
    //    public void TestYNC () {
    //        modalPanel.Choice ("Would you like a poke in the eye?\nHow about with a sharp stick?", TestYesFunction, TestNoFunction, TestCancelFunction);
    //    }
    //    
    //    public void TestYNI () {
    //        modalPanel.Choice ("Do you like this icon?", icon, TestYesFunction, TestNoFunction, TestCancelFunction);
    //    }
    //    
    //    public void TestYNCI () {
    //        modalPanel.Choice ("Do you want to use this icon?", icon, TestYesFunction, TestNoFunction, TestCancelFunction);
    //    }
    //    
    //    public void TestLambda () {
    //        modalPanel.Choice ("Do you want to create this sphere?", () => { InstantiateObject(thingToSpawn); }, TestNoFunction);
    //    }
    //    
    //    public void TestLambda2 () {
    //        modalPanel.Choice ("Do you want to create two spheres?", () => { InstantiateObject(thingToSpawn, thingToSpawn); }, TestNoFunction);
    //    }

    public void TestLambda3()
    {
        ModalPanelDetails modalPanelDetails = new ModalPanelDetails { question = "Do you want to create three spheres?" };
        modalPanelDetails.button1Details = new EventButtonDetails
        {
            buttonTitle = "Yes Please!",
            action = () => { InstantiateObject(thingToSpawn); InstantiateObject(thingToSpawn, thingToSpawn); }
        };
        modalPanelDetails.button2Details = new EventButtonDetails
        {
            buttonTitle = "No thanks!",
            action = TestNoFunction
        };

        modalPanel.NewChoice(modalPanelDetails);
    }

    //    The function to call when the button is clicked
    void TestYesFunction()
    {
        displayManager.DisplayMessage("Heck, yeah!");
    }

    void TestNoFunction()
    {
        displayManager.DisplayMessage("No way, Jose!");
    }

    void TestCancelFunction()
    {
        displayManager.DisplayMessage("I give up!");
    }

    void InstantiateObject(GameObject thingToInstantiate)
    {
        displayManager.DisplayMessage("Here you go!");
        Instantiate(thingToInstantiate, spawnPoint.position, spawnPoint.rotation);
    }

    void InstantiateObject(GameObject thingToInstantiate, GameObject thingToInstantiate2)
    {
        displayManager.DisplayMessage("Here you go!");
        Instantiate(thingToInstantiate, spawnPoint.position - Vector3.one, spawnPoint.rotation);
        Instantiate(thingToInstantiate2, spawnPoint.position + Vector3.one, spawnPoint.rotation);
    }
}