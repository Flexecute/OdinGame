using UnityEngine;
using UnityEditor;

public class TeleportLauncher : MonoBehaviour
{

    public CharacterController characterController;
    private bool disabled;

    public void Attack(Vector3 direction)
    {
        if (disabled)
            return;

        // Teleport character Controller
        Vector3 newPosition = new Vector3(transform.position.x + direction.x, 0f, transform.position.z + direction.z);

        characterController.enabled = false;
        characterController.transform.position = newPosition;
        characterController.enabled = true;

    }

    void OnDisable()
    {
        disabled = true;
    }


}