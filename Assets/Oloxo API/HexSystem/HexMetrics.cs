using UnityEngine;

namespace Oloxo.HexSystem {

    public static class HexMetrics {
        public const float OUTER_RADIUS = 5f;
        public const float INNER_RADIUS = OUTER_RADIUS * 0.866025404f;
        public static Vector3[] CORNERS = {
            new Vector3(0f, 0f, OUTER_RADIUS),
            new Vector3(INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
            new Vector3(INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
            new Vector3(0f, 0f, -OUTER_RADIUS),
            new Vector3(-INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
            new Vector3(-INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
            new Vector3(0f, 0f, OUTER_RADIUS),
        };

        public const int CHUNK_SIZE_X = 5;
        public const int CHUNK_SIZE_Z = 6;
        public const int MAX_RIVERS_PER_TILE = 4;

        public static Vector3 GetFirstCorner (HexDirection direction) {
            return CORNERS[(int) direction];
        }

        public static Vector3 GetSecondCorner (HexDirection direction) {
            return CORNERS[(int) direction + 1];
        }
    }
}