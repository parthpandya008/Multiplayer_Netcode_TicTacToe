using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI resultText;
    [SerializeField]
    private Color winColor;
    [SerializeField]
    private Color loseColor;
    [SerializeField]
    private Button rematchButton;

    private void Awake()
    {
        rematchButton.onClick.AddListener(()=>
        {
            GameManager.Instance.RematchRpc();
        });
    }

    private void Start()
    {
        Hide();
        GameManager.Instance.OnGameWin += OnGameWin;
        GameManager.Instance.OnRematch += OnRematch;
    }

    private void OnRematch(object sender, EventArgs e)
    {
        Hide();
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
