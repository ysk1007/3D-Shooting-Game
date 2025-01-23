using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class ExpEvent : UnityEngine.Events.UnityEvent<float> { }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject gameResumeButton;
    [SerializeField] private float gameTime;                       // �÷��� Ÿ��
    [SerializeField] private float waveTime = 1 * 60f;             // ���̺� Ÿ��
    [SerializeField] private float maxGameTime = 30 * 60f;         // �ִ� ���� �ð�
    [SerializeField] int gameLevel;
    [SerializeField] private GameObject poolSet;
    [SerializeField] private GameObject Erase;

    [SerializeField] private float exp;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI expText;

    [SerializeField] private GameObject merchant;                   // ����
    
    PhotonView photonView;

    // �ܺο��� �̺�Ʈ �Լ� ����� �� �� �ֵ��� public ����
    [HideInInspector] public ExpEvent onExpEvent = new ExpEvent();

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
        photonView = GetComponent<PhotonView>();
        onExpEvent.AddListener(UpdateExpSlider);
        enemyMemoryPool.DifficultyUpdate(UpdateDifficulty());
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
            enemyMemoryPool.DifficultyUpdate(UpdateDifficulty());
            gamePause = true;
            Erase.SetActive(true);
            merchant.SetActive(true);
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

    public void CallGameResume(bool isPause)
    {
        photonView.RPC("GameResume", RpcTarget.AllBuffered, isPause);
    }

    [PunRPC]
    public void GameResume(bool isPause)
    {
        gamePause = isPause;
        Erase.SetActive(isPause);
        merchant.SetActive(isPause);
    }


    private void UpdateExpSlider(float value)
    {
        expSlider.value = (exp / expSlider.maxValue) * 100;
        expText.text = String.Format("{0}%", expSlider.value);
    }

    public void UpdateExp(float value)
    {
        exp += value;
        onExpEvent.Invoke(exp);
    }

    public List<float> UpdateDifficulty()
    {
        List<float> difficulty = new List<float>();

        // �ִ� ��ȯ��
        if (gameLevel < 2) difficulty.Add(15);
        else if (gameLevel < 4) difficulty.Add(20);
        else if (gameLevel < 6) difficulty.Add(20);
        else if (gameLevel < 8) difficulty.Add(25);
        else difficulty.Add(30);

        // �� ���� ��ȯ�Ǵ� ��
        if (gameLevel < 2) difficulty.Add(1);
        else if (gameLevel < 4) difficulty.Add(1);
        else if (gameLevel < 6) difficulty.Add(1);
        else if (gameLevel < 8) difficulty.Add(2);
        else difficulty.Add(3);

        // ��ȯ �ӵ�
        if (gameLevel < 2) difficulty.Add(1f);
        else if (gameLevel < 4) difficulty.Add(1f);
        else if (gameLevel < 6) difficulty.Add(0.8f);
        else if (gameLevel < 8) difficulty.Add(0.7f);
        else difficulty.Add(0.5f);

        // ��ȯ ������ �� Ÿ�� ����
        if (gameLevel < 2) difficulty.Add(1);
        else if (gameLevel < 4) difficulty.Add(2);
        else if (gameLevel < 6) difficulty.Add(3);
        else if (gameLevel < 8) difficulty.Add(4);
        else difficulty.Add(5);

        return difficulty;
    }
}
