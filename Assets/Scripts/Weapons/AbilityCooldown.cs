using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityCooldown : MonoBehaviour
{

    public string abilityButtonAxisName = "Fire1";
    public Image darkMask;
    //public Text coolDownTextDisplay;

    private Ability ability;
    private GameObject weaponHolder;
    private Image myButtonImage;
    private AudioSource audioSource;
    private float coolDownDuration;
    private float nextReadyTime;
    private float coolDownTimeLeft;
    private float tellTime;
    private PlayerAnimationController animator;
    private AudioEvent audioEvent;

    private Camera mainCamera;
    private Plane plane;
    //private PlayerAnimationController animator;

/*    void Start()
    {
        if (ability != null)
            Initialise(ability, weaponHolder);
    }
*/

    public void Initialise(Ability selectedAbility, GameObject wh)
    {
        weaponHolder = wh;
        mainCamera = Camera.main;
        // Create plane at weaponHolder position for identifying where the mouse click is
        plane = new Plane(Vector3.up, weaponHolder.transform.position);
        // Find animator on weapon
        animator = weaponHolder.transform.parent.GetComponentInChildren<PlayerAnimationController>();
        // Find the power ups associated with this ability on the player
        PlayerData playerData = PlayerData.Instance;
        List<AbilityPowerup> abilityPowerups = playerData.GetAbilityPowerups(selectedAbility);
        // Update the camera view if this ability has the cameraView (un)powerup
        foreach (AbilityPowerup powerup in abilityPowerups)
        {
            for (int i=0;i<powerup.properties.Length;i++)
            {
                if (powerup.properties[i] == "cameraRange")
                {
                    mainCamera.fieldOfView = mainCamera.fieldOfView * powerup.values[i];
                }
            }
        }
        ability = selectedAbility;
        myButtonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        coolDownDuration = ability.aBaseCooldown * ability.PowerupThisPropertyMultiply("aBaseCooldown", abilityPowerups); ;
        tellTime = ability.tellTime;
        audioEvent = ability.audioEvent;
        ability.Initialise(weaponHolder, abilityPowerups);
        AbilityReady();
    }

    // Update is called once per frame
    void Update()
    {
        bool coolDownComplete = (Time.time > nextReadyTime);
        if (coolDownComplete)
        {
            AbilityReady();
            if (Input.GetButton(abilityButtonAxisName))
            {
                ButtonTriggered();
            }
        } else
        {
            CoolDown();
        }
    }

    private void AbilityReady()
    {
        //coolDownTextDisplay.enabled = false;
        darkMask.enabled = false;
    }

    private void CoolDown()
    {
        coolDownTimeLeft -= Time.deltaTime;
        float roundedCd = Mathf.Round(coolDownTimeLeft);
        //coolDownTextDisplay.text = roundedCd.ToString();
        darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    }

    private void ButtonTriggered()
    {
        nextReadyTime = coolDownDuration + Time.time;
        coolDownTimeLeft = coolDownDuration;
        darkMask.enabled = true;
        //coolDownTextDisplay.enabled = true;

        // Play sound if required
        if (audioEvent != null)
            audioEvent.Play(audioSource);
        Vector3 mousePosition = GetMousePositionInGame(Input.mousePosition);
        // Rotate and animate attack if required
        if (animator != null)
        {
            Quaternion newDirection = Quaternion.LookRotation(new Vector3(mousePosition.x - weaponHolder.transform.position.x, 0, mousePosition.z - weaponHolder.transform.position.z));
            animator.AnimateRotation(newDirection, true);
            // Set the animation Attack to make char attack
            animator.AnimateAttack(true);
        }
        StartCoroutine(AnimationAttack(mousePosition, tellTime));
    }

    private IEnumerator AnimationAttack(Vector3 mousePosition, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        //Debug.Log("Actual attack: " + timer);
        Vector3 attackDirection = new Vector3(mousePosition.x - weaponHolder.transform.position.x, 0, mousePosition.z - weaponHolder.transform.position.z);
        //Debug.DrawRay(firePoint.position, attackDirection, Color.red, 5f);
        ability.TriggerAbility(attackDirection);
    }

    private Vector3 GetMousePositionInGame(Vector3 clickPos)
    {

        Ray mouseRay = mainCamera.ScreenPointToRay(clickPos);
        // Determine distance along 'y' plane that the player is in that this ray intersects
        float rayDistance;
        plane.Raycast(mouseRay, out rayDistance);
        return(mouseRay.origin + mouseRay.direction * rayDistance);
    }
}