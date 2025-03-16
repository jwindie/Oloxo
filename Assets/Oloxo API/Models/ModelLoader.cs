using Oloxo.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Oloxo.Models {
    /// <summary>
    /// Class to load model information by enumerated name.
    /// </summary>
    public class ModelLoader : SingletonComponent<ModelLoader> {

        private Dictionary<ModelId, Model> modelDictionary;

        [SerializeField] private Model[] models = new Model[0];

        public ModelLoader Init () {

            SetSingletonInstance (this);

            modelDictionary = new Dictionary<ModelId, Model> ();

            return this;
        }

        /// <summary>
        /// Loads models using the editor serialized array of models.
        /// </summary>
        public void Load () {
            Load (models);
            Debug.LogWarning ("Remove this in favor of non-editor loading.");
        }

        public void Load (params Model[] models) {
            App.Current.LogHandler.StartBlock ($"[LOAD] Loading models...");
            for (int i = 0 ; i < models.Length ; i++) {
                LoadModel (models[i]);
            }
            App.Current.LogHandler.EndBlock ();
        }

        public Model GetModel (ModelId id) {
            if (modelDictionary.ContainsKey (id)) {
                return modelDictionary[id];
            }
            else return default;
        }

        private void LoadModel (Model m) {

            //overweite existing model entries
            if (modelDictionary.ContainsKey (m.Id)) {
                modelDictionary[m.Id] = m;
            }
            else {
                modelDictionary.Add (m.Id, m);
            }

            string meshName = m.Mesh != null ? m.Mesh.name : "NULL";
            App.Current.LogHandler.Log ($"[LOAD] {m.Id}: {meshName}");
        }
    }
}
