using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemShape
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] bool[] _shape;

    public int Width => _width;
    public int Height => _height;
    public bool[] Shape => _shape;
/*
    public ItemShape(int width, int height)
    {
        _width = width;
        _height = height;
        _shape = new bool[_width * _height];
    }

    public ItemShape(bool[,] shape)
    {
        _width = shape.GetLength(0);
        _height = shape.GetLength(1);
        _shape = new bool[_width * _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _shape[GetIndex(x, y)] = shape[x, y];
            }
        }
    }

    private int GetIndex(int x, int y)
    {
        y = (_height - 1) - y;
        return x + _width * y;
    }*/
}

[CreateAssetMenu(fileName = "new Item", menuName = "GameData/DiabolicItem")]
public class DiabolicItemInfo : ScriptableObject
{
    [SerializeField] int itemNum;
    [SerializeField] Sprite itemSprite;
    [SerializeField] string itemName;

    [Header("Stat")]
    [SerializeField] int maxHp;
    [SerializeField] int damage;
    [SerializeField] int recoverHp;
    [SerializeField] int defence;
    [SerializeField] int attackSpeed;
    [SerializeField] int speed;
    [SerializeField] int critical;
    [SerializeField] int avoid;

    [SerializeField] int maxCount;
    [SerializeField] ItemShape itemShape;  // 아이템 모양
    [SerializeField] string description;

    Dictionary<Status, int> itemStatus = new Dictionary<Status, int>();

    public int ItemNum => itemNum;
    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;
    public int MaxCount => maxCount;
    public ItemShape ItemShape => itemShape;
    public string Description => description;

    public Dictionary<Status, int> Stat()
    {
        if (itemStatus == null)
        {
            itemStatus.Add(Status.MAXHP, maxHp);
            itemStatus.Add(Status.DAMAGE, damage);
            itemStatus.Add(Status.RECOVER, recoverHp);
            itemStatus.Add(Status.DEFENCE, defence);
            itemStatus.Add(Status.ATTACK_SPEED, attackSpeed);
            itemStatus.Add(Status.SPEED, speed);
            itemStatus.Add(Status.CRITICAL, critical);
            itemStatus.Add(Status.AVOID, avoid);
        }

        else
        {
            itemStatus[Status.MAXHP] = maxHp;
            itemStatus[Status.DAMAGE] = damage;
            itemStatus[Status.RECOVER] = recoverHp;
            itemStatus[Status.DEFENCE] = defence;
            itemStatus[Status.ATTACK_SPEED] = attackSpeed;
            itemStatus[Status.SPEED] = speed;
            itemStatus[Status.CRITICAL] = critical;
            itemStatus[Status.AVOID] = avoid;
        }

        return itemStatus;
    }
}
