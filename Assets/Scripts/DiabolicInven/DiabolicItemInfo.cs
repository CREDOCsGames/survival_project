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
    [SerializeField] int maxCount;
    [SerializeField] ItemShape itemShape;  // 아이템 모양
    [SerializeField] string discription;

    public int ItemNum => itemNum;
    public Sprite ItemSprite => itemSprite;
    public string ItemName => itemName;
    public int MaxCount => maxCount;
    public ItemShape ItemShape => itemShape;
    public string Discription => discription;
}
