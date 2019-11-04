using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{
    static public Utils _instance;
    public static Utils Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
    }

    public static void DestroyObjectAfterTime(GameObject dObject, float time)
    {
        _instance.StartCoroutine(DestroyAfterTime(dObject, time));
    }

    private static IEnumerator DestroyAfterTime(GameObject dObject, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(dObject);
    }
}
