using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float startCooldown = 0.5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject bulletPrefab;

    // Event triggered when the player reaches the end of the level
    public event System.Action OnReachedEndOfLevel;

    private float shotCooldown;
    private bool isFiring;
    private float lastShotTime;

    public event System.Action OnPlayerHitByBullet; // New event for player being hit by a bullet


    private void Start()
    {
        shotCooldown = 0;
        isFiring = false;
        lastShotTime = -startCooldown; // Set initial value to allow the first shot immediately
    }

    private void Update()
    {
        ProcessMovementInput();
        ProcessShootingInput();
    }

    private void ProcessMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(0.0f, 0.0f, verticalInput * speed * Time.deltaTime);
        transform.Rotate(0.0f, horizontalInput * speed * Time.deltaTime, 0.0f);

    }

    private void ProcessShootingInput()
    {
        shotCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastShotTime >= startCooldown)
        {
            isFiring = true;
            FireBullet();
            lastShotTime = Time.time; // Update the last shot time
            shotCooldown = startCooldown; // Reset the shot cooldown
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isFiring = false;
        }

        if (isFiring && shotCooldown < 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Vector3 bulletTarget = new Vector3(hit.point.x, 0.5f, hit.point.z);
                shotCooldown = startCooldown;
                lastShotTime = Time.time; // Update the last shot time
                FireBullet(bulletTarget);
            }
        }
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        // Check if the player has been hit by a bullet
        if (hitCollider.tag == "EnemyBullet")
        {
            // Trigger the event for handling player hit by a bullet
            if (OnPlayerHitByBullet != null)
            {
                OnPlayerHitByBullet();
            }

            // Perform actions when hit by a bullet
            HandlePlayerHitByBullet();
        }

        // Check if the player has reached the end of the level
        else if (hitCollider.tag == "Finish")
        {
            // Trigger the event if it has subscribers
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    private void HandlePlayerHitByBullet()
    {
        // Perform actions when the player is hit by a bullet
        // For example, you can make the player disappear or destroy it
        Destroy(gameObject); // Destroy the player GameObject
        // Alternatively, you can deactivate the player object if you want to reuse it later
        // gameObject.SetActive(false);
    }

    private void FireBullet(Vector3 target = default)
    {
        GameObject bulletGO = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            Vector3 direction = (target - transform.position).normalized;
            bulletScript.SetDirection(direction);

            // Destroy the bullet after a certain time (adjust the time based on your needs)
            Destroy(bulletGO, 2f);
        }
        else
        {
            Debug.LogError("Bullet script not found on the bullet prefab.");
        }
    }
}
