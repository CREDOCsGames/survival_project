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
                descriptionText.text = "ĳ������ �ִ� ü�°� ���õ� �ɷ�ġ.\n1���Ϸ� �������� ĳ������ �ִ� ü���� 1�� ����˴ϴ�.";
                break;

            case 1:
                descriptionText.text = "ĳ���Ͱ� 1�ʸ��� �ڵ����� ȸ���ϴ� ü�� ��ġ�� ���õ� �ɷ�ġ.\n0������ ��� ������� �ʽ��ϴ�.";
                break;

            case 2:
                descriptionText.text = "ĳ���Ͱ� ���͸� �������� ��� ȸ���ϴ� ü�� ��ġ�� ���õ� �ɷ�ġ.\n0������ ��� ������� �ʰ�, �ִ� ����ġ�� 1�Դϴ�.\n���� ���ݿ��� ������� �ʽ��ϴ�.\n���� �ֵθ��⿡�� ����˴ϴ�.";
                break;

            case 3:
                descriptionText.text = "ĳ���Ͱ� ���Ϳ��� ���ݹ��� ��� �Դ� ���ظ� ���ҽ��� �ִ� �ɷ�ġ.\n0������ ��� ���ظ� �� �Խ��ϴ�.";
                break;

            case 4:
                descriptionText.text = "ĳ���Ͱ� ���Ϳ��� ���ݹ��� ��� ���ظ� �����ϴ� Ȯ���� ���õ� �ɷ�ġ.\n0������ ��� ������� �ʽ��ϴ�.";
                break;

            case 5:
                descriptionText.text = "��� ���ݷ��� �ջ��� ������ �������� ���ݷ� �������� ���õ� �ɷ�ġ.\n�� ĳ���͸��� �ٸ� ������ �����ϴ�.";
                break;

            case 6:
                descriptionText.text = "��/�ѷ� ������ ���ݷ¿� ���õ� �ɷ�ġ.\n���⿡ ����Ǵ� ���ݷ� ���� ��ġ�� 0���ϰ� �Ǹ� ������ �Ұ����մϴ�.";
                break;

            case 7:
                descriptionText.text = "�������� ������ ���ݷ�, �˽� ������ �˱�� ���õ� �ɷ�ġ.\n���⿡ ����Ǵ� ���ݷ� ���� ��ġ�� 0���ϰ� �Ǹ� ������ �Ұ����մϴ�.";
                break;

            case 8:
                descriptionText.text = "�˷� ������ ���ݷ°� ���õ� �ɷ�ġ.\n���⿡ ����Ǵ� ���ݷ� ���� ��ġ�� 0���ϰ� �Ǹ� ������ �Ұ����մϴ�.";
                break;

            case 9:
                descriptionText.text = "��/�������� ������ ���ݷ�, �˽� ������ �˱�� ���õ� �ɷ�ġ.\n���⿡ ����Ǵ� ���ݷ� ���� ��ġ�� 0���ϰ� �Ǹ� ������ �Ұ����մϴ�.";
                break;

            case 10:
                descriptionText.text = "���ݽ��� �˷� ������ �ֵθ��� �ӵ�,\n�Ѱ� �������� ������ ����ü �߻� �ӵ��� ���õ� �ɷ�ġ.\n0������ ��� ������ �⺻ ���� �ӵ��� ����˴ϴ�.";
                break;

            case 11:
                descriptionText.text = "ĳ���Ͱ� �̵��ϴ� �ӵ��� ���õ� �ɷ�ġ.\n1���Ϸ� �������� ĳ������ �̵��ӵ��� 1�� ����˴ϴ�.";
                break;

            case 12:
                descriptionText.text = "���ݽ� �˷� ������ �ֵθ��Ⱑ �����Ǵ� �Ÿ�,\n����ü�� �߻�ǰ� ������� ��Ÿ��� ���õ� �ɷ�ġ.\n ��Ÿ� �ּ� ���� ��ġ�� �⺻ ��Ÿ��� �����Դϴ�.";
                break;

            case 13:
                descriptionText.text = "���ĸ��������� Ư�� �������� ���� Ȯ��,\n���������� Ư�� �ɷ� �ߵ�,\nƯ�� �нú� �������� �ߵ� ����,\n���� ������ ��� Ȯ����� ���õ� �ɷ�ġ.\n0������ ��� ������� �ʰ�, �ִ� ����ġ�� 100�Դϴ�.";
                break;

            case 14:
                descriptionText.text = "�˷� ������ ���ݷ��� 1.5��� �� ���ϰ� ����Ǵ� Ȯ���� ���õ� �ɷ�ġ.\n0������ ��� ������� �ʽ��ϴ�.";
                break;
        }
    }
}
