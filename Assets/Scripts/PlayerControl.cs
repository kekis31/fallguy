using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    float currentFallSpeed;
    public float fallSpeed;
    public float maxVelocity;
    public float velocityLimit;

    [Tooltip("Movement")]
    public float moveSpeed;
    public float fallReset;
    float hori, vert;

    bool dead;
    public bool dashing;
    // Update is called once per frame
    void Update()
    {
        if (dead || !GameManager.instance.gameStarted)
            return;

        float fallMultiplier = 1;
        if (dashing)
            fallMultiplier = 1.33f;

        transform.Translate(Vector3.down * currentFallSpeed * fallMultiplier * Time.deltaTime, Space.World);

        IncreaseFallSpeed();

        // Move
        Move();

        if (transform.position.y < -fallReset)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            GameObject.Find("CloudSpawner").GetComponent<CloudSpawner>().GenerateCloud();

            maxVelocity += 1;
            maxVelocity = Mathf.Clamp(maxVelocity, 0, velocityLimit);

            GameManager.instance.AddLevel();
        }

        GameManager.instance.AddScore(currentFallSpeed * fallMultiplier * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            dashing = !dashing;
            GetComponent<Animator>().SetBool("Fast", dashing);

            if (dashing)
            {
                SoundManager.instance.PlaySound("wind", 0.05f, 1, true, false);
                GetComponent<CapsuleCollider>().direction = 1;
            }
            else
            {
                SoundManager.instance.StopSound("wind");
                GetComponent<CapsuleCollider>().direction = 0;
            }

            GameObject[] trails = GameObject.FindGameObjectsWithTag("Trail");
            foreach (GameObject t in trails)
            {
                if (dashing)
                    t.GetComponent<ParticleSystem>().Play();
                else
                    t.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    void IncreaseFallSpeed()
    {
        currentFallSpeed += fallSpeed * Time.deltaTime;

        currentFallSpeed = Mathf.Clamp(currentFallSpeed, 0, maxVelocity);
    }

    void Move()
    {
        hori = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(vert, 0, -hori);

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        GetComponent<Animator>().SetFloat("XMove", hori);
        GetComponent<Animator>().SetFloat("YMove", vert);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cloud") && transform.position.y >= collision.transform.position.y)
        {
            if (!dead)
                GameManager.instance.GameOver();

            dead = true;
        }
        if (collision.gameObject.CompareTag("Border"))
        {
            Vector3 moveDir = new Vector3(vert, 0, -hori);
            transform.Translate(-moveDir * 1, Space.World);
        }
    }
}
