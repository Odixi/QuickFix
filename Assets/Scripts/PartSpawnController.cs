using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSpawnController : MonoBehaviour
{
    public float partSize = 1;
    public GameObject[] parts;
    public int numberOfParts = 30;

    private void Start()
    {
        for(int i = 0; i < numberOfParts; i++)
        {
            var point = GetSpawnPoint();
            var obj = Instantiate(parts[(int)(Random.value * parts.Length)]);
            obj.transform.position = point;
        }
    }

    public Vector2 GetSpawnPoint()
    {
        while (true)
        {
            var viewportPoint = new Vector2(Random.value, Random.value);
            var ray = Camera.main.ViewportPointToRay(viewportPoint);
            var raycastRes = Physics2D.BoxCast(ray.origin, Vector2.one * partSize, 0, ray.direction);
            if (raycastRes.collider == null)
            {
                return Camera.main.ViewportToWorldPoint(viewportPoint);
            }
        }
    }

}
