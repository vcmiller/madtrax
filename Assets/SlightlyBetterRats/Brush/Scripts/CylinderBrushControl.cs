using UnityEngine;
using System.Collections.Generic;
using System;

namespace SBR {
    public class CylinderBrushControl : BrushControl {

        public float radius = 1;
        public float height = 1;

        public float texXScale = 1.0f;
        public float texYScale = 1.0f;
        public float texWrapScale = 1.0f;
        public float texZScale = 1.0f;

        new private MeshCollider collider;

        protected override int GetNumSubmeshes() {
            return 3;
        }

        protected override int[] GetSubmeshIndices(int index) {
            List<int> indices = new List<int>();
            for (int i = 0; i < mesh.triangles.Length; i++) {
                int vindex = mesh.triangles[i];
                Vector3 normal = mesh.normals[vindex];
                if (normal.y > .5f && index == 1) {
                    indices.Add(vindex);
                } else if (normal.y < -.5f && index == 2) {
                    indices.Add(vindex);
                } else if (normal.y >= -.5f && normal.y <= .5f && index == 0) {
                    indices.Add(vindex);
                }
            }

            return indices.ToArray();
        }

        protected override void UpdateCollider() {
            collider = GetComponent<MeshCollider>();

            collider.sharedMesh = scaledMesh;
        }

        protected override void UpdateMesh() {
            Matrix4x4 mat = Matrix4x4.Scale(new Vector3(radius * 2, height / 2.0f, radius * 2));

            for (int i = 0; i < scaledMesh.vertexCount; i++) {
                Vector3 v = vertices[i];
                if (!centered) {
                    v += new Vector3(0.5f, 1f, 0.5f);
                }

                scaledVertices[i] = mat.MultiplyPoint(v);
                scaledUV[i] = uv[i];

                if (Mathf.Abs(normals[i].y) > 0.5f) {
                    scaledUV[i].x *= radius / texWidth / texXScale;
                    scaledUV[i].y *= radius / texHeight / texZScale;
                } else {
                    scaledUV[i].x *= radius / texWidth / texWrapScale * 2;
                    scaledUV[i].y *= height / texHeight / texYScale;
                }
            }
        }
    }
}