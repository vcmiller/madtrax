using UnityEngine;
using System.Collections.Generic;
using System;

namespace SBR {
    public class BoxBrushControl : BrushControl {
        public float width = 1;
        public float height = 1;
        public float depth = 1;

        public float texXScale = 1.0f;
        public float texYScale = 1.0f;
        public float texZScale = 1.0f;

        new private BoxCollider collider;

        // Update is called once per frame
        protected override void UpdateMesh() {
            Matrix4x4 mat = Matrix4x4.Scale(new Vector3(width, height, depth));

            for (int i = 0; i < scaledMesh.vertexCount; i++) {
                Vector3 v = vertices[i];
                if (!centered) {
                    v += new Vector3(0.5f, 0.5f, 0.5f);
                }

                scaledVertices[i] = mat.MultiplyPoint(v);
                scaledUV[i] = uv[i];

                if (Mathf.Abs(normals[i].x) > 0.5f) {
                    scaledUV[i].x *= depth / texWidth * texZScale;
                    scaledUV[i].y *= height / texHeight * texYScale;
                } else if (Mathf.Abs(normals[i].z) > 0.5f) {
                    if (normals[i].z < 0) {
                        scaledUV[i].y = 1.0f - scaledUV[i].y;
                        scaledUV[i].x = 1.0f - scaledUV[i].x;
                    }

                    scaledUV[i].x *= width / texWidth / texXScale;
                    scaledUV[i].y *= height / texHeight / texYScale;
                } else {
                    scaledUV[i].x *= width / texWidth / texXScale;
                    scaledUV[i].y *= depth / texHeight / texZScale;
                }
            }
        }

        protected override int[] GetSubmeshIndices(int index) {
            int[] tris = new int[6];
            for (int j = 0; j < 6; j++) {
                tris[j] = mesh.triangles[index * 6 + j];
            }

            return tris;
        }

        protected override int GetNumSubmeshes() {
            return 6;
        }

        protected override void UpdateCollider() {
            collider = GetComponent<BoxCollider>();

            collider.size = new Vector3(width, height, depth);

            if (!centered) {
                collider.center = collider.size * 0.5f;
            } else {
                collider.center = Vector3.zero;
            }
        }
    }
}
