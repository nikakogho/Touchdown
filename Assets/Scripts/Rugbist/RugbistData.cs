using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rugbist", menuName = "Rugbist")]
public class RugbistData : ScriptableObject {
    new public string name;
    public Division division;
    public float speed = 2;
    //may do
    //[Range(0, 1)]
    //public float kickAccuracy = 0.6f;
    public float passAngle = 45;
    public enum Division { Bronze, Silver, Gold, Platinum }
}
