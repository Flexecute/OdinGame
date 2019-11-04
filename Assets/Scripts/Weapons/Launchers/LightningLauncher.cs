using UnityEngine;
using UnityEditor;
using DigitalRuby.LightningBolt;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightningLauncher : MonoBehaviour
{
    // These properties should be set by the Ability on Initialise
    [HideInInspector] public int damage = 1;
    [HideInInspector] public float weaponRange = 10f;
    [HideInInspector] public int forks = 2;
    [HideInInspector] public int[] shootableLayers;
    public GameObject lightningPrefab;
    [HideInInspector] public int extraShots;
    public int multishotSpreadAngle = 5;

    public float forkTime=0.05f;
    public ParticleSystem gunshotAnimation;
    private int shootableLayerMask;
    //private LightningBoltScript lbScript;
    private List<LightningBoltScript> forkObjects;
    private bool disabled;
    private int currentForkPointer;

    public void Initialise()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask = shootableLayerMask + (1 << layer);
        }

        // Create a lightning prefab for as many forks and extra shots as is required
        GameObject tmp;
        // Find the lbScript from the lightningPrefab
        forkObjects = new List<LightningBoltScript>();
        // Create more prefabs (line renderers) for each extra shot
        for (int i=0;i<(forks+1)*(extraShots +1);i++)
        {
            // Create a new prefab for this lightning fork
            tmp = Instantiate(lightningPrefab, this.transform.position, this.transform.rotation);
            LightningBoltScript tmpScript = tmp.GetComponent<LightningBoltScript>();
            forkObjects.Add(tmpScript);
        }

    }

    // Create more prefabs for each new fork
    public void setNumForks(int numForks)
    {
        for (int i = forks; i < numForks*(extraShots + 1); i++)
        {
            // Create a new prefab for this lightning fork
            GameObject tmp = Instantiate(lightningPrefab, this.transform.position, this.transform.rotation);
            LightningBoltScript tmpScript = tmp.GetComponent<LightningBoltScript>();
            forkObjects.Add(tmpScript);
        }
        forks = numForks;
    }
    

    public void Attack(Vector3 direction)
    {
        if (disabled)
            return;

        currentForkPointer = 0;
        // Create bullet for each extra shots
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Offset direction to ensure it 'averages' to required direction
        if (extraShots > 0)
            direction = Quaternion.Euler(0, -multishotSpreadAngle * extraShots / 2, 0) * direction;
        for (int i = 0; i < extraShots + 1; i++)
        {
            CreateBolt(direction);
            // Rotate the angle slightly
            direction = Quaternion.Euler(0, multishotSpreadAngle, 0) * direction;
        }
    }

    private void CreateBolt(Vector3 direction)
    {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hitInfo;

        LightningBoltScript lbScript = forkObjects[currentForkPointer++];
        lbScript.StartPosition = transform.position;
        if (Physics.Raycast(ray, out hitInfo, weaponRange, shootableLayerMask))
        {
            // Cap destination of lightning at this location
            Vector3 lastPosition = new Vector3(hitInfo.collider.transform.position.x, transform.position.y, hitInfo.collider.transform.position.z);
            lbScript.EndPosition = lastPosition;
            lbScript.Trigger();
            // Reduce the health of the 'shootable' if it has health
            var health = hitInfo.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            // Create co-routine to fork
            if (forks > 0)
            {
                //StartCoroutine(Fork(hitInfo.collider.transform));
                Fork(hitInfo.collider.transform);
            }

        } else
        {
            // Set the destination of the lightning bolt to be 'uncapped'
            lbScript.EndPosition = transform.position + direction.normalized * weaponRange;
            lbScript.Trigger();
        }
    }

    //private IEnumerator Fork(Transform originalTransform)
    private void Fork(Transform originalTransform)
    {
        Vector3 lastPosition = originalTransform.position;
        // Try to fork
        Collider[] forkColliders = Physics.OverlapSphere(lastPosition, weaponRange, shootableLayerMask);
        // Sort the colliders by distance to the original hit
        Array.Sort(forkColliders, (x, y) => PositionHelper.ClosestCompare(x.transform.position, y.transform.position, lastPosition));
        int i = 0;
        int numForks = 0;
        while (numForks < forks && i < forkColliders.Length)
        {
            //yield return new WaitForSeconds(forkTime);
            Collider collider = forkColliders[i];
            // Don't fork to the original
            if (collider.transform != originalTransform)
            {
                // Activate another fork
                LightningBoltScript tmpScript = forkObjects[currentForkPointer++];
                tmpScript.StartPosition = lastPosition;
                lastPosition = new Vector3(collider.transform.position.x, transform.position.y, collider.transform.position.z);
                tmpScript.EndPosition = lastPosition;
                // Animate flash
                tmpScript.Trigger();
                numForks = numForks + 1;
                var health2 = collider.GetComponent<Health>();
                if (health2 != null)
                {
                    health2.TakeDamage(damage);
                }
            }
            i++;
        }
        
    }

    void OnDisable()
    {
        disabled = true;
    }

}