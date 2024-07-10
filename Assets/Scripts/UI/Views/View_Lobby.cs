using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_Lobby : View
{
    [Header("Components")]
    [SerializeField] private TextMeshPro TMP_P1Name;
    [SerializeField] private TextMeshPro TMP_P2Name;
    [Space(5)]
    [SerializeField] private Button Btn_StartGame;

    private void OnEnable()
    {
        Btn_StartGame.enabled = false;
    }

    public void UpdateLobby(string P1_Name, string P2_Name)
    {
        TMP_P1Name.text = P1_Name;

        if (P2_Name == null)
        {
            TMP_P2Name.text = "Waiting for a player...";
            Btn_StartGame.enabled = false;
            return;
        }
        
        TMP_P2Name.text = P2_Name;
        Btn_StartGame.enabled = true;
    }
}
