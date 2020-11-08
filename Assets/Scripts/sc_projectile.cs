using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sc_projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public bool enemy, ship;
    public GameObject projectile;
    public sc_playController FieldController;
    // Start is called before the first frame update
    void Start()
    {
        FieldController = GameObject.Find("Field").GetComponent<sc_playController>();
        if (enemy)
        {
            if (!ship) rb.angularVelocity = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 3f), Random.Range(0f, 2f));
            rb.velocity = Vector3.down * speed;
            if (ship) StartCoroutine(shoot());
        }
        else
        {
            rb.velocity = Vector3.up * speed;
        }
    }

    IEnumerator shoot()
    {
        yield return new WaitForSecondsRealtime(1f);
        Instantiate(projectile, new Vector3(transform.position.x, transform.position.y-0.5f,0), projectile.transform.rotation,FieldController.projectiles.transform);
        StartCoroutine(shoot());
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Borders":
                if (enemy && gameObject.tag != "EnemyShot")
                {
                    if (other.name == "BorderB")
                    {
                        FieldController.enemyDied();
                        Destroy(gameObject);
                    }
                } else
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Shot":
                if (gameObject.tag != "EnemyShot")
                {
                    FieldController.enemyDied();
                    Destroy(gameObject);
                }
                break;
            case "Player":
                if (enemy)
                {
                    FieldController.playerGotHit();
                    if (gameObject.tag != "EnemyShot") FieldController.enemyDied();
                    Destroy(gameObject);
                }
                break;
            case "Enemy":
                if (!enemy) Destroy(gameObject);
                break;
        }
    }
}
