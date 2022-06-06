using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkSpawner : MonoBehaviour
{
    public GameObject fireworkSquare;
    public GameObject boardParent;

    [ContextMenu(nameof(StartSpawning))]
    public void StartSpawning()
    {
        StartCoroutine(SpawnFirework());
    }

    private IEnumerator SpawnFirework()
    {
        float randomTime = Random.Range(0.3f, 0.5f);

        yield return new WaitForSeconds(randomTime);

        int randX = Random.Range(0, 10);
        int randY = Random.Range(-10, 51);
        int randZ = Random.Range(0, 10);

        GameObject temp = Instantiate(fireworkSquare, new Vector3(0,0,0), Quaternion.identity, boardParent.transform);
        temp.transform.localPosition = new Vector3(randX * 60, randY, randZ * 60);
        StartCoroutine(DestroyFirework(temp));

        StartCoroutine(SpawnFirework());
    }

    private IEnumerator DestroyFirework(GameObject firework)
    {
        yield return new WaitForSeconds(10);

        Destroy(firework);
    }
}
