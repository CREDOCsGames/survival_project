using UnityEngine;
using UnityEngine.UI;

enum StatText
{
    MaxHp,
    ReHp,
    AbHp,
    Def,
    Avoid,
    PDmg,
    WAtk,
    EAtk,
    SAtk,
    LAtk,
    ASpd,
    Spd,
    Ran,
    Luk,
    Cri,
}

public class DescriptionUI : MonoBehaviour
{
    [SerializeField] Text descriptionText;
    [SerializeField] RectTransform rect;

    Vector3 screenPoint;

    public void SetTextInfo(int num)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, Input.mousePosition, Camera.main, out screenPoint);
        transform.position = screenPoint;

        switch(num)
        {
            case 0:
                descriptionText.text = "캐릭터의 최대 체력과 관련된 능력치.\n1이하로 떨어져도 캐릭터의 최대 체력은 1로 적용됩니다.";
                break;

            case 1:
                descriptionText.text = "캐릭터가 1초마다 자동으로 회복하는 체력 수치와 관련된 능력치.\n0이하인 경우 적용되지 않습니다.";
                break;

            case 2:
                descriptionText.text = "캐릭터가 몬스터를 공격했을 경우 회복하는 체력 수치와 관련된 능력치.\n0이하인 경우 적용되지 않고, 최대 적용치는 1입니다.\n폭발 공격에는 적용되지 않습니다.\n검은 휘두르기에만 적용됩니다.";
                break;

            case 3:
                descriptionText.text = "캐릭터가 몬스터에게 공격받을 경우 입는 피해를 감소시켜 주는 능력치.\n0이하인 경우 피해를 더 입습니다.";
                break;

            case 4:
                descriptionText.text = "캐릭터가 몬스터에게 공격받을 경우 피해를 무시하는 확률과 관련된 능력치.\n0이하인 경우 적용되지 않습니다.";
                break;

            case 5:
                descriptionText.text = "모든 공격력의 합산이 끝난후 곱해지는 공격력 배율값과 관련된 능력치.\n각 캐릭터마다 다른 배율을 가집니다.";
                break;

            case 6:
                descriptionText.text = "검/총류 무기의 공격력와 관련된 능력치.\n무기에 적용되는 공격력 합의 수치가 0이하가 되면 공격이 불가능합니다.";
                break;

            case 7:
                descriptionText.text = "스태프류 무기의 공격력, 검신 뇨파의 검기와 관련된 능력치.\n무기에 적용되는 공격력 합의 수치가 0이하가 되면 공격이 불가능합니다.";
                break;

            case 8:
                descriptionText.text = "검류 무기의 공격력과 관련된 능력치.\n무기에 적용되는 공격력 합의 수치가 0이하가 되면 공격이 불가능합니다.";
                break;

            case 9:
                descriptionText.text = "총/스태프류 무기의 공격력, 검신 뇨파의 검기와 관련된 능력치.\n무기에 적용되는 공격력 합의 수치가 0이하가 되면 공격이 불가능합니다.";
                break;

            case 10:
                descriptionText.text = "공격시의 검류 무기의 휘두르는 속도,\n총과 스태프류 무기의 투사체 발사 속도와 관련된 능력치.\n0이하인 경우 무기의 기본 공격 속도로 적용됩니다.";
                break;

            case 11:
                descriptionText.text = "캐릭터가 이동하는 속도와 관련된 능력치.\n1이하로 떨어지면 캐릭터의 이동속도는 1로 적용됩니다.";
                break;

            case 12:
                descriptionText.text = "공격시 검류 무기의 휘두르기가 시전되는 거리,\n투사체가 발사되고 사라지는 사거리와 관련된 능력치.\n 사거리 최소 적용 수치는 기본 사거리의 절반입니다.";
                break;

            case 13:
                descriptionText.text = "이파리나무에서 특별 아이템의 등장 확률,\n스태프류의 특수 능력 발동,\n특수 패시브 아이템의 발동 조건,\n상점 아이템 등급 확률등과 관련된 능력치.\n0이하인 경우 적용되지 않고, 최대 적용치는 100입니다.";
                break;

            case 14:
                descriptionText.text = "검류 무기의 공격력이 1.5배로 더 강하게 적용되는 확률과 관련된 능력치.\n0이하인 경우 적용되지 않습니다.";
                break;
        }
    }
}
