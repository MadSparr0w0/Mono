using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPawn : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BoardPath boardPath;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("State")]
    [SerializeField] private int startMoney = 1500;

    private readonly NetworkVariable<int> boardIndex = new NetworkVariable<int>(0);
    private readonly NetworkVariable<int> money = new NetworkVariable<int>(1500);

    public int BoardIndex => boardIndex.Value;
    public int Money => money.Value;

    private void Start()
    {
        if (IsServer)
        {
            money.Value = startMoney;
        }
    }

    public void SetBoardPath(BoardPath path)
    {
        boardPath = path;
    }

    public int GetBoardIndex()
    {
        return boardIndex.Value;
    }

    public void SetBoardIndex(int index)
    {
        if (!IsServer) return;
        boardIndex.Value = index;
    }

    public void AddMoney(int amount)
    {
        if (!IsServer) return;
        money.Value += amount;
    }

    public void TeleportTo(Vector3 worldPosition)
    {
        if (!IsServer) return;
        transform.position = worldPosition;
    }

    public void SnapToBoardIndex()
    {
        if (boardPath == null) return;
        transform.position = boardPath.GetPosition(boardIndex.Value);
    }

    public void MoveToBoardIndexImmediate(int index)
    {
        if (!IsServer) return;
        boardIndex.Value = index;
        if (boardPath != null)
            transform.position = boardPath.GetPosition(index);
    }

    public void MoveStepsAnimated(int steps)
    {
        if (!IsServer) return;
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(steps));
    }

    private IEnumerator MoveRoutine(int steps)
    {
        if (boardPath == null || boardPath.Count == 0)
            yield break;

        for (int i = 0; i < steps; i++)
        {
            int nextIndex = boardPath.WrapIndex(boardIndex.Value + 1);
            Vector3 target = boardPath.GetPosition(nextIndex);

            while (Vector3.Distance(transform.position, target) > 0.02f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = target;
            boardIndex.Value = nextIndex;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
