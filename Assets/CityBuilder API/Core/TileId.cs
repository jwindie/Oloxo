namespace Citybuilder.Core { 
    public enum TileId {
        Grass = 1,
        Canal = 2,
        Road = 3,
        Pipeline = 4,
        Building = 5,
        Tree = 6,
        Zoning = 7,
        Wire = 8,
        WheatFarm = 9,
        Bush = 10,
        //
        NotGrass = -1,
        NotCanal = -2,
        NotRoad = -3,
        NotPipeline = -4,
        NotBuilding = -5,
        NotTree = -6,
        NotZoning = -7,
        NotWire = -8,
        NotWheatFarm = -9,
        NotBush = -10,
        //
        Any = int.MaxValue,
        Empty = 0,
    }
}