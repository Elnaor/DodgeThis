using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulse : MonoBehaviour
{
    public Anchors anchor;

    private void OnTriggerEnter2D(Collider2D item)
    {
        if (item.tag != "Bouncer" && item.tag != "Anchor") {
            if (!anchor.repulseArea.Contains(item.gameObject)) {
                anchor.repulseArea.Add(item.gameObject);
            }
        }
    }

    IEnumerator RepulseWait(GameObject item)
    {
        yield return new WaitForSecondsRealtime(1);
        if (item != null) {
            if (item.tag != "Bouncer" && item.tag != "Anchor") {
                if (anchor.repulseArea.Contains(item.gameObject)) {
                    anchor.repulseArea.Remove(item.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D item)
    {
        StartCoroutine(RepulseWait(item.gameObject));
    }
}
