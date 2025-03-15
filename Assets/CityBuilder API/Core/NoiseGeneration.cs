using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {
    public static class NoiseGeneration {

        private static Dictionary<string, NoisePreset> presetDictionary = new Dictionary<string, NoisePreset> () {
            {"TreeSize",new NoisePreset {
                scale = Vector2.one * .45f,
                amplitude = 2f,
                min = .6f,
                max = 1.2f,
            }}
        };


        [SerializeField]
        public struct NoisePreset {
            public Vector2 scale;
            public float amplitude;
            public float min;
            public float max;
        }


        public static float SampleNoise (NoisePreset preset, float x, float y) {

            float val = Mathf.PerlinNoise (x * preset.scale.x, y * preset.scale.y) * preset.amplitude;

            if (preset.min > 0 && preset.max > 0) val = Mathf.Clamp (val, preset.min, preset.max);

            return val;
        }

        public static float SampleNoise (string presetName, float x, float y) {
            NoisePreset preset = new NoisePreset ();
            presetDictionary.TryGetValue (presetName, out preset);

            return SampleNoise (preset, x, y);
        }

        public static void AddPreset (string name, NoisePreset preset) {
            presetDictionary.Add (name, preset);
        }

        public static void RemovePreset (string name) {
            presetDictionary.Remove (name);
        }
    }
}