using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] Transform[] carSpawners;
    [SerializeField] GameObject[] carPrefabs;

    public bool spawnCars;
    [SerializeField] float spawnRate;

    public float timeSinceLevelStart;

    private void Start()
    {
        InvokeRepeating("SpawnCar", 2, spawnRate);
    }

    private void Update()
    {
        timeSinceLevelStart += Time.deltaTime;
    }

    public void SpawnCar()
    {
        if (GameManager.instance.levelGenerated)
        {
            Transform spawnPoint = PickSpawnPoint();

            int r = Random.Range(0, carPrefabs.Length);

            GameObject car = Instantiate(carPrefabs[r], spawnPoint.position, spawnPoint.rotation);
            car.GetComponent<CarController>().target = PickCarTarget();

            car.transform.SetParent(GameManager.instance.carHolder);
        }
    }

    Transform PickSpawnPoint()
    {
        return carSpawners[Random.Range(0, carSpawners.Length)];
    }

    Transform PickCarTarget()
    {
        return GameManager.instance.carTargets[Random.Range(0, GameManager.instance.carTargets.Length)];
    }
}
