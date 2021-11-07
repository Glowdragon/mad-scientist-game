using System;
using System.Collections;
using System.Collections.Generic;
using GameJam;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Zenject;
using Random = UnityEngine.Random;

public class Scientist : MonoBehaviour
{
    [Inject]
    private GameController gameController;

    [Inject]
    private CameraController cameraController;
    
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Image healthBarFillImage;
    
    [SerializeField]
    private float maxHealth = 10;
    
    [SerializeField]
    private int maxAmmo = 100;

    [SerializeField]
    private float ammoReloadTime;

    [Header("Shooting")]
    [SerializeField]
    private Transform rotatingTransform;
    
    [SerializeField]
    private GameObject bulletPrefab;
    
    [SerializeField]
    private Transform bulletSpawn;

    [SerializeField]
    private LayerMask bulletBlockingLayers;

    [SerializeField]
    private float shootCooldown = 0.2f;

    [SerializeField]
    private AudioClip shootSound;
    
    [SerializeField]
    private AudioClip noAmmoSound;

    private AudioSource audioSource;
    
    private float health = 0;
    private int ammo = 0;

    private float shootTimer = 0;
    private float ammoReloadTimer = 0;
    private float noAmmoSoundTimer = 0;

    public float Health => this.health;
    
    public bool IsAlive => health > 0;
    
    public Scientist ProtectTarget { get; set; }

    public Zombie AttackTarget { get; set; }
    
    public float AimingAngle { get; set; }

    private void Start()
    {
        this.audioSource = this.GetComponent<AudioSource>();
        
        this.health = this.maxHealth;
        this.ammo = this.maxAmmo;
    }

    private void OnEnable()
    {
        this.Invoke(() =>
        {
            this.gameController.Scientists.Add(this);
        }, 0.1f);
    }

    private void OnDisable()
    {
        this.gameController.Scientists.Remove(this);
    }

    private void Update()
    {
        this.shootTimer += Time.deltaTime;
        this.noAmmoSoundTimer += Time.deltaTime;

        if (this.ammo <= 0)
        {
            this.ammoReloadTimer += Time.deltaTime;
            if (this.ammoReloadTimer > this.ammoReloadTime)
            {
                this.ammoReloadTimer = 0;
                this.ammo = this.maxAmmo;
            }
        }
        
        this.healthBarFillImage.fillAmount = this.health / this.maxHealth;

        this.animator.SetFloat("AimAngle", this.AimingAngle - this.rotatingTransform.eulerAngles.y);
    }

    public void Shoot()
    {
        if (this.shootTimer >= this.shootCooldown)
        {
            this.shootTimer = 0;
            
            if (this.ammo <= 0)
            {
                if (this.noAmmoSoundTimer > 3)
                {
                    this.noAmmoSoundTimer = 0;
                    this.audioSource.PlayOneShot(this.noAmmoSound);
                }
                return;
            }
            
            GameObject bulletObj = GameObject.Instantiate(this.bulletPrefab, this.bulletSpawn.position, Quaternion.Euler(0, this.AimingAngle + Random.Range(-6f, 6f), 0));
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Shooter = this;

            this.ammo--;
            
            this.audioSource.PlayOneShot(this.shootSound);
        }
    }

    public void ShootAt(Vector3 targetPosition)
    {
        if (this.shootTimer >= this.shootCooldown)
        {
            if (!Physics.Raycast(this.transform.position, (this.AttackTarget.transform.position - this.transform.position).normalized, Vector3.Distance(this.transform.position, this.AttackTarget.transform.position), this.bulletBlockingLayers))
            {
                this.rotatingTransform.LookAt(targetPosition);
                this.Shoot();
            }
        }
    }

    public void ReceiveDamageByAttack(float damage)
    {
        this.health -= damage;

        if (this.health <= 0)
        {
            this.GetComponent<DeadScientist>().enabled = true;
            this.healthBarFillImage.transform.parent.gameObject.SetActive(false);
        }
    }
}
