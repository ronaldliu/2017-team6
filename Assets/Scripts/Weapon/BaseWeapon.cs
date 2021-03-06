﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour {

    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float range = 20f;                       // The distance the gun can fire.
    public int clipSize = 30;
    public Light gunLight;                          // Reference to the light component.

    float nextTimeToFire;                                    // A timer to determine when to fire.
    Ray shootRay;                                   // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;                           // Reference to the audio source.
    AudioSource reloadAudio;                        // Reference to the reload audio source.
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
    private bool reloading = false;
    float reloadTimer = 0f;
    bool timerRunning = false;                      //timer begins at this value
    float reloadSpeed = 5.0f;                       //time reached to do something

    private int ammo;

    void Awake()
    {
        // Create a layer mask for the Shootable layer.
        shootableMask = LayerMask.GetMask("Shootable");

        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();

        ammo = clipSize;
    }

    public void Fire() {
        nextTimeToFire += Time.deltaTime;

        Debug.Log(nextTimeToFire + " " + timeBetweenBullets);

        if (nextTimeToFire >= timeBetweenBullets &&
            reloading == false &&
            ammo > 0) {
            Shoot();
        } else {
            DisableEffects();
        }
    }


    public void Reloading()
    {
        if (reloading)
        {
            return;
        }
        //do the reloading process
        reloading = true;

        timerRunning = true;

        if (timerRunning)
        {
            ReloadTimer();
        }

        ammo = clipSize;
        reloading = false;

        //play reloading sound
        //reloadAudio.Play();
    }


    void ReloadTimer()
    {
        reloadTimer += Time.deltaTime;
        if(reloadTimer > reloadSpeed)
        {
            reloadTimer = 0f;
            timerRunning = false;
        }
    }


    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLine.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot()
    {
        // Reset the timer.
        nextTimeToFire = 0f;
        ammo--;

        // Play the gun shot audioclip.
        //gunAudio.Play();

        // Enable the light.
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {

            print("shot object\n");

            // Set the second position of the line renderer to the point the raycast hit.
            gunLine.SetPosition(1, shootHit.point);
        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer to the fullest extent of the gun's range.
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }

}
