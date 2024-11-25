using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDesk : MonoBehaviour
{
    [SerializeField] DiabolicItemInfo piece;

    GameManager gameManager;
    Item item1;

    private void Start()
    {
        gameManager = GameManager.Instance;

        Dictionary<Item.MaterialType, int> item1Materials = new Dictionary<Item.MaterialType, int>()
        {
            { Item.MaterialType.Wood, 10 }
        };

        item1 = new Item(1, Item.ItemType.PieceItem, item1Materials, piece);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            CreateItem(item1);
    }

    void CreateItem(Item item)
    {
        foreach (var material in item.NeedMaterials)
        {
            if (!gameManager.haveMaterials.ContainsKey(material.Key))
                return;

            else if (gameManager.haveMaterials[material.Key] < material.Value)
                return;
        }

        foreach (var material in item.NeedMaterials)
        {
            gameManager.haveMaterials[material.Key] -= material.Value;
        }

        item.AddItem();
    }
}
