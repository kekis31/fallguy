using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBehaviour : MonoBehaviour
{
    void Start()
    {
        // Randomize position
        transform.position = new Vector3(transform.position.x + Random.Range(-0.2f, 0.2f),
            transform.position.y + Random.Range(-0.2f, 0.2f),
            transform.position.z + Random.Range(-0.2f, 0.2f));
    }

    void Update()
    {

    }

    public void HideCloud(float duration)
    {
        GetComponent<MeshRenderer>().material = GameManager.instance.materials[1];

        Invoke(nameof(ShowCloud), duration);
    }

    void ShowCloud()
    {
        GetComponent<MeshRenderer>().material = GameManager.instance.materials[0];
    }
}
