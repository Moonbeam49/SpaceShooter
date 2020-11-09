using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Модель игрока, представляет список досутпных ему функций
public class sc_player : MonoBehaviour
{
    public sc_playController playController;

    public float speed;
    public Rigidbody rb;
    public GameObject projectile;

    //Двигает игрока в определенном направлении
    public void move(Vector3 dir)
    {
        rb.AddForce(dir * speed, ForceMode.Impulse);
    }

    //Создает объект выстрела
    public void shoot()
    {
        Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + 1), new Quaternion(), playController.projectiles.transform);
    }
}
