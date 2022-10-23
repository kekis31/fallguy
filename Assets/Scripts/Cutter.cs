using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    public enum CutterType { Normal, Fake }
    public CutterType type;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            if (type == CutterType.Normal)
            {
                Destroy(collision.gameObject);
            }
            else if (type == CutterType.Fake)
            {
                collision.gameObject.GetComponent<CloudBehaviour>().HideCloud(Random.Range(2, 2.5f));
            }
        }
    }
}
