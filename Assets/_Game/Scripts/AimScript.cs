using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimScript : MonoBehaviour
{
    [SerializeField] private GameObject player;

    void Update()
    {
        transform.position = player.transform.position;
    }
}
