using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkIdentity))]
public class UIManager : NetworkBehaviour
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

    [ClientRpc]
    private void UpdateScore(bool P1_Scored, int new_score)
    {
        LayoutGroup score_to_update = P1_Scored ? Score_P1 : Score_P2;

        if (!score_to_update.transform.GetChild(new_score - 1).TryGetComponent<UI_Point>(out UI_Point ui_point)) return;

        ui_point.Fill.fillAmount = 1;
    }
}
