using UnityEngine;

namespace Oloxo.HexSystem {

    /// <summary>
    /// Represents cubic coordinates.
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates {

        [SerializeField] int x, z;

        public int X {
            get {
                return x;
            }
        }

        public int Z {
            get {
                return z;
            }
        }
        public int Y {
            get {
                return -X - Z;
            }
        }

        public HexCoordinates (int x, int z) {
            this.x = x;
            this.z = z;
        }

        /// <summary>
        /// Create a set of coordinates from regular offset (zig zag) coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static HexCoordinates FromOffsetCoordinates (int x, int z) {
            return new HexCoordinates (x - z / 2, z);
        }

        /// <summary>
        /// Creates a set of coordinates from a world position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static HexCoordinates FromPosition (Vector3 position) {
            float x = position.x / (HexMetrics.INNER_RADIUS * 2f);
            float y = -x;

            //shift the z to undo the offset
            float offset = position.z / (HexMetrics.OUTER_RADIUS * 3f);
            x -= offset;
            y -= offset;

            //round them to integers to get centers
            //derive Z and construct final coord
            int iX = Mathf.RoundToInt (x);
            int iY = Mathf.RoundToInt (y);
            int iZ = Mathf.RoundToInt (-x - y);

            //correct for rounding issues
            //discard the value with th ebiggest rounding value 
            //rebuild coords from other 2 values
            if (iX + iY + iZ != 0) {
                float dX = Mathf.Abs (x - iX);
                float dY = Mathf.Abs (y - iY);
                float dZ = Mathf.Abs (-x - y - iZ);

                if (dX > dY && dX > dZ) {
                    iX = -iY - iZ;
                }
                else if (dZ > dY) {
                    iZ = -iX - iY;
                }
            }

            return new HexCoordinates (iX, iZ);
        }

        public override string ToString () {
            return "(" +
                X.ToString () + ", " + Y.ToString () + ", " + Z.ToString () + ")";
        }

        /// <summary>
        /// String override that prints to separate lines.
        /// </summary>
        /// <returns></returns>
        public string ToStringOnSeparateLines () {
            return X.ToString () + "\n" + Y.ToString () + "\n" + Z.ToString ();
        }
    }
}