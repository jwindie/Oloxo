using Citybuilder.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Testing {
    public class MapEditor : Singleton<MapEditor> {

        [System.Serializable]
        public class EditorSetting {
            public bool enabled;
            public bool state;
        }

        public EditorSetting terrainSetting;
        public EditorSetting treeSetting;
        public EditorSetting roadSetting;
        public EditorSetting hollowSetting;
        public EditorSetting harborSetting;

        public EditorSetting GetSetting (string setting) {
            switch (setting) {

                case "TERRAIN":
                    return terrainSetting;

                case "TREE":
                    return treeSetting;

                case "ROAD":
                    return roadSetting;

                case "HOLLOW":
                    return hollowSetting;


                case "HARBOR":
                    return harborSetting;

                default: return null;
            }
        }


        public void UpdateSetting (string setting, bool enabled, bool state) {

            switch (setting) {

                case "TERRAIN":
                    terrainSetting.enabled = enabled;
                    terrainSetting.state = state;
                    return;

                case "TREE":
                    treeSetting.enabled = enabled;
                    treeSetting.state = state;
                    return;

                case "ROAD":
                    roadSetting.enabled = enabled;
                    roadSetting.state = state;
                    return;

                case "HOLLOW":
                    hollowSetting.enabled = enabled;
                    hollowSetting.state = state;
                    return;

                case "HARBOR":
                    harborSetting.enabled = enabled;
                    harborSetting.state = state;
                    return;
            }
        }
    }
}
