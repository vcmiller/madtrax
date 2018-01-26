using UnityEngine;
using System.Collections.Generic;

namespace SBR {
    public class SlantBrushControl : BrushControl {
        public float width = 1;
        public float height = 1;
        public float depth = 1;

        public float texXScale = 1.0f;
        public float texYScale = 1.0f;
        public float texZScale = 1.0f;
        public float texSlantScale = 1.0f;

        new private MeshCollider collider;

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
                } else if (normals[i].z < -0.5f) {
                    scaledUV[i].x *= width / texWidth / texXScale;
                    scaledUV[i].y *= height / texHeight / texYScale;
                } else if (normals[i].y < -.5f) {
                    scaledUV[i].x *= width / texWidth / texXScale;
                    scaledUV[i].y *= depth / texHeight / texZScale;
                } else {
                    scaledUV[i].x *= width / texWidth / texXScale;
                    scaledUV[i].y *= Mathf.Max(height, depth) / texHeight / texSlantScale;
                }
            }
        }

        protected override int[] GetSubmeshIndices(int index) {
            List<int> indices = new List<int>();
            for (int i = 0; i < mesh.triangles.Length; i++) {
                int vindex = mesh.triangles[i];
                Vector3 normal = mesh.normals[vindex];
                if (normal.x > .5f && index == 0) {
                    indices.Add(vindex);
                } else if (normal.x < -.5f && index == 1) {
                    indices.Add(vindex);
                } else if (normal.y < -.5f && index == 2) {
                    indices.Add(vindex);
                } else if (normal.z < -.5f && index == 3) {
                    indices.Add(vindex);
                } else if (normal.z > .1f && normal.y > .1f && index == 4) {
                    indices.Add(vindex);
                }
            }

            return indices.ToArray();
        }

        protected override int GetNumSubmeshes() {
            return 5;
        }

        protected override void UpdateCollider() {
            collider = GetComponent<MeshCollider>();

            collider.sharedMesh = scaledMesh;
        }
    }
}