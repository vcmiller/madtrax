using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBR {
    public class RaycastProjectile2D : Projectile {
        private void Update() {
            Vector3 oldPosition = transform.position;
            transform.position += velocity * Time.deltaTime;

            RaycastHit2D hit;

            if (hit = Physics2D.Linecast(oldPosition, transform.position)) {
                OnHitObject(hit.collider, hit.point, hit.normal);
            }
        }
    }
}