using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class FootStep : MonoBehaviour
{
    [SerializeField] private List<AudioClip> footStepsmp3s;
    Transform player;
    private AudioSource src;
    private int mp3Index = 0;
        
    void Start()
    {
        player = transform.parent;
        src = player.GetComponent<AudioSource>();
    }

    public void PlayFootStepSound()
    {
        src.loop = false;
        src.clip = footStepsmp3s[mp3Index];
        if (!src.isPlaying)
        {
            src.Play();
            if (mp3Index < footStepsmp3s.Count - 1)
            {
                mp3Index++;
            }
            else
            {
                mp3Index = 0;
            }
        }
       

    }

   
}
