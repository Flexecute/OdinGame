using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Powerup : MonoBehaviour
{
    public PowerupEffect effect;
    public GameObject animationOnPickup;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other);
        }        
    }

    private void Pickup(Collider player)
    {
        // Display animation
        if (animationOnPickup != null)
            Instantiate(animationOnPickup, transform.position, transform.rotation);
        effect.Apply(player.gameObject);
        Destroy(gameObject);
    }
}
