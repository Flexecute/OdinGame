using UnityEngine;
using UnityEngine.UI;

public class CheckUsername : MonoBehaviour
{
    public GameObject usernameTextBox;
    public GameObject acceptButton;
    public GameObject continueButton;

    private InputField usernameText;

    public void Start()
    {
        usernameText = usernameTextBox.GetComponent<InputField>();
    }

    public void checkUsername()
    {
        if (usernameText.text.Length > 0)
        {
            acceptButton.SetActive(true);
        } else
        {
            acceptButton.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the players username
    /// </summary>
    public void SaveUsername()
    {
        PlayerData playerData = PlayerData.Instance;
        string newName = usernameText.text;
        playerData.username = newName;
    }
}
