using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {


    public static class AtlasSampler {

        private const int ATLAS_TILE_DIMENSIONS = 16;
        private const float ATLAS_TILE_WIDTH = 1f / ATLAS_TILE_DIMENSIONS;
        private const float ATLAS_TILE_WIDTH_HALF = ATLAS_TILE_WIDTH / 2f;

        private static Texture2D atlas;

        public static void SetAtlas (Texture2D atlas) {
            AtlasSampler.atlas = atlas;
        }

        public static Vector2 GetUV (Vector2Int tileCoords) {
            return new Vector2 (
                (tileCoords.x * ATLAS_TILE_WIDTH) + ATLAS_TILE_WIDTH_HALF,
                (tileCoords.y * ATLAS_TILE_WIDTH) + ATLAS_TILE_WIDTH_HALF
                );
        }
        public static Vector2 GetUV (int x, int y) {
            return new Vector2 (
                (x * ATLAS_TILE_WIDTH) + ATLAS_TILE_WIDTH_HALF,
                (y * ATLAS_TILE_WIDTH) + ATLAS_TILE_WIDTH_HALF
                );
        }
    }
}
