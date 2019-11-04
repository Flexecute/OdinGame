using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyShader : MonoBehaviour
{

    [SerializeField]
    private float transparentDistance = 5f;

    [SerializeField]
    private float zOffset = 20f;
    public float transparencyAmount = 0.4f;

    private Camera mainCamera;
    private Material[] materials;
    private bool transparent;

    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main;
        Renderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        // Create new array for materials and defaultShaders
        materials = new Material[renderers.Length];
        int i = 0;
        foreach (Renderer renderer in renderers) 
        {
            materials[i] = renderer.material;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transparent) 
        {
            if (Mathf.Abs(mainCamera.transform.position.x - transform.position.x) > transparentDistance || Mathf.Abs(mainCamera.transform.position.z - transform.position.z + zOffset) > transparentDistance) 
            {
                transparent = false;
                revertShaders();
            }
        } else 
        {
            if (Mathf.Abs(mainCamera.transform.position.x - transform.position.x) <= transparentDistance && Mathf.Abs(mainCamera.transform.position.z - transform.position.z + zOffset) <= transparentDistance) {
                transparent = true;
                changeShaders(transparencyAmount);
            }
        }
    }

    private void changeShaders(float alpha) 
    {
        for (int i = 0;i<materials.Length;i++) 
        {
            Color color = materials[i].color;
            color.a = alpha;
            materials[i].color = color;// new Color(1, 1, 1, alpha);
        }
    }

    private void revertShaders() {
        for (int i = 0; i < materials.Length; i++) {
            Color color = materials[i].color;
            color.a = 1;
            materials[i].color = color; // new Color(1, 1, 1, 1);
        }
    }
}
