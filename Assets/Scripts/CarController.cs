using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [SerializeField] NavMeshAgent mySelf;
    [SerializeField] SpriteRenderer selectionRing;
    [SerializeField] Collider col;
    public Transform target;
    public Transform playerSpot;
    public float moveSpeed, spdMultiplier;
    [SerializeField] ParticleSystem coinFX;

    public bool hostingPlayer, hostingAI, touched, touchedByAI;
    Collider jumpRange;
    float d;

    public void ToggleSelectionRing(bool state)
    {
        selectionRing.gameObject.SetActive(state);
    }

    private void Start()
    {
        mySelf.speed = (Random.Range(0.75f * moveSpeed, 1.25f * moveSpeed)) * spdMultiplier;
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        d = (transform.position - target.transform.position).magnitude;

        if (d > 1)
            mySelf.SetDestination(target.position);
        else
            gameObject.SetActive(false);

        if (jumpRange)
        {
            if (col.bounds.Intersects(jumpRange.bounds) || jumpRange.bounds.Contains(transform.position))
            {
                if (!hostingPlayer)
                    ToggleSelectionRing(true);
                else
                    ToggleSelectionRing(false);
            }
            else
                ToggleSelectionRing(false);
        }
        else
            ToggleSelectionRing(false);
    }

    public void CoinFX()
    {
        coinFX.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "JumpRange")
            jumpRange = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "JumpRange")
            jumpRange = null;
    }
}
