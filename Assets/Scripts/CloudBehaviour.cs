using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBehaviour : MonoBehaviour
{
    MeshRenderer mr;

    public bool fake;
    void Start()
    {
        // Randomize position
        transform.position = new Vector3(transform.position.x + Random.Range(-0.2f, 0.2f),
            transform.position.y + Random.Range(-0.2f, 0.2f),
            transform.position.z + Random.Range(-0.2f, 0.2f));

        mr = GetComponent<MeshRenderer>();
    }

    public void HideCloud()
    {
        fake = true;

        mr = GetComponent<MeshRenderer>();

        mr.material = GameManager.instance.materials[1];
    }

    public void ShowCloud()
    {
        fake = false;

        mr.material = GameManager.instance.materials[0];
    }
}
