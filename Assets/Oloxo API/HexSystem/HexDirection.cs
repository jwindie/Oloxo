namespace Oloxo.HexSystem {

    public enum HexDirection {
        NE, E, SE, SW, W, NW
    }

    public static class HexDirectionExtensions {

        /// <summary>
        /// Returns the direction opposite this one.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static HexDirection Opposite (this HexDirection direction) {
            return (int) direction < 3 ? (direction + 3) : (direction - 3);
        }

        public static HexDirection Previous (this HexDirection direction) {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }

        public static HexDirection Next (this HexDirection direction) {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
        }
    }
}