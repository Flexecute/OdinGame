using System;
using UnityEngine;
using UnityEngine.AI;

public class AggroDetection : MonoBehaviour
{
    public event Action<Transform> OnAggro = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            OnAggro(player.transform);
        }
    }

    public void SetAggro(Transform target)
    {
        OnAggro(target);
    }

}
