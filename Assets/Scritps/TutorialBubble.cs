using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TutorialBubble : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TransformFollower follower;
    [SerializeField]
    private TextMeshPro textMeshPro;
    [SerializeField]
    private RectTransform textRectTransform;

    [SerializeField]
    private string testingText;

    public void SetText(string text)
    {
        textRectTransform.sizeDelta = new Vector2(30, textRectTransform.sizeDelta.y);

        TMP_TextInfo info = textMeshPro.GetTextInfo(text);

        float bottomRightX = info.characterInfo[0].bottomRight.x;

        for (int i = 1; i < text.Length; i++)
        {
            TMP_CharacterInfo characterInfo = info.characterInfo[i];

            if (char.IsSymbol(characterInfo.character))
                continue;
            if (characterInfo.bottomRight.x >= bottomRightX)
                bottomRightX = characterInfo.bottomRight.x;
        }
        float width = bottomRightX - info.characterInfo[0].bottomLeft.x + 0.05f;

        textRectTransform.sizeDelta = new Vector2(width, textRectTransform.sizeDelta.y);
        textMeshPro.text = text;

        spriteRenderer.size = new Vector2(width + 0.6f, spriteRenderer.size.y);
    }

    public void Set(Transform followTarget, string text)
    {
        follower.SetTarget(followTarget);
        SetText(text);
    }

    void OnValidate()
    {
        SetText(testingText);
    }
}
