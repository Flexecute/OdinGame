using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(Weapon))]
public class PlayerAttack : MonoBehaviour
{
    private float attackRate = 0.3f;

    [SerializeField]
    private GameObject weaponPrefab;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private string fireButton;
    public float tellTime;

    public Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>(); 

    private float timer;

    private PlayerAnimationController animator;
    private Camera mainCamera;
    private Plane plane;
    private Weapon weapon;

    private void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimationController>();
        plane = new Plane(Vector3.up, firePoint.position);
        //plane = new Plane(Vector3.up, 0);
        // Set up the weapon
        GameObject tmp = Instantiate(weaponPrefab, firePoint.position, firePoint.rotation);
        weapon = tmp.GetComponent<Weapon>();
        weapon.setFirePoint(firePoint, gameObject);
        attackRate = weapon.getAttackRate();
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Find any powerups for the weapon being used
        PlayerData playerData = PlayerData.Instance;
        //List<PowerUps> powerups = playerData.getWeaponPowerups(weapon);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;
        //Debug.Log("Attack update: " + timer);
        if (Input.GetButton(fireButton))
        {            
            if (timer >= attackRate)
            {
                //Debug.Log("Attack: " + timer);
                // If still animating previous attack, need to wait for that to finish
                //Debug.Log("Attack: " + timer);
                timer = 0f;
                // Point the character where the mouse currently is
                Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                // Determine distance along 'y' plane that the player is in that this ray intersects
                float rayDistance;
                plane.Raycast(mouseRay, out rayDistance);
                Vector3 look = mouseRay.origin + mouseRay.direction * rayDistance;
                Vector3 lookPosition = new Vector3(look.x, transform.position.y, look.z);
                // Check if there are any enemies close to mouse click
                /*
                Collider[] colliders = Physics.OverlapSphere(lookPosition, 3f, weapon.getShootableLayerMask());
                Collider closestEnemy = null;
                foreach (Collider collider in colliders) {
                    // Find the closest one to the click ..
                    closestEnemy = collider;
                }
                if (closestEnemy != null)
                    lookPosition = closestEnemy.transform.position;
                */    
                Quaternion newDirection = Quaternion.LookRotation(new Vector3(look.x - transform.position.x, 0, look.z - transform.position.z));
                animator.AnimateRotation(newDirection, true);
                // Set the animation Attack to make char attack
                animator.AnimateAttack(true);
                // Start a co-routine to actually attack after the 'tell time'
                StartCoroutine(AnimationAttack(lookPosition, tellTime));
            }
        }

        /*
        if (Input.GetButtonUp(fireButton))
        {
            // Set the animation Attack to make char attack
            animator.AnimateAttack(false);
        }
        */
    }

    private IEnumerator AnimationAttack(Vector3 mousePosition, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        //Debug.Log("Actual attack: " + timer);
        Vector3 attackDirection = new Vector3(mousePosition.x - firePoint.position.x, 0, mousePosition.z - firePoint.position.z);
        //Debug.DrawRay(firePoint.position, attackDirection, Color.red, 5f);
        weapon.Attack(attackDirection);
    }


}

