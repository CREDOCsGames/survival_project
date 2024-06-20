using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectSceneCard : WeaponCard
{
    private void Start()
    {
        itemManager = ItemManager.Instance;
        gameManager = GameManager.Instance;
        character = Character.Instance;

        Setting();
        CardColor();
    }

    protected override void Setting()
    {
        price = 0;
        base.Setting();
    }

    public override void Select()
    {
        base.Select();
        character.gameObject.SetActive(true);
        gameManager.ToNextScene("Game");
    }
}
