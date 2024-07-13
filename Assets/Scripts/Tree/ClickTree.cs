using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickTree : MonoBehaviour, IMouseInteraction
{
    Character character;

    private void Start()
    {
        character = Character.Instance;
    }

    public void InteractionFuc()
    {
        StartCoroutine(character.MoveToTree(transform.position));
    }
}
