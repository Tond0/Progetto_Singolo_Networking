using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Views")]
    [SerializeField, Tooltip("The first view active")] private View view_First;
    [Space(13)]
    [SerializeField] private View view_HostOrJoin;
    [SerializeField] private View view_Lobby;
    [SerializeField] private View view_JoinGame;
    [Space(5)]
    [SerializeField] private View view_Game;
    [SerializeField] private View view_MatchOver;

    private View currentView;
    [Header("Score")]
    [SerializeField] private LayoutGroup Score_P1;
    [SerializeField] private LayoutGroup Score_P2;

    private void OnEnable()
    {
        CustomNetworkManager.OnClientConnected += Handle_ClientConnected;

        GameManager.OnScoreUpdate += UpdateScore;
        GameManager.OnMatchOver += OnMatchOver;
    }

    private void OnDisable()
    {
        CustomNetworkManager.OnClientConnected -= Handle_ClientConnected;

        GameManager.OnScoreUpdate -= UpdateScore;
        GameManager.OnMatchOver -= OnMatchOver;
    }

    private void Start()
    {
        //FIXME: per ora qua
        currentView = view_First;
    }


    //FIXME:Spostare queste cose nella view???
    private void UpdateScore(bool P1_Scored, int new_score)
    {
        LayoutGroup score_to_update = P1_Scored ? Score_P1 : Score_P2;

        if (!score_to_update.transform.GetChild(new_score - 1).TryGetComponent<UI_Point>(out UI_Point ui_point)) return;

        ui_point.Fill.fillAmount = 1;
    }
    private void OnMatchOver(bool is_Winner, int difference_Score)
    {
        //Switch to GameOver view
        SwitchView(view_MatchOver, false);

        //Get text component
        TextMeshProUGUI TMP_winnerText = view_MatchOver.GetComponentInChildren<TextMeshProUGUI>();

        //Get the text to display at the end of the game for each client
        string matchOver_text = GetMatchOverText(is_Winner, difference_Score);

        //Display the text!
        TMP_winnerText.text = matchOver_text;
    }
    private string GetMatchOverText(bool isThisClientWinner, int score_Difference)
    {
        //Are u the winner?
        if(isThisClientWinner)
        {
            //Che figata non avevo idea che uno switch potesse essere scitto in questo modo
            return score_Difference switch
            {
                1 => "Hard-fought Win",
                2 => "Deserved Win",
                3 => "Domination Win",
                _ => string.Empty,
            };
        }
        else
        {
            return score_Difference switch
            {
                1 => "Heartbreaking Defeat",
                2 => "Deserved Defeat",
                3 => "Total Defeat",
                _ => string.Empty,
            };
        }
    }

    #region Lobby
    private void Handle_ClientConnected()
    {
        View_Lobby view = (View_Lobby)view_Lobby;

        if (NetworkManager.singleton.numPlayers <= 1)
            view.UpdateLobby("Player 1", null);
        else
            view.UpdateLobby("Player 1", "Player 2");
    }
    #endregion

    #region View System
    public void ChangeView(View newView) => SwitchView(newView, false);
    public void AddView(View newView) => SwitchView(newView, true);
    private void SwitchView(View newView, bool additive)
    {
        if(!additive)
            currentView.gameObject.SetActive(false);

        currentView = newView;

        currentView.gameObject.SetActive(true);
    }
    #endregion
}
