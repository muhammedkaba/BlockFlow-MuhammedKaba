using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MKDev
{
    public class GameManager : Singleton<GameManager>
    {
        public List<Block> AllBlocks = new List<Block>();

        public Action<Block> BlockDestroyed;

        public float GameTime;
        private float gameTimer;

        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private Transform[] winPanelButtons;
        [SerializeField] private CanvasGroup losePanel;
        [SerializeField] private Transform[] losePanelButtons;

        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerfillImage;
        [SerializeField] private TextMeshProUGUI levelText;

        public bool GameEnded = false;

        private void Awake()
        {
            BlockDestroyed = new Action<Block>(b => { });
            BlockDestroyed += BlockDestroyed_Action;
        }

        private void Start()
        {
            levelText.text = PlayerPrefs.GetInt("Level", 1).ToString();
        }

        private void Update()
        {
            if (!GameEnded)
            {
                gameTimer += Time.deltaTime;

                float remainingTime = GameTime - gameTimer;
                timerText.text = string.Format("{0}:{1:00}", (int)remainingTime / 60, (int)remainingTime % 60);
                timerfillImage.fillAmount = (remainingTime / GameTime);
                if (gameTimer > GameTime)
                {
                    SetGameEnd(false);
                }

            }
        }

        public void SetGameEnd(bool isWin)
        {
            GameEnded = true;
            if (isWin)
            {
                winPanel.gameObject.SetActive(true);
                winPanel.DOFade(1f, 0.7f);
                _ = OpenPanelButtons(winPanelButtons);
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
                _ = SoundManager.i.PlaySound(SFXType.GameWon);
            }
            else
            {
                losePanel.gameObject.SetActive(true);
                losePanel.DOFade(1f, 0.7f);
                _ = OpenPanelButtons(losePanelButtons);
                _ = SoundManager.i.PlaySound(SFXType.GameLost);
            }
        }

        public async UniTaskVoid OpenPanelButtons(Transform[] buttons)
        {
            await UniTask.Delay(700);

            foreach (var item in buttons)
            {
                await item.DOScale(1f, 0.4f).SetEase(Ease.OutBack).ToUniTask();
            }
        }

        public void BlockDestroyed_Action(Block b)
        {
            AllBlocks.Remove(b);
            foreach (var item in AllBlocks.Where(x => x.IsLocked))
            {
                item.ReduceLock();
            }
            if (AllBlocks.Count == 0)
            {
                SetGameEnd(true);
            }
            b.gameObject.SetActive(false);
        }

        public void NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
