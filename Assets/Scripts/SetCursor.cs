using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public bool centerHotspot = true;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 hotspot;
        if (centerHotspot)
            hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        else
            hotspot = Vector2.zero;
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

}
