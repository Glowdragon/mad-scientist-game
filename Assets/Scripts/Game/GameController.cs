﻿using System.Collections.Generic;
using Development.Debugging;
using UnityEngine;
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

        private float time = 0;

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
            time += UnityEngine.Time.deltaTime;

            if (time > 3 && this.Scientists.Count == 0)
            {
                this.musicPlayer.Stop();
                this.GetComponent<AudioSource>().PlayOneShot(this.gameOverSound);   
            }
        }
    }
}