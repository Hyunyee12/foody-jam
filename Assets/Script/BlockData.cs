using UnityEngine;
using System.Collections.Generic;

public class BlockData : MonoBehaviour
{
    public enum BlockType { Material, Tool }
    public enum ToolKind { None, Knife, Pot, Oven } // 나중에 도구가 추가되면 여기에 이름만 더 적으면 돼!

    [Header("Who am I?")]
    public BlockType myType;
    
    [Header("I am tool (material = None)")]
    public ToolKind myToolKind = ToolKind.None;

    [Header("I am material (with which tool?)")]
    public List<ToolKind> compatibleTools; // 합쳐질 수 있는 도구들 선택
    public GameObject mergedPrefab; // 도구랑 합쳐진 후 짜잔! 하고 변신할 새로운 블록 (결과물)
}