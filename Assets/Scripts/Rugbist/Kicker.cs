using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicker : MonoBehaviour {
    public Transform ballHandle;

    public RugbistData Data { get; private set; }
    public bool IsInit { get; private set; } = false;

    private Renderer[] rends;

    public void Init(Color color, RugbistData data)
    {
        rends = GetComponentsInChildren<Renderer>();
        foreach (var rend in rends) rend.material.color = color;
        Data = data;

        IsInit = true;
    }
}
