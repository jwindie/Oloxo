//Author: Jordan Williams

#if UNITY_EDITOR
using UnityEditor;

//Set the scale of all the imported models to  "globalScaleModifier"
//and dont generate materials for the imported objects

public class CustomImportSettings : AssetPostprocessor {
    void OnPreprocessModel () {
        ModelImporter i = (ModelImporter) assetImporter;
        // model
        //i.globalScale = .01f;
        i.useFileScale = false;
        i.importBlendShapes = false;
        i.importVisibility = false;
        i.importCameras = false;
        i.importLights = false;
        i.preserveHierarchy = false;
        i.sortHierarchyByName = false;

        i.meshCompression = ModelImporterMeshCompression.Off;
        i.isReadable = true;
        i.meshOptimizationFlags = MeshOptimizationFlags.Everything;
        i.addCollider = false;

        i.keepQuads = false;
        i.weldVertices = true;
        i.indexFormat = ModelImporterIndexFormat.Auto;
        i.importBlendShapeNormals = ModelImporterNormals.None;
        //i.importNormals = ModelImporterNormals.Calculate;
        i.normalCalculationMode = ModelImporterNormalCalculationMode.Unweighted;
        i.normalSmoothingSource = ModelImporterNormalSmoothingSource.None;
        i.importTangents = ModelImporterTangents.CalculateMikk;
        i.swapUVChannels = false;
        i.generateSecondaryUV = false;
        i.normalSmoothingAngle = 0;

        //rig
        i.animationType = ModelImporterAnimationType.None;

        //animation
        i.importConstraints = false;
        i.importAnimation = false;

        //materials
        i.materialImportMode = ModelImporterMaterialImportMode.None;
    }
}
#endif