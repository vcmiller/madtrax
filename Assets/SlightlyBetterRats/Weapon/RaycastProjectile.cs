using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBR {
    public class RaycastProjectile : Projectile {
        private void Update() {
            Vector3 oldPosition = transform.position;
            transform.position += velocity * Time.deltaTime;

            RaycastHit hit;

            if (Physics.Linecast(oldPosition, transform.position, out hit)) {
                OnHitObject(hit.collider, hit.point, hit.normal);
            }
        }
    }
}