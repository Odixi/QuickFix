using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSpawnController : MonoBehaviour
{
    public float partSize = 1;
    public GameObject[] Parts;
    public GameObject[] Guns;
    public GameObject[] Thrusters;
    public int NumberOfParts = 30;
    public int MinThrusters = 3;
    public int MinGuns = 3;
    public float MaxThrowForce = 10f;
    private int spawnedParts = 0;
    private float TimeBetweenSpawns = 0.1f;

    private void Start()
    {
        StartCoroutine(SpawnParts());
    }

    IEnumerator SpawnParts()
    {
        for (int i = 0; i < MinThrusters; i++)
        {
            GameObject original = Thrusters[(int)(Random.value * Thrusters.Length)];
            SpawnObject(original);
            spawnedParts++;
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
        for (int i = 0; i < MinGuns; i++)
        {
            GameObject original = Guns[(int)(Random.value * Guns.Length)];
            SpawnObject(original);
            spawnedParts++;
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
        while (spawnedParts < NumberOfParts)
        {
            GameObject original = Parts[(int)(Random.value * Parts.Length)];
            SpawnObject(original);
            spawnedParts++;
            yield return new WaitForSeconds(TimeBetweenSpawns);
        }
    }

    void SpawnObject(GameObject spawnable)
    {
        Transform spawn = transform.GetChild((int)(Random.value * transform.childCount));
        GameObject spawnedObject = Instantiate(spawnable, spawn.position, spawn.rotation);
        float x = Random.Range(-MaxThrowForce, MaxThrowForce);
        float y = Random.Range(-MaxThrowForce, MaxThrowForce);
        float z = 0;
        Vector3 force = new Vector3(x, y, z);
        spawnedObject.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }
}
