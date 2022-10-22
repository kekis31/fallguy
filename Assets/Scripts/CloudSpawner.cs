using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    Vector3 centerPos;

    [SerializeField]
    GameObject cloudParticlePrefab;
    [SerializeField]
    GameObject[] cloudCutPrefabs;

    [SerializeField, Range(4, 500)]
    int cloudPathCount;
    [SerializeField, Range(15, 100)]
    int maxDistance;

    public int holeCount;

    List<Vector3> holes = new List<Vector3>();
    void Start()
    {
        centerPos = transform.position;

        GenerateCloud();
    }

    public void GenerateCloud()
    {
        GameObject[] clouds = GameObject.FindGameObjectsWithTag("Cloud");
        if (clouds.Length > 0)
        {
            foreach (GameObject cloud in clouds)
            {
                Destroy(cloud);
            }
        }

        float turnIncrement = 360f / (cloudPathCount * 2);

        for (int i = 0; i < cloudPathCount; i++)
        {
            transform.position = centerPos;
            transform.Rotate(new Vector3(0, turnIncrement, 0));

            int distance = Random.Range(15, maxDistance);
            for (int d = 0; d < distance; d++)
            {
                GameObject spawnedCloud = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                spawnedCloud.transform.localScale = Vector3.one * Mathf.Clamp(((float)d / distance), 0.33f, 1) * 3 * Random.Range(1, 1.25f);
                spawnedCloud.transform.parent = GameObject.Find("Clouds").transform;
                transform.Translate(transform.forward);
            }
        }

        transform.position = centerPos;

        StartCoroutine(CutHolesInCloud());
    }
    IEnumerator CutHolesInCloud()
    {
        float maxRange = 12;

        holes.Clear();

        for (int i = 0; i < holeCount; i++)
        {
            Vector3 randomPos = Vector3.zero;
            while (true)
            {
                randomPos = new Vector3(transform.position.x + Random.Range(-maxRange, maxRange),
                    transform.position.y,
                    transform.position.z + Random.Range(-maxRange, maxRange));

                if (holes.Count == 0)
                {
                    break;
                }

                bool goodPos = true;

                foreach (Vector3 h in holes)
                {
                    if (Vector3.Distance(h, randomPos) < 4)
                    {
                        goodPos = false;
                    }
                }

                if (goodPos)
                {
                    break;
                }
            }


            GameObject cutter = Instantiate(cloudCutPrefabs[Random.Range(0, cloudCutPrefabs.Length)], randomPos, Quaternion.identity);
            cutter.transform.Rotate(Vector3.up, Random.Range(0, 359));
            cutter.transform.localScale = Vector3.one * 6;
            
            // Decide type
            if (GameManager.instance.GetLevel() >= 10 && i != 0 && Random.value < 0.33f)
            {
                cutter.GetComponent<Cutter>().type = Cutter.CutterType.Fake;
            }
            cutter.AddComponent<Rigidbody>();
            holes.Add(cutter.transform.position);

            yield return new WaitForSeconds(0.1f);
            Destroy(cutter);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
