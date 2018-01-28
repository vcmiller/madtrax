using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBR;

public class WTFGameTheMapIsAttackingMe : MonoBehaviour {
    public Transform[] options;
    public GameObject prefab;

    public float cooldown = 10;
    public float speed = 10;

    public float spawnDelay1 = 1.0f;
    public float spawnDelay2 = 0.33f;
    public float disableDelay = 1;

    private CooldownTimer timer;
    private Transform chosen;

	// Use this for initialization
	void Start () {
        timer = new CooldownTimer(cooldown);
	}

    private void Update() {
        if (timer.Use()) {
            EnableMeshes();

            Invoke("SpawnCars", spawnDelay1);
            Invoke("SpawnCars", spawnDelay1 + spawnDelay2);
            Invoke("SpawnCars", spawnDelay1 + spawnDelay2 * 2);
            Invoke("DisableMeshes", spawnDelay1 + spawnDelay2 * 2 + disableDelay);
        }
    }

    void EnableMeshes() {
        chosen = options[Random.Range(0, options.Length)];
        chosen.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void SpawnCars () {
        for (int i = 0; i < chosen.childCount; i++) {
            var target = chosen.GetChild(i);

            var spawned = Instantiate(prefab, target.position, target.rotation);

            spawned.GetComponent<Bullet>().Shoot(target.forward * speed);
        }
	}

    void DisableMeshes() {
        chosen.gameObject.SetActive(false);
    }
}
