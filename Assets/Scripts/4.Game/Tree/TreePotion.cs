using UnityEngine;

public class TreePotion : MonoBehaviour
{
    Character character;
    Transform characterPos;

    float speed;

    private void Start()
    {
        character = Character.Instance;
        characterPos = Character.Instance.transform;
    }

    private void Update()
    {
        if(character.currentHp != character.maxHp)
            MovePotion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Character"))
        {
            character = Character.Instance;

            if (character.currentHp != character.maxHp)
            {
                SoundManager.Instance.PlayES("EatSound");

                if (!character.isDead)
                    character.currentHp += 10f;

                if (GameManager.Instance.buffNum != 0)
                {
                    character.buffTime = 5f;
                    character.isBuff = true;
                }

                Destroy(gameObject);
            }

            else if(character.currentHp == character.maxHp)
            {
                SoundManager.Instance.PlayES("EatSound");

                character.shield = Mathf.Clamp(character.shield + 5f, 0f, 10f);

                if (GameManager.Instance.buffNum != 0)
                {
                    character.buffTime = 5f;
                    character.isBuff = true;
                }

                Destroy(gameObject);
            }
        }
    }

    public void MovePotion()
    {
        float distance = Vector3.Distance(characterPos.position, transform.position);

        if (distance <= 1.5f)
            speed = character.speed + 1f;

        else
            speed = 0f;

        transform.position = Vector3.MoveTowards(transform.position, characterPos.position, speed * Time.deltaTime);
    }
}
