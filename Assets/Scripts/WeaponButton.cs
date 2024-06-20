using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButton : MonoBehaviour
{
    [SerializeField] Button button;

    Character character;

    private void Start()
    {
        character = Character.Instance;
    }

    public void SelectWeapon(int num)
    {
        for (int i = 0; i < character.weaponParent.childCount; i++)
            character.weaponParent.GetChild(i).gameObject.SetActive(false);

        character.weaponParent.GetChild(num).gameObject.SetActive(true);
    }
}
