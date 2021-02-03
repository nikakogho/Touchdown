using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Ball : MonoBehaviour {
    public Rugbist Owner { get; private set; }
    public Rugbist LastOwner { get; private set; }
    public static Ball instance;

    GameManager manager;

    Rigidbody rb;
    //Collider col;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        //col = GetComponent<Collider>();
    }

    void Start()
    {
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
        rb.velocity = dir * GameManager.instance.ballThrowForce;
        manager = GameManager.instance;
    }
    
    public void SetOwner(Rugbist rugbist)
    {
        if (rugbist.Team == PlayerManager.player1.Team) PlayerManager.player1.SwitchControl(rugbist);
        else if (!GameManager.onePlayer) PlayerManager.player2.SwitchControl(rugbist);
        
        LastOwner = Owner = rugbist;
        rb.isKinematic = true;
        transform.parent = rugbist.ballHandle;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Release()
    {
        Throw((transform.forward + Vector3.up * 0.2f) * 0.5f);
    }

    public void Throw(Vector3 dir)
    {
        Owner = null;
        transform.parent = null;
        rb.isKinematic = false;
        rb.velocity = dir;
    }

    void FixedUpdate()
    {
        float x = Mathf.Clamp(transform.position.x, -manager.borderX - 1, manager.borderX + 1);
        float y = Mathf.Clamp(transform.position.y, 0, 10);
        float z = Mathf.Clamp(transform.position.z, -manager.borderZ, manager.borderZ);
        Vector3 vec = new Vector3(x, y, z);
        if (vec != transform.position) manager.Out();
    }
}
