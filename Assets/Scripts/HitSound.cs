using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    public GameObject source;
    private IEnumerator soundCoroutine;

    private void Awake()
    {
        soundCoroutine = DestroySounds();
        StartCoroutine(soundCoroutine);
    }

    public IEnumerator DestroySounds()
    {
        yield return new WaitForSeconds(3f);
        Destroy(source);
    }

}
