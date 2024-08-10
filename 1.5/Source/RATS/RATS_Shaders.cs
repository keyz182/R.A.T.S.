using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace RATS;

[StaticConstructorOnStartup]
public static class RATS_Shaders
{
    private static AssetBundle bundleInt;
    private static Dictionary<string, Shader> lookupShaders;
    private static Dictionary<string, Material> lookupMaterials;

    public static readonly Material ZoomMat = LoadMaterial(Path.Combine("Assets", "Shaders", "Unlit_ZoomShader.mat"));

    public static AssetBundle AssetBundle
    {
        get
        {
            if (bundleInt != null)
            {
                return bundleInt;
            }

            bundleInt = RATSMod.mod.MainBundle;
            Log.Message($"bundleInt: {bundleInt.name}");

            return bundleInt;
        }
    }

    public static Shader LoadShader(string shaderName)
    {
        lookupShaders ??= new Dictionary<string, Shader>();
        if (!lookupShaders.ContainsKey(shaderName))
        {
            Log.Message($"lookupShaders: {lookupShaders.ToList().Count}");
            lookupShaders[shaderName] = AssetBundle.LoadAsset<Shader>(shaderName);
        }

        Shader shader = lookupShaders[shaderName];
        if (shader == null)
        {
            Log.Warning($"Could not load shader: {shaderName}");
            return ShaderDatabase.DefaultShader;
        }

        if (shader != null)
        {
            Log.Message($"Loaded shaders: {lookupShaders.Count}");
        }

        return shader;
    }

    public static Material LoadMaterial(string matName)
    {
        lookupMaterials ??= new Dictionary<string, Material>();
        if (!lookupMaterials.ContainsKey(matName))
        {
            Log.Message($"lookupMaterials: {lookupMaterials.ToList().Count}");
            lookupMaterials[matName] = AssetBundle.LoadAsset<Material>(matName);
        }

        Material mat = lookupMaterials[matName];
        if (mat == null)
        {
            Log.Warning($"Could not load material: {matName}");
            return null;
        }

        if (mat != null)
        {
            Log.Message($"Loaded materials: {lookupMaterials.Count}");
        }

        return mat;
    }
}
