using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    Transform player;
    Animator anim;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool nearPlayer = Physics.CheckSphere(player.position, 6.5f, LayerMask.GetMask("Pipe"));

        anim.SetBool("Active", nearPlayer);
    }
}
