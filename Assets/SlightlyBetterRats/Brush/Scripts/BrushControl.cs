using UnityEngine;
using System.Collections;

namespace SBR {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Brush))]
    public abstract class BrushControl : MonoBehaviour {
        public float texWidth = 1.0f;
        public float texHeight = 1.0f;

        public bool splitMaterials = false;
        public bool centered = false;

        public Mesh mesh;
        protected Mesh scaledMesh;
        protected MeshFilter filter;
        new protected MeshRenderer renderer;


        protected Vector3[] vertices;
        protected Vector3[] scaledVertices;

        protected Vector2[] uv;
        protected Vector2[] scaledUV;

        protected Vector3[] normals;

        // Update is called once per frame
        void Update() {

            filter = GetComponent<MeshFilter>();
            renderer = GetComponent<MeshRenderer>();

            if (scaledMesh != null) {
                DestroyImmediate(scaledMesh);
            }

            scaledMesh = Instantiate(mesh);
            filter.mesh = scaledMesh;

            vertices = mesh.vertices;
            scaledVertices = scaledMesh.vertices;

            uv = mesh.uv;
            scaledUV = scaledMesh.uv;

            normals = mesh.normals;

            UpdateMesh();
            UpdateSubmeshes();

            scaledMesh.uv = scaledUV;
            scaledMesh.vertices = scaledVertices;
            scaledMesh.RecalculateBounds();

            UpdateCollider();
        }

        protected abstract void UpdateMesh();
        protected abstract void UpdateCollider();
        protected abstract int GetNumSubmeshes();
        protected abstract int[] GetSubmeshIndices(int index);

        protected virtual void UpdateSubmeshes() {
            if (splitMaterials) {
                int numSubmeshes = GetNumSubmeshes();

                scaledMesh.subMeshCount = numSubmeshes;

                for (int i = 0; i < numSubmeshes; i++) {
                    scaledMesh.SetTriangles(GetSubmeshIndices(i), i);
                }

                if (renderer.sharedMaterials.Length < numSubmeshes) {
                    Material m = renderer.sharedMaterials[0];
                    renderer.sharedMaterials = new Material[numSubmeshes];
                    for (int i = 0; i < numSubmeshes; i++) {
                        renderer.sharedMaterials[i] = m;
                    }
                }
            } else {
                Material m = renderer.sharedMaterial;
                renderer.sharedMaterials = new Material[1];
                renderer.sharedMaterial = m;
            }
        }
    }
}