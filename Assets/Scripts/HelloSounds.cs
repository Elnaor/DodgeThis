using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class HelloSounds : MonoBehaviour
{
    public GameObject source;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void HelloButton()
    {
        Instantiate(source);
    }

}
