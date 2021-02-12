using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    [SerializeField] NavMeshAgent mySelf;
    public Transform target;
    public Transform playerSpot;
    public float moveSpeed, spdMultiplier;

    public bool hostingPlayer;
    float d;

    private void Start()
    {
        mySelf.speed = (Random.Range(0.75f * moveSpeed, 1.25f * moveSpeed)) * spdMultiplier;
    }

    private void Update()
    {
        d = (transform.position - target.transform.position).magnitude;

        if (d > 1)
            mySelf.SetDestination(target.position);
        else
            Destroy(gameObject);
    }
}
