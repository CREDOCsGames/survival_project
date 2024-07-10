using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class _2DArray
{
    public int[] row;   // 행
}

[CreateAssetMenu(fileName = "new Item", menuName = "GameData/DiabolicItem")]
public class DiabolicItemInfo : ScriptableObject
{
    [SerializeField] int itemNum;
    [SerializeField] string itemName;
    [SerializeField] _2DArray[] itemShape;  // 열

    public int ItemNum => itemNum;
    public string ItemName => itemName;
    public _2DArray[] ItemShape => itemShape;
}
