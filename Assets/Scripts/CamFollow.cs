﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed;

    private void Update()
    {
        if (GameManager.instance.currentGameState == GameState.EndScreen)
            transform.position = Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);
        else
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, target.transform.position.z);

            transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
        }
    }
}
