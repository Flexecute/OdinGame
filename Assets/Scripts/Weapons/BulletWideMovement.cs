using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just like a bullet only wide! Rather than a line / point bullet, this represents a 'thick line' or plane moving through space
/// </summary>
public class BulletWideMovement : MonoBehaviour
{
    private float bulletSpeed = 1f;
    private int damage = 10;
    private int pierceNumber = -1; // -1 means it pierces forever
    private float bulletStickTime = 0f;
    private GameObject bulletImpactAnimation;
    private float bulletWidth = 1f;

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
    private Vector3 boxHalfExtents;
    private Quaternion boxRotation;

    private HashSet<Health> ignoreTargets = new HashSet<Health>();


    //private ParticleSystem bulletImpactAnimation;

    public void Initiailise(Vector3 initDirection, float initSpeed, int initDamage, float weaponRange, int pierce, int layerMask, float bWidth)
    {
        bulletSpeed = initSpeed;
        damage = initDamage;
        direction = initDirection.normalized;
        prevPos = transform.position;
        translation = direction * bulletSpeed;
        shootableLayerMask = layerMask;
        bulletLifeTime = weaponRange / initSpeed / fps;
        pierceNumber = pierce;
        bulletWidth = bWidth;
        // Destroy bullet after certain amount of time
        Invoke("DestroyObject", bulletLifeTime);

        // Work out the collider box parameters to be checked every frame
        boxHalfExtents = new Vector3(bulletWidth/2, 0.5f, bulletSpeed);
        boxRotation = transform.rotation;
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            // Move bullet
            transform.Translate(translation, Space.World);
            // Did we hit anything?
            Collider[] hitColliders = Physics.OverlapBox(prevPos, boxHalfExtents, boxRotation, shootableLayerMask);
            //ExtDebug.DrawBox(prevPos, boxHalfExtents, boxRotation, Color.red);
            //Debug.Log(Time.deltaTime + " " + prevPos.ToString());
            int i = 0;
            //Check when there is a new collider coming into contact with the box
            while (i < hitColliders.Length)
            {
                // Reduce the health of the 'shootable' if it has health
                var health = hitColliders[i].GetComponent<Health>();
                // Can only hit each target once
                if (health != null && !ignoreTargets.Contains(health))
                {
                    health.TakeDamage(damage);
                    ignoreTargets.Add(health);
                    // Knockback
                    //hits[i].collider.transform.parent.position += direction;
                }
                i++;
                currentPierced++;
            }
            // Can we pierce? (Yes)
            if (pierceNumber > 0 && currentPierced > pierceNumber)
            {
                isActive = false;
                // Attach the bullet to the first thing we hit
                transform.SetParent(hitColliders[0].transform);
                //Invoke("Kill", bulletStickTime);
                //StartCoroutine(Kill());
                Kill();
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
