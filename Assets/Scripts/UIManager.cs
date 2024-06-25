using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private LayoutGroup Score_P1;
    [SerializeField] private LayoutGroup Score_P2;

    private void OnEnable()
    {
        GameManager.OnScoreUpdate += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnScoreUpdate -= UpdateScore;
    }

    private void UpdateScore(bool P1_Scored, int new_score)
    {
        LayoutGroup score_to_update = P1_Scored ? Score_P1 : Score_P2;

        if (!score_to_update.transform.GetChild(new_score - 1).TryGetComponent<UI_Point>(out UI_Point ui_point)) return;

        Debug.Log(new_score);

        ui_point.Fill.fillAmount = 1;
    }
}
