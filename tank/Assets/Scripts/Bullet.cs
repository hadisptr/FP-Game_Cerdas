using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public event Action OnHitEnemy;
    //Target to be specified by script that fires the bullet
    public Vector3 target;

    //tells the bullet who it belongs to (enemy owned or not)
    public bool enemyBullet;

    //speed bullet travels
    [SerializeField]
    private float speed;

    //direction to be created
    private Vector3 direction;

    void Start()
    {
        // Set the velocity to move the bullet in its own forward direction
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
        GetComponent<Rigidbody>().velocity = direction * speed;
    }

    void Update()
    {
        /*
        // bullet travels in its forward direction each frame
        float distThisFrame = speed * Time.deltaTime;
        transform.Translate(transform.forward * distThisFrame, Space.World);
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        //firstly checks to see if the bullet belongs to an enemy
        if (enemyBullet)
        {
            //if bullet is an enemy's we need to set certain tags with actions
            if (other.tag == "Enemy")
            {
                //this would be the case where the bullet initially spawns touching the enemy
                //so we don't want it to just kill itself, so we ignore the enemy tag
                return;
            }
            else if (other.tag == "Player")
            {
                //the enemy fires at the player so here we would deduct player health etc
                //at the moment all this does is delete the bullet
                Destroy(gameObject);
            }
            else
            {
                //this is to destroy the bullet if it hits anything else: walls and obstacles
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                //this would be the case where the bullet initially spawns touching the player
                //so we don't want it to just kill ourself, so we ignore the player tag
                return;
            }
            else if (other.tag == "Enemy")
            {
                //the player fires at the enemies so here we would deduct enemy health etc
                //at the moment all this does is delete the bullet and delete the enemy it hit
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
            else
            {
                //this is to destroy the bullet if it hits anything else: walls and obstacles
                Destroy(gameObject);
            }
        }
    }
}
