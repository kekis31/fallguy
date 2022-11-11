using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public enum ControlScheme { Normal }
    public ControlScheme controlScheme;

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

    // Touch screen stuff
    bool touchHoldingScreen;
    Vector2 touchPosition;
    RopeBehaviour virtualJoystick;

    private void Start()
    {
        virtualJoystick = GameObject.Find("VirtualJoystick").GetComponent<RopeBehaviour>();
    }

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

            StartCoroutine(GameObject.Find("CloudSpawner").GetComponent<CloudSpawner>().GenerateCloud());

            maxVelocity += 1;
            maxVelocity = Mathf.Clamp(maxVelocity, 0, velocityLimit);

            GameManager.instance.AddLevel();
        }

        GameManager.instance.AddScore(currentFallSpeed * fallMultiplier * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && !dashing)
        {
            Dive();
        }
    }

    public void Dive()
    {
        dashing = !dashing;
        GetComponent<Animator>().SetBool("Fast", dashing);

        if (dashing)
        {
            SoundManager.instance.PlaySound("wind", 0.05f, 1, true, false);
        }
        else
        {
            SoundManager.instance.StopSound("wind");
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

    void IncreaseFallSpeed()
    {
        currentFallSpeed += fallSpeed * Time.deltaTime;

        currentFallSpeed = Mathf.Clamp(currentFallSpeed, 0, maxVelocity);
    }

    void Move()
    {
        hori = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");

        // Touch controls
        if (Input.touchCount > 0 && controlScheme == ControlScheme.Normal)
        {
            Vector2 touchAxis = GetTouchAxis();
            hori = touchAxis.x;
            vert = touchAxis.y;

            Vector2 touchPos = Input.GetTouch(0).position;
            virtualJoystick.target1 = touchPos;
            virtualJoystick.target2 = touchPosition;
        }
        else
        {
            touchHoldingScreen = false;
            virtualJoystick.target1 = Vector2.zero;
            virtualJoystick.target2 = Vector2.zero;
        }

        hori = Mathf.Clamp(hori, -1, 1);
        vert = Mathf.Clamp(vert, -1, 1);


        Vector3 moveDir = new Vector3(vert, 0, -hori);

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        GetComponent<Animator>().SetFloat("XMove", hori);
        GetComponent<Animator>().SetFloat("YMove", vert);
    }

    Vector2 GetTouchAxis()
    {
        Vector2 touchAxis = Vector2.zero;

        Touch t = Input.GetTouch(0);

        if (!touchHoldingScreen)
        {
            touchPosition = t.position;
            touchHoldingScreen = true;
        }

        touchAxis = (t.position - touchPosition) / 400;

        touchAxis.x = Mathf.Clamp(touchAxis.x, -1, 1);
        touchAxis.y = Mathf.Clamp(touchAxis.y, -1, 1);

        return touchAxis;
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
