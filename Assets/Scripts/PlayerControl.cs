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

    bool dead;
    public bool dashing;
    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        float fallMultiplier = 1;
        if (dashing)
            fallMultiplier = 1.33f;

        transform.Translate(Vector3.down * currentFallSpeed * fallMultiplier * Time.deltaTime);

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
        }
    }

    void IncreaseFallSpeed()
    {
        currentFallSpeed += fallSpeed * Time.deltaTime;

        currentFallSpeed = Mathf.Clamp(currentFallSpeed, 0, maxVelocity);
    }

    void Move()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(vert, 0, -hori);

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cloud") && transform.position.y >= collision.transform.position.y)
        {
            dead = true;
            GameManager.instance.GameOver();
        }
        if (collision.gameObject.CompareTag("Border"))
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }
}
