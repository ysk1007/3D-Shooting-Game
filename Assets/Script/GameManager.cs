using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private float gameTime;                       // 플레이 타임
    [SerializeField] private float maxGameTime = 30 * 60f;         // 최대 게임 시간
    [SerializeField] int gameLevel;
    [SerializeField] private GameObject poolSet;
    public bool TestMode = false;

    float sec = 0;
    int min = 0;

    public float GameTime => gameTime;
    public float MaxGameTime => maxGameTime;
    public float GameLevel => gameLevel;

    private void Awake()
    {
        instance = this;
        if (TestMode) return;
            poolSet.SetActive(true);
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        gameLevel = Mathf.FloorToInt(gameTime / 10f);

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        min = (int)gameTime / 60;
        sec = (int)gameTime % 60;

        textTimer.text = string.Format("{0:D2}:{1:D2}", min, (int)sec);
    }
}
