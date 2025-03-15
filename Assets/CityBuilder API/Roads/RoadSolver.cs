using Citybuilder.Core;
using Citybuilder.ProceduralGeometry;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Roads {

    public class RoadSolverData {
        public RoadType[] neighborTypes;
        public RoadType centerType;

        public RoadSolverData (RoadType roadType = RoadType.None) {
            neighborTypes = new RoadType[4];
        }
    }

    public static class RoadSolver {

        private static Vector2 uv_road;
        private static Vector2 uv_sidewalk;


        private static readonly RoadParams params_basicRoad = new RoadParams {
            tileWidth = 1,
            roadDepth = .03f,
            roadwayWidth = .55f,
            medianSize = 0,
            sidewalkWidth = 0.225f
        };

        private static Dictionary<RoadType, RoadParams> roadParamsDictionary;

        public static void Init () {
            uv_road = AtlasSampler.GetUV (0, 8); //road UV
            uv_sidewalk = AtlasSampler.GetUV (0, 7); //road UV


            roadParamsDictionary = new Dictionary<RoadType, RoadParams> ();
            roadParamsDictionary.Add (RoadType.Basic, params_basicRoad);
        }



        public static void GenerateMesh (ChunkMesh chunkMesh, RoadSolverData data, Vector3 position) {

            //Debug.Log ("SOLVING ROADS");
            //draw sunken road tile first
            float depth = roadParamsDictionary[data.centerType].roadDepth;
            float halfWidth = roadParamsDictionary[data.centerType].roadwayWidth / 2f;

            //Vector3[] points = GetPointsOnRadius (halfWidth, 4, 4, 0, position);
            //for (int i = 1 ; i < points.Length ; i++) {
            //    chunkMesh.AddTriangle (points[i - 1], points[i], position);
            //    chunkMesh.AddTriangleUV (uv_sidewalk);
            //}
            //chunkMesh.AddTriangle (points[points.Length - 1], points[0], position);
            //chunkMesh.AddTriangleUV (uv_sidewalk);

            //return;

            Vector3 p0 = position + new Vector3 (-halfWidth, -depth, halfWidth);
            Vector3 p1 = position + new Vector3 (halfWidth, -depth, halfWidth);
            Vector3 p2 = position + new Vector3 (halfWidth, -depth, -halfWidth);
            Vector3 p3 = position + new Vector3 (-halfWidth, -depth, -halfWidth);
            chunkMesh.AddQuad (p0, p1, p2, p3);
            chunkMesh.AddQuadUV (uv_road);

            //draw the sidewalks
            Vector3 p0a = position + new Vector3 (-halfWidth, 0, halfWidth);
            Vector3 p1a = position + new Vector3 (halfWidth, 0, halfWidth);
            Vector3 p2a = position + new Vector3 (halfWidth, 0, -halfWidth);
            Vector3 p3a = position + new Vector3 (-halfWidth, 0, -halfWidth);

            float sidewalkWidth = roadParamsDictionary[data.centerType].sidewalkWidth;
            Vector3 p0b = position + new Vector3 (-(sidewalkWidth + halfWidth), 0, sidewalkWidth + halfWidth);
            Vector3 p1b = position + new Vector3 (sidewalkWidth + halfWidth, 0, sidewalkWidth + halfWidth);
            Vector3 p2b = position + new Vector3 (sidewalkWidth + halfWidth, 0, -(sidewalkWidth + halfWidth));
            Vector3 p3b = position + new Vector3 (-(sidewalkWidth + halfWidth), 0, -(sidewalkWidth + halfWidth));

            //draw curbs
            chunkMesh.AddQuad (p0, p0a, p1a, p1);
            chunkMesh.AddQuad (p1, p1a, p2a, p2);
            chunkMesh.AddQuad (p2, p2a, p3a, p3);
            chunkMesh.AddQuad (p3, p3a, p0a, p0);
            chunkMesh.AddQuadUV (uv_sidewalk);
            chunkMesh.AddQuadUV (uv_sidewalk);
            chunkMesh.AddQuadUV (uv_sidewalk);
            chunkMesh.AddQuadUV (uv_sidewalk);

            //draw sidewalk surfaces
            chunkMesh.AddQuad (p0a, p0b, p1b, p1a);
            chunkMesh.AddQuad (p1a, p1b, p2b, p2a);
            chunkMesh.AddQuad (p2a, p2b, p3b, p3a);
            chunkMesh.AddQuad (p3a, p3b, p0b, p0a);
            chunkMesh.AddQuadUV (uv_sidewalk);
            chunkMesh.AddQuadUV (uv_sidewalk);
            chunkMesh.AddQuadUV (uv_sidewalk);
            chunkMesh.AddQuadUV (uv_sidewalk);
        }

        private static Vector3[] GetPointsOnRadius (float radius, int samples, int quadrants, float startingAngle, Vector3 offset) {
            Vector3[] points = new Vector3[samples * quadrants];
            float interval = 90f / samples;
            float t = startingAngle;
            for (int i = 0 ; i < samples * quadrants ; i++) {
                points[i] = new Vector3 (
                    radius * Mathf.Cos (t),
                    0,
                    radius * Mathf.Cos (t)
                    ) + offset;
                t += interval;
            }
            return points;
        }
    }
}
