using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType { PieceItem, Food, Weapon }

    public enum MaterialType
    {
        Wood,
        Rock,
        Fish,
        Fruit,
    }
    /// <summary>
    /// XXXXXX
    /// ^^    : 01 = 아이템
    ///   ^^  : 아이템 종류 구분
    /// </summary>
    public ulong ItemId { get; }
    public ItemType Type { get; }
    public Dictionary<MaterialType, int> NeedMaterials { get; }

    public int PieceId { get; }

    public ulong ImageId { get; }

    public Item(ulong itemId, ItemType type, Dictionary<MaterialType, int> needMaterials, DiabolicItemInfo info)
    {
        ItemId = itemId;
        NeedMaterials = needMaterials;
        Type = type;
        PieceId = info.ItemNum;
    }

    public Item(ulong itemId, int type, string needMaterialTypes, string needMaterialCounts, int pieceId)
    {
        ItemId = itemId;
        ImageId = itemId;

        string[] tempTypeString = needMaterialTypes.Split(',');
        string[] tempCountString = needMaterialCounts.Split(",");

        Dictionary<MaterialType, int> tempDic = new Dictionary<MaterialType, int>();

        for (int i = 0; i < tempTypeString.Length; i++)
        {
            MaterialType mType = (MaterialType)int.Parse(tempTypeString[i]);
            int mCount = int.Parse(tempCountString[i]);

            tempDic.Add(mType, mCount);
        }

        NeedMaterials = tempDic;
        Type = (ItemType)type;
        PieceId = pieceId;
    }

    public void AddItem()
    {
        switch (Type)
        {
            case ItemType.PieceItem:
                ItemManager.Instance.AddItem(PieceId);
                break;

        }
    }
}
