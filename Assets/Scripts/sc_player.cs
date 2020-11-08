using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sc_player : MonoBehaviour
{
    public sc_playController playController;

    public float speed;
    public Rigidbody rb;
    public GameObject projectile;

    public void move(Vector3 dir)
    {
        rb.AddForce(dir * speed, ForceMode.Impulse);
    }

    public void shoot()
    {
        Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + 1), new Quaternion(), playController.projectiles.transform);
    }
}
