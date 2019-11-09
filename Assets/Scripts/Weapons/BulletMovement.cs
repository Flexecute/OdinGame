using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed = 1f;
    [SerializeField]
    private int damage = 10;
    [SerializeField]
    private int pierceNumber = 0;
    [SerializeField]
    private float bulletStickTime = 0f;
    [SerializeField]
    private GameObject bulletImpactAnimation;

    private float bulletLifeTime = 5f;
    // Normalised direction of the bullet
    private Vector3 direction = new Vector3(0.0f,0.0f,1f);
    // Actual vector that the bullet will be translated by each update
    private Vector3 translation;
    private Vector3 prevPos;
    private int shootableLayerMask;
    private int currentPierced=0;
    private bool isActive=true;
    private float fps = 60; // Used to calculate bullet lifetime

    
    //private ParticleSystem bulletImpactAnimation;

    public void Initiailise(Vector3 initDirection, float initSpeed, int initDamage, float weaponRange, int pierce, int layerMask)
    {

        bulletSpeed = initSpeed;
        damage = initDamage;
        direction = initDirection.normalized;
        prevPos = transform.position;
        translation = direction * bulletSpeed;
        shootableLayerMask = layerMask;
        bulletLifeTime = weaponRange / initSpeed / fps;
        pierceNumber = pierce;

        // Destroy bullet after certain amount of time
        Invoke("DestroyObject", bulletLifeTime);

        // Find the bullet impact system from the prefab
        /*
        ParticleSystem[] pSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pSystems)
        {
            if (ps.gameObject.CompareTag("BulletImpact"))
            {
                bulletImpactAnimation = ps;
                break;
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            // Move bullet
            transform.Translate(translation, Space.World);
            // Did we hit anything?
            RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, direction), bulletSpeed, shootableLayerMask);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    // Reduce the health of the 'shootable' if it has health
                    var health = hits[i].collider.GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(damage);
                        // Knockback
                        //hits[i].collider.transform.parent.position += direction;
                    }
                }
                // Can we pierce?
                currentPierced += hits.Length;
                if (currentPierced > pierceNumber)
                {
                    isActive = false;
                    // Attach the bullet to the first thing we hit
                    transform.SetParent(hits[0].transform);
                    //Invoke("Kill", bulletStickTime);
                    //StartCoroutine(Kill());
                    Kill();
                }
            }
            prevPos = transform.position;
        }
    }

    private void Kill()
    {
        if (bulletImpactAnimation != null)
        {
            GameObject tmp = Instantiate(bulletImpactAnimation, transform.position, transform.rotation);
            ParticleSystem ps = tmp.GetComponent<ParticleSystem>();
            ps.Play();
            //StartCoroutine(Utils.DestroyObjectAfterTime(tmp, ps.main.duration));
            //Utils.DestroyObjectAfterTime(tmp, ps.main.duration);
        }
        //StartCoroutine(DestroyObjectAfterStick());
        Invoke("DestroyObject", bulletStickTime);
    }

//    IEnumerator DestroyObjectAfterStick()
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

}
