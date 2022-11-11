using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RopeBehaviour : MonoBehaviour
{
    public Vector2 target1; // The first civil that the rope is connected to
    public Vector2 target2; // The second civil that the rope is connected to

    RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target1 == null || target2 == null) return;

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Vector3.Distance(target1, target2));

        Vector3 pos1 = target1;
        Vector3 pos2 = target2;

        transform.position = new Vector3(pos1.x + (pos2.x - pos1.x) / 2, pos1.y + (pos2.y - pos1.y) / 2, 0);

        var offset = -90f;
        Vector2 direction = target1 - target2;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }
}
