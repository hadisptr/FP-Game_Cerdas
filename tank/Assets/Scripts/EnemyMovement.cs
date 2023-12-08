using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private float detectionRange;
    [SerializeField] private float firingRange;
    [SerializeField] private float fireRateValue;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Material alertColor;
    [SerializeField] private GameObject destructionEffect;

    private Material normalColor;
    private MeshRenderer thisRenderer;
    private Vector3 targetNode;
    private Vector3 destinationNode;
    private float fireRateTimer;
    private float destinationNumber;
    private float distance;
    private int state;
    private bool isAlert;
    private GameObject player;
    private NavMeshAgent thisAgent;
    private bool isAlive = true;
    private bool isHit = false;

    void Start()
    {
        thisAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        state = 0;
        destinationNumber = 2;
        thisRenderer = GetComponent<MeshRenderer>();
        normalColor = thisRenderer.material;
        thisAgent.speed = 8f;

        Bullet bulletScript = bulletPrefab.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.OnHitEnemy += HandleEnemyHit;
        }
        else
        {
            Debug.LogError("Bullet script not found on the bullet prefab.");
        }
    }

    private void HandleEnemyHit()
    {
        if (isAlive && !isHit)
        {
            isHit = true;

            // Instantiate destruction effect (optional)
            Instantiate(destructionEffect, transform.position, Quaternion.identity);

            // Disable components and scripts
            thisAgent.enabled = false;
            GetComponent<Collider>().enabled = false;  // Disable the collider
            enabled = false;  // Disable this script
            isAlive = false;

            // Disable the entire GameObject
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isAlive || isHit)
        {
            // If the enemy is not alive or is hit, do not perform any further updates
            return;
        }

        fireRateTimer -= Time.deltaTime;

        switch (state)
        {
            case 0:
                Patrolling();
                isAlert = false;
                break;
            case 1:
                Attacking(false);
                isAlert = true;
                break;
            case 2:
                Attacking(true);
                isAlert = true;
                break;
            case 3:
                DetectPlayer();
                break;
        }

        thisAgent.SetDestination(destinationNode);
        AlertActive(isAlert);
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange)
        {
            Vector3 direction = player.transform.position - transform.position;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (distance <= firingRange)
                        state = 1; // Change the state to attacking
                    else
                        state = 2; // Change the state to approaching

                    Debug.DrawRay(transform.position, direction, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, direction, Color.red);

                    if (state == 0)
                        return;
                    else
                        state = 3; // Change the state to detect player
                }
            }
        }
    }

    void AlertActive(bool alertState)
    {
        if (alertState)
            thisRenderer.material = alertColor;
        else
            thisRenderer.material = normalColor;
    }

    void Patrolling()
    {
        if (destinationNumber == 1)
        {
            destinationNode = point1.position;
            distance = Vector3.Distance(transform.position, destinationNode);

            if (distance <= 1f)
                destinationNumber = 2;
        }
        else if (destinationNumber == 2)
        {
            destinationNode = point2.position;
            distance = Vector3.Distance(transform.position, destinationNode);

            if (distance <= 1f)
                destinationNumber = 1;
        }
    }

    void Attacking(bool isFiring)
    {
        destinationNode = player.transform.position;

        if (isFiring && fireRateTimer < 0)
        {
            fireRateTimer = fireRateValue;
            FireAtTarget(player);
        }
    }

    void FireAtTarget(GameObject target)
    {
        GameObject bulletGO = Instantiate(bulletPrefab, transform.position, transform.rotation);
        targetNode = target.transform.position;
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.target = targetNode;
            bullet.enemyBullet = true;
        }
    }

    void DetectPlayer()
    {
        // Keep pursuing the last known position of the player
        thisAgent.SetDestination(player.transform.position);

        // Check if the player is still within the detection range
        if (distance > detectionRange)
        {
            state = 0; // Change the state to patrolling
        }
    }
}
