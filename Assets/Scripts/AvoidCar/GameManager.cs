using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AvoidCar.Common
{
    public class GameManager : MonoBehaviour
    {
        public GameEventSO gameOverEvent;

        public static GameManager _instance;

        public GameObject GameOverUI;
        public Button restartButton;

        private void Awake()
        {
            _instance = this;
        }

        private void OnEnable()
        {
            restartButton.onClick.AddListener(RestartGame);
            gameOverEvent.OnEventRaised += OnGameOver;
        }

        private void OnDisable()
        {
            gameOverEvent.OnEventRaised -= OnGameOver;
        }
        public void OnGameOver()
        {
            GameOverUI.SetActive(true);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
