using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.HID;
using Unity.Burst.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    InputManager inputManager;
    [SerializeField]
    ScoreManager scoreManager;
    [SerializeField]
    EnemySpawnManager enemySpawnManager;

    [SerializeField]
    bool reloadInput;
    [SerializeField]
    bool shootInput;
    [SerializeField]
    Vector2 mouseInput;

    bool shooting;
    bool reloading;
    bool canShoot;
    bool canReload;

    [SerializeField]
    int maxAmmo;
    int ammo;

    public float zAxis;
    Vector3 mousePos;


    [SerializeField]
    Image playerReticleImage;
    [SerializeField]
    GameObject playerReticle;
    [SerializeField]
    GameObject player;
    [SerializeField]
    TextMeshProUGUI ammoCount;

    public Camera polaroidCamera;

    float delta;

    [SerializeField]
    float shootCooldown;
    [SerializeField]
    float currentShootCooldown;
    bool shootCooldownActive;

    [SerializeField]
    float reloadCooldown;
    [SerializeField]
    float currentReloadCooldown;
    bool reloadCooldownActive;

    
    public float defaultPlayerSpeed;
    public float playerSlowSpeed;


    // Start is called before the first frame update
    void Start()
    {
        ammo = maxAmmo;
        currentShootCooldown = 0;
        currentReloadCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs(); //retrieve inputs from input manager script
        CheckAmmo(); //check ammo state and update booleans appropriately
        HandleShoot(); //handle shoot action
        HandleReload(); //handle reload action

        delta = Time.deltaTime;

        if (shooting && shootCooldownActive)
        {
            if (currentShootCooldown < shootCooldown && !shootInput)
            {
                currentShootCooldown += delta;
            }
            else if(currentShootCooldown >= shootCooldown)
            {
                shooting = false;
                currentShootCooldown = 0;
                playerReticleImage.color = Color.green;
            }
        }

        if (reloading && reloadCooldownActive)
        {
            if (currentReloadCooldown < reloadCooldown)
            {
                currentReloadCooldown += delta;
                playerReticleImage.color = Color.yellow;
            }
            else
            {
                reloading = false;
                ammo = maxAmmo;
                currentReloadCooldown = 0;
                playerReticleImage.color = Color.green;
            }
        }

        ammoCount.text = ammo.ToString();
        //playerReticleImage.transform.position = mouseInput;
        mousePos = new Vector3(mouseInput.x, mouseInput.y, zAxis);
        playerReticle.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }


    private void GetInputs()
    {
        mouseInput = inputManager.GetMouseInput();
        shootInput = inputManager.GetLClickInput();
        reloadInput = inputManager.GetRClickInput();
        
    }

    private void HandleShoot()
    {
        if(reloading)
        {
            canShoot = false;
            return;
        }

        if(canShoot && shootInput)
        {
            print(canShoot);
            playerReticleImage.color = Color.red;
            ammo -= 1;
            canShoot = false;
            shooting = true;
            shootCooldownActive = true;
            DetectEnemies();
        }
    }

    private void HandleReload()
    {
        if (shooting)
        {
            canReload = false;
            return;
        }

        if (canReload && reloadInput)
        {
            reloading = true;
            reloadCooldownActive = true;
        }
    }

    private void CheckAmmo()
    {
        if(ammo > 0 && !shooting)
        {
            canShoot = true;
        }
        if(ammo < maxAmmo)
        {
            canReload = true;
        }
        if(ammo <= 0)
        {
            canShoot = false;
            playerReticleImage.color = Color.yellow;
        }
        if(ammo >= maxAmmo)
        {
            canReload = false;
        }
    }
    private void DetectEnemies()
    {
        Ray ray = new(polaroidCamera.transform.position, polaroidCamera.transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray.origin, sphereCastRadius, ray.direction * range, out hit, range, layerMask))
        {
            CheckEnemyType(hit);
        }
    }

    private void CheckEnemyType(RaycastHit hitEnemy)
    {
        Enemy enemy = hitEnemy.transform.GetComponent<Enemy>();

        if ((int)enemy.ghostType == 0)
        {
            print("Real!");
            scoreManager.AddPoints(enemy.ghostPointValue);
            
        }
        else if ((int)enemy.ghostType == 1)
        {
            print("Fake!");
            scoreManager.RemovePoints(enemy.ghostPointValue);
        }
        GameObject.Destroy(enemy.gameObject);

    }

    [Range(0.1f, 1f)] public float sphereCastRadius;
    [Range(1f, 100f)] public float range;
    public LayerMask layerMask;
    private void OnDrawGizmos()
    {
        Ray ray = new(polaroidCamera.transform.position, polaroidCamera.transform.forward);
        RaycastHit hit;

        Gizmos.DrawWireSphere(transform.position, range);

        if (Physics.SphereCast(ray.origin, sphereCastRadius, ray.direction * range, out hit, range, layerMask))
        {
            Gizmos.color = Color.green;
            Vector3 sphereCastMidpoint = ray.origin + (ray.direction * hit.distance);
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Gizmos.DrawSphere(hit.point, 0.1f);
            Debug.DrawLine(ray.origin, sphereCastMidpoint, Color.green);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = ray.origin + (ray.direction * (range - sphereCastRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Debug.DrawLine(ray.origin, sphereCastMidpoint, Color.red);
        }
    }
}
