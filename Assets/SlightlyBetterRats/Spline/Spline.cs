using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBR {
    public class Spline {
        private int sampleCount;

        private Vector3[] vertices;
        private Vector3[] tangents;

        private CubicSegmentVector[] segments;
        private Sample[] samples;

        private float totalArcLength;

        private struct Sample {
            public float u;
            public float arc;
        }

        public struct SplineHitResult {
            public float arc;
            public Vector3 position;
            public Vector3 tangent;
        }

        public Spline(Vector3[] vertices, Vector3[] tangents, int sampleCount = 256) {
            UpdateSpline(vertices, tangents, sampleCount);
        }

        public void UpdateSpline(Vector3[] vertices, Vector3[] tangents, int sampleCount = 256) {
            this.vertices = vertices;
            this.tangents = tangents;
            this.sampleCount = sampleCount;

            CalcuateSegments();
            SampleCurve();
        }

        public static Vector3[] CalculateTangents(Vector3[] vertices, float scale = 1, bool loop = false) {
            Vector3[] tangents = new Vector3[vertices.Length];

            for (int i = 1; i < vertices.Length - 2; i++) {
                tangents[i] = (vertices[i + 1] - vertices[i - 1]) * scale;
            }

            if (loop) {
                tangents[0] = (vertices[1] - vertices[vertices.Length - 1]) * scale;
                tangents[vertices.Length - 1] = (vertices[0] - vertices[vertices.Length - 2]);
            } else {
                tangents[0] = (vertices[1] * 2 - vertices[2] - vertices[0]) * scale;
                tangents[vertices.Length - 1] = (vertices[vertices.Length - 2] * 2 - vertices[vertices.Length - 3] - vertices[vertices.Length - 1]) * -scale;
            }

            return tangents;
        }

        private void CalcuateSegments() {
            segments = new CubicSegmentVector[vertices.Length - 1];

            float uPer = 1.0f / (vertices.Length - 1);

            for (int i = 0; i < segments.Length; i++) {
                Vector3 v1 = vertices[i];
                Vector3 v2 = vertices[i + 1];

                Vector3 t1 = tangents[i];
                Vector3 t2 = tangents[i + 1];

                segments[i] = new CubicSegmentVector(i * uPer, (i + 1) * uPer, v1, v2, t1, t2);
            }
        }

        public Vector3 getLocation(float arc, bool uniform = true) {
            if (arc < 0) {
                return vertices[0];
            } else if (arc >= 1.0f) {
                return vertices[vertices.Length - 1];
            }

            float u = uniform ? arcToU(arc) : arc;

            // Parametric value per index.
            float uPer = 1.0f / (vertices.Length - 1);

            // Calculate index of correct segment.
            int index = Mathf.FloorToInt(u / uPer);

            // Pass calculated u to correct segment.
            return segments[index].getPoint(arc);
        }

        public Vector3 getDerivative(float arc, bool uniform = true) {
            if (arc < 0) {
                return tangents[0];
            } else if (arc >= 1.0f) {
                return tangents[tangents.Length - 1];
            }

            float u = uniform ? arcToU(arc) : arc;

            // Parametric value per index.
            float uPer = 1.0f / (vertices.Length - 1);

            // Calculate index of correct segment.
            int index = Mathf.FloorToInt(u / uPer);

            // Pass calculated u to correct segment.
            return segments[index].getDerivative(u).normalized;
        }

        private void SampleCurve() {
            samples = new Sample[sampleCount];

            totalArcLength = 0; // Total arc length so far.

            float uPer = 1.0f / (sampleCount - 1);

            samples[0].arc = 0;
            samples[0].u = 0;

            for (int i = 1; i < sampleCount; i++) {
                float u = i * uPer;
                Vector3 posPrev = getLocation(u - uPer, false);
                Vector3 pos = getLocation(u, false);
                float delta = (pos - posPrev).magnitude; // Approximate arc length from previous point to current.

                totalArcLength += delta;
                samples[i].arc = totalArcLength;
                samples[i].u = u;
            }

            // Normalize arc lengths in range [0, 1].
            for (int i = 0; i < sampleCount; i++) {
                samples[i].arc /= totalArcLength;
            }
        }

        private float arcToU(float arc) {
            // Check bounds
            if (arc <= 0) {
                return 0;
            } else if (arc >= 1) {
                return 1;
            }

            // Linear search for desired arc
            for (int i = 0; i < sampleCount - 1; i++) {
                float t = samples[i].arc;

                if (t == arc) {
                    return samples[i].u; // Special case where exact value is found.
                } else if (t < arc) {
                    // If cur < arc and next > arc, found currect range.
                    if (samples[i + 1].arc > arc) {
                        float tNext = samples[i + 1].arc;

                        // Map values.
                        return Mathf.Lerp(samples[i].u, samples[i + 1].u, (arc - t) / (tNext - t));
                    }
                }
            }

            return 1;
        }

        private float uToArc(float u) {
            // Check bounds
            if (u <= 0) {
                return 0;
            } else if (u >= 1) {
                return 1;
            }

            int i = Mathf.RoundToInt(u * (sampleCount - 1));
            return samples[i].arc;
        }

        public SplineHitResult findNearbyArcPosition(Vector3 pos, int samples, bool uniform = false) {
            SplineHitResult o = new SplineHitResult();

            float minDist = float.PositiveInfinity;

            for (int i = 0; i < samples; i++) {
                float u = i * 1.0f / samples;

                if (uniform) {
                    u = arcToU(u);
                }

                Vector3 v = getLocation(u, false);

                float d2 = Vector3.SqrMagnitude(v - pos);
                if (d2 < minDist) {
                    minDist = d2;
                    o.arc = u;
                    o.position = v;
                }
            }

            o.tangent = getDerivative(o.arc, false);
            o.arc = uToArc(o.arc);

            return o;
        }
    }
}