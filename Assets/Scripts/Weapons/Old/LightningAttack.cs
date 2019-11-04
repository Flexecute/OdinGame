using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;
using System;

public class LightningAttack : Weapon
{
    [SerializeField]
    private float length = 10f;

    [SerializeField]
    private float attackRate = 1f;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float forkRadius = 10;
    private int forks;
    private List<LightningBoltScript> forkObjects;

    [SerializeField]
    GameObject lightningPrefab;

    [SerializeField]
    private AudioSource sound;

    [SerializeField]
    public int[] shootableLayers;
    private int shootableLayerMask;

    private LightningBoltScript lbScript;

    private void Awake()
    {
        // Find the lbScript from the lightningPrefab
        lbScript = this.GetComponent<LightningBoltScript>();
        lbScript.StartObject = FirePoint;
        lbScript.EndObject = null;
        forkObjects = new List<LightningBoltScript>();
        // Bit shift the index of the layer (8) to get a bit mask
        foreach (int layer in shootableLayers)
        {
            shootableLayerMask += (1 << layer);
        }
    }
    public override float getAttackRate()
    {
        return attackRate;
    }
    public override int getShootableLayerMask() {
        return shootableLayerMask;
    }
    public override void setFirePoint(Transform newFirePoint, GameObject player)
    {
        FirePoint = newFirePoint;
        lbScript.StartObject = FirePoint;
        setNumForks(2);
    }

    public void setNumForks(int numForks)
    {
        for (int i=forks; i<numForks;i++)
        {
            // Create a new prefab for this lightning fork
            GameObject tmp = Instantiate(lightningPrefab, this.transform.position, this.transform.rotation);
            LightningBoltScript tmpScript = tmp.GetComponent<LightningBoltScript>();
            forkObjects.Add(tmpScript);
        }
        forks = numForks;
    }

    public override void Attack(Vector3 direction)
    {

        // Play sound
        if (sound != null)
            sound.Play();

        Ray ray = new Ray(FirePoint.position, direction);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, length, shootableLayerMask))
        {
            // Cap destination of lightning at this location
            Vector3 lastPosition = new Vector3(hitInfo.collider.transform.position.x, FirePoint.position.y, hitInfo.collider.transform.position.z);
            lbScript.EndPosition = lastPosition;

            // Reduce the health of the 'shootable' if it has health
            var health = hitInfo.collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            // Try to fork
            Collider[] forkColliders = Physics.OverlapSphere(hitInfo.collider.transform.position, forkRadius, shootableLayerMask);
            // Sort the colliders by distance to the original hit
            Array.Sort(forkColliders, (x, y) => ClosestCompare(x.transform.position, y.transform.position, hitInfo.collider.transform.position));
            int i = 0;
            int numForks = 0;
            while (numForks < forks && i < forkColliders.Length)
            {
                Collider collider = forkColliders[i];
                // Don't fork to the original
                if (collider.transform != hitInfo.collider.transform)
                {
                    // Activate another fork
                    //LightningBoltScript tmpScript = forkObjects[numForks];
                    LightningBoltScript tmpScript = forkObjects[0];
                    tmpScript.StartPosition = lastPosition;
                    lastPosition = new Vector3(collider.transform.position.x, FirePoint.position.y, collider.transform.position.z);
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
        } else
        {
            // Set the destination of the lightning bolt to be 'uncapped'
            lbScript.EndPosition = FirePoint.position + direction.normalized * length;
        }
        // Animate flash
        lbScript.Trigger();


    }

    /// <summary>
    /// Determines which 3d position (x or y) is closest to the referencePosition
    /// Return -1,0,1 as per a compareTo function
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="referenceTransform"></param>
    /// <returns></returns>
    private int ClosestCompare(Vector3 x, Vector3 y, Vector3 referencePosition)
    {
        float diff = Vector3.Distance(x, referencePosition) - Vector3.Distance(y, referencePosition);
        if (diff < 0)
            return -1;
        else if (diff > 0)
            return 1;
        else
            return 0;
    }
}
