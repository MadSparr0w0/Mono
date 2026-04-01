using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardPath : MonoBehaviour
{
    [SerializeField] private List<BoardSpace> spaces = new List<BoardSpace>();

    public IReadOnlyList<BoardSpace> Spaces => spaces;
    public int Count => spaces != null ? spaces.Count : 0;

    private void Awake()
    {
        RebuildIfEmpty();
    }

    [ContextMenu("Rebuild From Children")]
    public void RebuildFromChildren()
    {
        spaces = GetComponentsInChildren<BoardSpace>(true)
            .OrderBy(s => s.Index)
            .ToList();
    }

    private void RebuildIfEmpty()
    {
        if (spaces == null || spaces.Count == 0)
            RebuildFromChildren();
    }

    public BoardSpace GetSpace(int index)
    {
        if (spaces == null || spaces.Count == 0)
            return null;

        index = WrapIndex(index);
        return spaces[index];
    }

    public Vector3 GetWorldPoint(int index)
    {
        BoardSpace space = GetSpace(index);
        return space != null ? space.GetWorldPoint() : transform.position;
    }

    public Vector3 GetPosition(int index)
    {
        return GetWorldPoint(index);
    }

    public int WrapIndex(int index)
    {
        if (spaces == null || spaces.Count == 0)
            return 0;

        int count = spaces.Count;
        while (index < 0) index += count;
        while (index >= count) index -= count;
        return index;
    }

    public int FindFirstIndexByType(SpaceType targetType)
    {
        if (spaces == null) return -1;

        for (int i = 0; i < spaces.Count; i++)
        {
            if (spaces[i] != null && spaces[i].SpaceType == targetType)
                return i;
        }

        return -1;
    }
}
