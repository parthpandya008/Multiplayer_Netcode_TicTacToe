using System;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI resultText;
    [SerializeField]
    private Color winColor;
    [SerializeField]
    private Color loseColor;

    private void Start()
    {
        Hide();
        GameManager.Instance.OnGameWin += OnGameWin;
    }

    private void OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            resultText.text = "You Win!";
            resultText.color = winColor;
        }
        else
        {
            resultText.text = "You lose!";
            resultText.color = loseColor;
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
