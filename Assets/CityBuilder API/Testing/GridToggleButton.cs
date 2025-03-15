using ModelShark;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.UI {
    public class GridToggleButton : Singleton<GridToggleButton> {
        [SerializeField] private Material material;
        [SerializeField] private Sprite gridOnIcon;
        [SerializeField] private Sprite gridOffIcon;
        [SerializeField] private Image image;


        private void Start () {
            if (material) {
                image.sprite = material.GetInt ("_EnableGrid") == 0 ? gridOnIcon : gridOffIcon;
            }
        }
        public void ToggleGrid (bool state) {
            if (material) {
                if (state) {
                    image.sprite = gridOffIcon;
                    material.SetInt ("_EnableGrid", 1);
                }
                else {
                    image.sprite = gridOnIcon;
                    material.SetInt ("_EnableGrid", 0);
                }
            }
        }

        public void ToggleGrid () {
            ToggleGrid (material.GetInt ("_EnableGrid") != 1); //int version of != true
        }
    }
}
