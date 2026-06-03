using System.Drawing;
using UnityEngine;

public static class OverlapUtil
{
    public static bool Overlaps(RectTransform a, RectTransform b)
    {
        Rect A = new(a.localPosition.x - a.rect.width / 2,
                        a.localPosition.y - a.rect.height / 2,
                        a.rect.width, a.rect.height);

        Rect B = new(b.localPosition.x - b.rect.width / 2,
                         b.localPosition.y - b.rect.height / 2,
                         b.rect.width, b.rect.height);

        return A.Overlaps(B);
    }

    public static bool Overlaps(BoxCollider a, BoxCollider b)
    {
        return a.bounds.Intersects(b.bounds);
    }
}