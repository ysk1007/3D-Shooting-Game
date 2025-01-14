using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject gameResumeButton;
    [SerializeField] private float gameTime;                       // 플레이 타임
    [SerializeField] private float waveTime = 1 * 60f;             // 웨이브 타임
    [SerializeField] private float maxGameTime = 30 * 60f;         // 최대 게임 시간
    [SerializeField] int gameLevel;
    [SerializeField] private GameObject poolSet;
    [SerializeField] private GameObject Erase;

    [Space]
    public BulletMemoryPool bulletMemoryPool;
    public GunMemoryPool gunMemoryPool;
    public EnemyMemoryPool enemyMemoryPool;
    public CasingMemoryPool casingMemoryPool;
    public ItemMemoryPool itemMemoryPool;
    public DamageTextMemoryPool damageTextMemoryPool;

    public bool gamePause = false;
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
        if (gamePause) return;

        gameTime += Time.deltaTime;

        if (gameTime > waveTime)
        {
            gameTime = 0;
            gameLevel++;
            gamePause = true;
            Erase.SetActive(true);
            if (PhotonNetwork.IsMasterClient) gameResumeButton.SetActive(true);
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        min = (int)gameTime / 60;
        sec = (int)gameTime % 60;

        textTimer.text = string.Format("{0:D2}:{1:D2}", min, (int)sec);
        waveText.text = string.Format("Wave {0}",gameLevel);
    }

    public void GameResume()
    {
        gamePause = false;
        Erase.SetActive(false);
    }
}
