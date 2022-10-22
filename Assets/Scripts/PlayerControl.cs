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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * currentFallSpeed * Time.deltaTime);

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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            GameManager.instance.GameOver();
        }
        if (collision.gameObject.name == "Pipe")
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }
}
