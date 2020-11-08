using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sc_playController : MonoBehaviour
{
    public sc_gameController gameC;
    public sc_player player;

    public GameObject BordL, BordR, BordT, BordB, projectiles;
    public GameObject[] Enemies;

    int enemiesLeft = 0;
    public int lives = 3;

    List<cl_campaign.cl_level.Enemy> curLVL;
    float xMax, yMax;
    void Start()
    {
        yMax = Camera.main.orthographicSize;
        xMax = yMax * Camera.main.aspect;
        if (xMax > 5.15f) xMax = 5.15f;
        BordL.transform.position = new Vector3(-xMax, 0);
        BordR.transform.position = new Vector3(xMax, 0);
        BordT.transform.position = new Vector3(0, yMax);
        BordB.transform.position = new Vector3(0, -yMax+0.15f);
    }
    
    public void playLevel(cl_campaign.cl_level level)
    {
        curLVL = level.level;
        enemiesLeft = curLVL.Count;
        StartCoroutine(spawnMeteor(curLVL[0].spawntime, curLVL[0].type, curLVL[0].speed, 0));
    }

    IEnumerator spawnMeteor(float delay, int type, float spd, int count)
    {
        yield return new WaitForSecondsRealtime(delay);
        GameObject tmp = Instantiate(Enemies[type], new Vector3(Random.Range(-xMax + 0.5f, xMax - 0.5f), yMax + 1f), Enemies[type].transform.rotation, projectiles.transform);
        tmp.GetComponent<sc_projectile>().speed = spd;
        if (count + 1 < curLVL.Count)
        {
            StartCoroutine(spawnMeteor(curLVL[count + 1].spawntime - curLVL[count].spawntime, curLVL[count + 1].type, curLVL[count + 1].speed, count + 1));
        }

    }

    public void playerGotHit()
    {
        lives--;
        gameC.newHealth(lives);
    }

    public void enemyDied()
    {
        enemiesLeft--;
        int prg = System.Convert.ToInt32((100f/curLVL.Count) * (curLVL.Count - enemiesLeft));
        if(lives != 0)gameC.newProgress(prg);
    }

    public void cleanField()
    {
        StopAllCoroutines();
        foreach(Transform child in projectiles.transform)
        {
            Destroy(child.gameObject);
        }
        player.rb.position = new Vector3(0, -5);
        player.rb.velocity = new Vector3(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = Vector3.zero;
        float y = 0, x = 0;
        if (Input.GetKey(KeyCode.W))
        {
            y += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            y -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            x += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.shoot();
        }

        if (x != 0 && y != 0)
        {
            x *= 0.75f;
            y *= 0.75f;
        }
        player.move(new Vector3(x, y));
    }
}
