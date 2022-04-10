using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Config", menuName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    public Vector3Int startPosition;
    public Vector3Int endPosition;
    public int tilesLimit;
}
