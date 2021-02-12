using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] Transform[] carSpawners;
    [SerializeField] Transform[] carTargets;
    [SerializeField] GameObject[] carPrefabs;
    [SerializeField] GameObject spawnerHolder;

    [SerializeField] bool spawnCars;
    [SerializeField] float spawnRate;
    [SerializeField] float spawnerSpeed;

    public float timeSinceLevelStart;

    private void Start()
    {
        if(spawnCars)
        InvokeRepeating("SpawnCar", 0, spawnRate);
    }

    private void Update()
    {
        timeSinceLevelStart += Time.deltaTime;

        spawnerHolder.transform.Translate(transform.forward * spawnerSpeed * Time.deltaTime);
    }

    public void SpawnCar()
    {
        Transform spawnPoint = PickSpawnPoint();
        int r = Random.Range(0, carPrefabs.Length);
        GameObject car = Instantiate(carPrefabs[r], spawnPoint.position, spawnPoint.rotation);
        car.GetComponent<CarController>().target = PickCarTarget();
    }

    Transform PickSpawnPoint()
    {
        return carSpawners[Random.Range(0, carSpawners.Length)];
    }

    Transform PickCarTarget()
    {
        return carTargets[Random.Range(0, carTargets.Length)];
    }
}
