using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRugbistController : RugbistController {
    protected override bool ActiveByDefault => false;

    float moveX;
    float moveY;

    bool isTeamA;

    Rigidbody rb;

    bool onePlayer;

    #region Input

    string hor;
    string ver;
    KeyCode kickButton;
    KeyCode throwButton;
    KeyCode passButton;
    KeyCode dropButton;

    #region Player 1

    string hor1 = "Horizontal2";
    string ver1 = "Vertical2";
    KeyCode throw1 = KeyCode.T;
    KeyCode pass1 = KeyCode.Space;
    KeyCode drop1 = KeyCode.E;

    #endregion

    #region Player 2

    string hor2 = "Horizontal";
    string ver2 = "Vertical";
    KeyCode throw2 = KeyCode.Delete;
    KeyCode pass2 = KeyCode.KeypadEnter;
    KeyCode drop2 = KeyCode.Backslash;

    #endregion

    #endregion

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        isTeamA = rugbist.Team == manager.TeamA;
        hor = isTeamA ? hor1 : hor2;
        ver = isTeamA ? ver1 : ver2;
        kickButton = KeyCode.F;
        throwButton = isTeamA ? throw1 : throw2;
        passButton = isTeamA ? pass1 : pass2;
        dropButton = isTeamA ? drop1 : drop2;
        onePlayer = GameManager.onePlayer;
    }

    void Update()
    {
        if (PauseManager.Paused) return;
        
        if (onePlayer)
        {
            moveX += Input.GetAxis(hor1) + Input.GetAxis(hor);
            moveY += Input.GetAxis(ver1) + Input.GetAxis(ver);
        }
        else
        {
            moveX -= Input.GetAxis(ver);
            moveY += Input.GetAxis(hor);
        }

        if (Input.GetKeyDown(kickButton)) Kick();
        else if (Input.GetKeyDown(throwButton)) Throw(transform.forward);
        else if (Input.GetKeyDown(passButton)) Pass(new Vector3(moveX, 0, moveY));
        else if (Input.GetKeyDown(dropButton)) Drop();
        else if (onePlayer)
        {
            if(Input.GetKeyDown(throw1)) Throw(transform.forward);
            else if (Input.GetKeyDown(pass1)) Pass(new Vector3(moveX, 0, moveY));
            else if (Input.GetKeyDown(drop1)) Drop();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (moveX != 0 || moveY != 0)
        {
            Vector3 dir = new Vector3(moveX, 0, moveY);
            Vector3 move = dir.normalized * rugbist.Data.speed * Time.fixedDeltaTime;
            moveX = moveY = 0;
            Vector3 destination = rb.position + move;
            destination.x = Mathf.Clamp(destination.x, minX, maxX);
            destination.z = Mathf.Clamp(destination.z, minZ, maxZ);
            transform.LookAt(destination);
            rb.MovePosition(destination);
        }
    }
}
