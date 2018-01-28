using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SBR;

public class DeathScreen : MonoBehaviour {
    public Health target;
    private bool activated;
    private Image text;

    public float fadeIn1 = 0.1f;
    public float duration = 1.0f;
    public float fadeOut = 0.5f;

    // Use this for initialization
    void Start() {
        text = GetComponent<Image>();

        text.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (target.health <= 0 && !activated) {
            activated = true;
            StartCoroutine(FadeIn());
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
        

        yield return new WaitForSeconds(duration);

        while (c.a > 0) {
            c.a -= Time.deltaTime / fadeOut;
            text.color = c;

            yield return null;
        }

        text.enabled = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
