using Oloxo.Models;

namespace Oloxo {
    /// <summary>
    /// Representation of all types of Terrain possible in the game.
    /// </summary>
    public enum Terrain {
        Water,
        Grass,
        Beach,
        Mountain
    }

    public static class TerrainExtensionMethods {

        /// <summary>
        /// Returns the <see cref="ModelId"/> for this terrain.
        /// </summary>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static ModelId GetModelId (this Terrain terrain) {
            switch (terrain) {
                default:
                case Terrain.Water: return Models.ModelId.Terrain_Water;
                case Terrain.Grass: return Models.ModelId.Terrain_Land;
                case Terrain.Beach: return Models.ModelId.Terrain_Sand;
                case Terrain.Mountain: return Models.ModelId.Terrain_Mountain;
            }
        }
    }
}