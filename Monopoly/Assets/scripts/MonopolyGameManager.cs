using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonopolyGameManager : NetworkBehaviour
{
    public static MonopolyGameManager Instance { get; private set; }

    [Header("Board")]
    [SerializeField] private BoardPath boardPath;
    [SerializeField] private List<PlayerPawn> playerPawns = new List<PlayerPawn>();

    [Header("Economy")]
    [SerializeField] private int startSalary = 200;
    [SerializeField] private int taxPenalty = 100;

    [Header("Turn State")]
    [SerializeField] private int currentTurnIndex = 0;

    public int CurrentTurnIndex => currentTurnIndex;
    public BoardPath BoardPath => boardPath;
    public IReadOnlyList<PlayerPawn> PlayerPawns => playerPawns;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        base.OnDestroy();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            SetupPawnsAtStart();
        }
    }

    private void SetupPawnsAtStart()
    {
        if (boardPath == null) return;

        Vector3 startPoint = boardPath.GetWorldPoint(0);

        for (int i = 0; i < playerPawns.Count; i++)
        {
            if (playerPawns[i] == null) continue;

            playerPawns[i].SetBoardPath(boardPath);
            playerPawns[i].SetBoardIndex(0);
            playerPawns[i].TeleportTo(startPoint);
        }
    }

    public void MoveCurrentPlayer(int steps)
    {
        if (!IsServer) return;
        if (playerPawns == null || playerPawns.Count == 0) return;
        if (boardPath == null) return;

        PlayerPawn pawn = playerPawns[currentTurnIndex];
        if (pawn == null) return;

        int oldIndex = pawn.GetBoardIndex();
        int rawTarget = oldIndex + steps;
        int newIndex = boardPath.WrapIndex(rawTarget);

        if (rawTarget >= boardPath.Count)
        {
            pawn.AddMoney(startSalary);
        }

        pawn.SetBoardIndex(newIndex);
        pawn.TeleportTo(boardPath.GetWorldPoint(newIndex));

        ResolveLanding(pawn, boardPath.GetSpace(newIndex));
        AdvanceTurn();
    }

    public void RollAndMove()
    {
        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);
        MoveCurrentPlayer(dice1 + dice2);
    }

    private void ResolveLanding(PlayerPawn pawn, BoardSpace landedSpace)
    {
        if (pawn == null || landedSpace == null || boardPath == null)
            return;

        switch (landedSpace.SpaceType)
        {
            case SpaceType.Tax:
                int penalty = landedSpace.TaxAmount > 0 ? landedSpace.TaxAmount : taxPenalty;
                pawn.AddMoney(-Mathf.Abs(penalty));
                break;

            case SpaceType.GoToJail:
                int jailIndex = boardPath.FindFirstIndexByType(SpaceType.Jail);
                if (jailIndex >= 0)
                {
                    pawn.SetBoardIndex(jailIndex);
                    pawn.TeleportTo(boardPath.GetWorldPoint(jailIndex));
                }
                break;
        }
    }

    private void AdvanceTurn()
    {
        if (playerPawns == null || playerPawns.Count == 0) return;

        currentTurnIndex++;
        if (currentTurnIndex >= playerPawns.Count)
            currentTurnIndex = 0;
    }
}
