using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour {
    public GameObject target;
    private bool activated = true;
    private Image text;
    private Image glow;

    public float fadeIn1 = 0.1f;
    public float fadeIn2 = 0.4f;
    public float duration = 1.0f;
    public float fadeOut = 0.5f;

	// Use this for initialization
	void Start () {
        text = GetComponent<Image>();
        glow = transform.GetChild(0).GetComponent<Image>();

        text.enabled = false;
        glow.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!target && !activated) {
            activated = true;
            StartCoroutine(FadeIn());
        } else if (target) {
            activated = false;
        }
	}

    IEnumerator FadeIn() {
        Color c = text.color;
        c.a = 0;
        text.color = c;
        text.enabled = true;

        while (c.a < 1) {
            c.a += Time.deltaTime / fadeIn1;
            text.color = c;
            yield return null;
        }

        c = glow.color;
        c.a = 0;
        glow.color = c;
        glow.enabled = true;

        while (c.a < 1) {
            c.a += Time.deltaTime / fadeIn1;
            glow.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        while (c.a > 0) {
            c.a -= Time.deltaTime / fadeOut;
            glow.color = c;
            text.color = c;

            yield return null;
        }

        text.enabled = false;
        glow.enabled = false;
    }
}
