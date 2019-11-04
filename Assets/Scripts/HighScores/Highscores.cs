using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Xml;

public class Highscores : MonoBehaviour
{

    static readonly string[] privateCode = { "0", "1", "2", "3", "cmIbMIbPIEupfp_auVEfiw7G1391V29U2hb8Vq5o5tZg",
                                                            "BdxnKyAFmkGNGEQiZ9VYcQr7bhi7wFvkqEuQBaPcoiKw",
                                                            "zm0tddD1j0iBD3NBTdDg0wexpR0k5nsU6d_IuTRfxm0A"};
    static readonly string[] publicCode = { "0", "1", "2", "3", "5dbff28ad1041303ecef0069", "5dc01971d1041303ecefd40e", "5dc01a20d1041303ecefd8d9"};
    const string webURL = "http://dreamlo.com/lb/";
    const int maxEntries = 10;

    private PlayerData playerData;
    DisplayHighscores highscoreDisplay;
    public Highscore[] highscoresList;
    static Highscores instance;

    void Awake()
    {
        highscoreDisplay = GetComponent<DisplayHighscores>();
        instance = this;
        playerData = PlayerData.Instance;
        // Add this new high score
        if (playerData.previousLevelTime > 0)
            AddAndRetrieveHighScores(playerData.username, 2000f / playerData.previousLevelTime, playerData.previousLevelTime, playerData.currentLevel - 1);
        else
            DownloadHighscores(playerData.currentLevel-1);
    }

    public static void AddAndRetrieveHighScores(string username, float score, float seconds, int levelNumber)
    {
        instance.StartCoroutine(instance.AddAndDownloadHighscoresFromDatabase(username, (int) Math.Round(score,0), (int) Math.Round(seconds,0), levelNumber));
    }

    IEnumerator AddAndDownloadHighscoresFromDatabase(string username, int score, int seconds, int levelNumber)
    {
        //maxEntries + "/" +
        string url = webURL + privateCode[levelNumber] + "/add-xml-seconds-asc/" + UnityWebRequest.EscapeURL(username) + "/" + score + "/" + seconds;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            print("Error Downloading: " + www.error);
        } else
        {
            FormatHighscores(www.downloadHandler.text);
            highscoreDisplay.OnHighscoresDownloaded(highscoresList);
        }
    }

    public void DownloadHighscores(int levelNumber)
    { 
        instance.StartCoroutine(instance.DownloadHighscoresFromDatabase(levelNumber));
    }

    IEnumerator DownloadHighscoresFromDatabase(int levelNumber)
    {
        string url = webURL + publicCode[levelNumber] + "/xml-seconds-asc/";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            print("Error Downloading: " + www.error);
        } else
        {
            FormatHighscores(www.downloadHandler.text);
            highscoreDisplay.OnHighscoresDownloaded(highscoresList);
        }
    }

    void FormatHighscores(string textStream)
    {

        XmlDocument doc = new XmlDocument();
        // Load the xml
        doc.LoadXml(textStream);
        // Find the entries
        XmlNodeList entries = doc.GetElementsByTagName("entry");
        highscoresList = new Highscore[entries.Count];
        for (int i = 0; i < entries.Count; i++)
        {
            XmlNode entryNode = entries[i];
            string username = entryNode["name"].InnerText;
            // Try to parse the score, ignore if it fails
            int score;
            if (!Int32.TryParse(entryNode["score"].InnerText, out score))
                continue;
            // Try to parse the score, ignore if it fails
            int seconds;
            if (!Int32.TryParse(entryNode["seconds"].InnerText, out seconds))
                continue;
            highscoresList[i] = new Highscore(username, score, seconds);
            //print(highscoresList[i].username + ": " + highscoresList[i].seconds + " sec");
        }
    }
}

public struct Highscore
{
    public string username;
    public int score;
    public int seconds;

    public Highscore(string _username, int _score, int _seconds)
    {
        username = _username;
        score = _score;
        seconds = _seconds;
    }

}