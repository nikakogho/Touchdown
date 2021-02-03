using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {
    public bool teamA;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) GameManager.instance.Touchdown(teamA);
    }
}
