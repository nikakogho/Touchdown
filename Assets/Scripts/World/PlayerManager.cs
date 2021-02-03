using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
    public Transform arrow;
    public Image arrowImage;
    public Vector3 arrowOffset = new Vector3(0, 2, 0);

    public static PlayerManager player1, player2;

    public TeamManager Team { get; private set; }
    public Rugbist Rugbist { get; private set; }

    public bool isPlayer1;
    static bool onePlayer;

    bool started = false;

    void Awake()
    {
        if (isPlayer1) player1 = this;
        else player2 = this;
        arrow.gameObject.SetActive(false);
    }

    public static void Init(TeamManager teamA, TeamManager teamB)
    {
        player1.arrowImage.color = GameManager.instance.teamBColor;
        player1.Team = teamB;
        player1.SwitchControl(teamB.Front);
        player1.started = true;

        onePlayer = GameManager.onePlayer;
        if (GameManager.onePlayer) return;

        player2.arrowImage.color = GameManager.instance.teamAColor;
        player2.Team = teamA;
        player2.SwitchControl(teamA.Front);
        player2.started = true;
    }

    void Update()
    {
        if (!started) return;

        if (onePlayer || !isPlayer1)
        {
            for (int i = 1; i <= 5; i++) if (Input.GetKeyDown(i.ToString())) SwitchTo(i);
        }
        else
        {
            int[] arr = new int[] { 6, 7, 8, 9, 0 };
            for (int i = 0; i < 5; i++) if (Input.GetKeyDown(arr[i].ToString())) SwitchTo(i + 1);
        }
    }

    void FixedUpdate()
    {
        if (!started) return;
        if (Rugbist == null) return;

        arrow.position = Rugbist.transform.position + arrowOffset;
    }

    void SwitchTo(int index)
    {
        if (Rugbist != Team[index]) SwitchControl(Team[index]);
    }

    public void SwitchControl(Rugbist target)
    {
        if (Rugbist != null) Rugbist.FreedFromPlayer();
        arrow.gameObject.SetActive(true);
        target.PlayerTakesOver();
        Rugbist = target;
    }
}
