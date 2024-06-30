using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkIdentity))]
public class UIManager : NetworkBehaviour
{
    [Header("Views")]
    [SerializeField] private View view_Game;
    [SerializeField] private View view_MatchOver;

    private View currentView;
    [Header("Score")]
    [SerializeField] private LayoutGroup Score_P1;
    [SerializeField] private LayoutGroup Score_P2;

    private void OnEnable()
    {
        GameManager.OnScoreUpdate += UpdateScore;
        GameManager.OnMatchOver += OnMatchOver;
    }

    private void OnDisable()
    {
        GameManager.OnScoreUpdate -= UpdateScore;
        GameManager.OnMatchOver -= OnMatchOver;
    }

    private void Start()
    {
        //FIXME: per ora qua
        currentView = view_Game;
    }

    [ClientRpc]
    private void UpdateScore(bool P1_Scored, int new_score)
    {
        LayoutGroup score_to_update = P1_Scored ? Score_P1 : Score_P2;

        if (!score_to_update.transform.GetChild(new_score - 1).TryGetComponent<UI_Point>(out UI_Point ui_point)) return;

        ui_point.Fill.fillAmount = 1;
    }

    private void OnMatchOver(bool Is_P1)
    {
        SwitchView(view_MatchOver);

        TextMeshProUGUI TMP_winnerText = view_MatchOver.GetComponentInChildren<TextMeshProUGUI>();

        string winner = Is_P1 ? "Player 1" : "Player 2";

        TMP_winnerText.text = winner + " won!";
    }

    //FIXME:Publi?
    private void SwitchView(View newView)
    {
        currentView.gameObject.SetActive(false);

        currentView = newView;

        currentView.gameObject.SetActive(true);
    }
}
