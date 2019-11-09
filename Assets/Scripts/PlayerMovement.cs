using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerAnimationController animatorController;
    [SerializeField]
    public float moveSpeed = 100;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animatorController = GetComponentInChildren<PlayerAnimationController>();
        // Factor animation speed based on difficulty to account for enemy speed up
        int difficultyLevel = PlayerData.Instance.difficultyLevel;
        if (difficultyLevel > 0)
        {
            float increaseSpeed = System.Math.Min(1.75f, difficultyLevel * (1 + (PlayerData.difficultySpeedFactor - 1) / 3));
            animatorController.SetAnimationSpeed(increaseSpeed);
            moveSpeed = moveSpeed * increaseSpeed;
        }

    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(1);
        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var movement = new Vector3(horizontal, 0, vertical);

        // Move char
        characterController.SimpleMove(movement * Time.deltaTime * moveSpeed);
        // Set the animation speed to make char run
        animatorController.AnimateSpeed(movement.magnitude);
        // Rotate char if moving
        if (movement.magnitude > 0)
        {
            Quaternion newDirection = Quaternion.LookRotation(movement);
            animatorController.AnimateRotation(newDirection, false);
        }
        
    }
}
