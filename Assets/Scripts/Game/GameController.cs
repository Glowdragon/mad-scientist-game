using System;
using System.Collections;
using System.Collections.Generic;
using Development.Debugging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameJam
{
    public class GameController : MonoBehaviour
    {
        [Inject]
        private DebugScreen debugScreen;

        [Inject]
        private MusicPlayer musicPlayer;

        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        private AudioClip gameOverSound;

        [SerializeField]
        private TextMeshProUGUI timeLabel;
        
        [SerializeField]
        private TextMeshProUGUI bigTimeLabel;

        private float time = 0;
        private bool gameOver = false;

        public GameObject PlayerPrefab => this.playerPrefab;

        public float Time => this.time;

        public List<Scientist> Scientists { get; } = new List<Scientist>();
        
        public List<Zombie> Zombies { get; } = new List<Zombie>();

        public List<Zombie> GetAttackersOfScientist(Scientist scientist)
        {
            List<Zombie> attackers = new List<Zombie>();
            foreach (Zombie zombie in this.Zombies)
            {
                if (zombie.IsAlive && zombie.Target == scientist)
                {
                    attackers.Add(zombie);
                }
            }
            return attackers;
        }
        
        public List<Scientist> GetAttackersOfZombie(Zombie zombie)
        {
            List<Scientist> attackers = new List<Scientist>();
            foreach (Scientist scientist in this.Scientists)
            {
                if (scientist.IsAlive && scientist.AttackTarget == zombie)
                {
                    attackers.Add(scientist);
                }
            }
            return attackers;
        }

        private void Update()
        {
            if (this.gameOver)
            {
                return;
            }

            time += UnityEngine.Time.deltaTime;

            this.timeLabel.text = (Mathf.RoundToInt(this.time) / 60).ToString().PadLeft(2, '0') + ":" + (Mathf.RoundToInt(this.time) % 60).ToString().PadLeft(2, '0');

            if (time > 3 && this.Scientists.Count == 0 && GameObject.FindObjectOfType<Scientist>() == null)
            {
                this.gameOver = true;
                this.StartCoroutine(this.GameOver_Coroutine());
            }
        }

        private IEnumerator GameOver_Coroutine()
        {
            this.musicPlayer.Stop();
            this.GetComponent<AudioSource>().PlayOneShot(this.gameOverSound);
            this.bigTimeLabel.text = this.timeLabel.text;
            this.bigTimeLabel.gameObject.SetActive(true);
            this.timeLabel.gameObject.SetActive(false);
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(0);
        }
    }
}