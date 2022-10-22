using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    float yOffset;

    private void Start()
    {
        yOffset = transform.position.y - player.transform.position.y;
    }
    void Update()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y + yOffset + transform.position.z);
    }
}
