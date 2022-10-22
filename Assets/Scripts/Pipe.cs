using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    Transform player;
    MeshRenderer mr;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool nearPlayer = Physics.CheckSphere(player.position, 6.5f, LayerMask.GetMask("Pipe"));

        if (nearPlayer)
        {
            mr.enabled = true;
        }
        else
        {
            mr.enabled = false;
        }
    }
}
