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
    public static readonly Shader Zoom = LoadShader(
        Path.Combine("Assets", "Shaders", "ZoomShader.shader")
    );

    public static AssetBundle PWBundle
    {
        get
        {
            if (bundleInt == null)
            {
                bundleInt = RATSMod.mod.MainBundle;
                Log.Message("bundleInt: " + bundleInt.name);
            }
            return bundleInt;
        }
    }

    public static Shader LoadShader(string shaderName)
    {
        if (lookupShaders == null)
        {
            lookupShaders = new Dictionary<string, Shader>();
        }
        if (!lookupShaders.ContainsKey(shaderName))
        {
            Log.Message("lookupShaders: " + lookupShaders.ToList().Count);
            lookupShaders[shaderName] = PWBundle.LoadAsset<Shader>(shaderName);
        }
        Shader shader = lookupShaders[shaderName];
        if (shader == null)
        {
            Log.Warning("Could not load shader: " + shaderName);
            return ShaderDatabase.DefaultShader;
        }
        if (shader != null)
        {
            Log.Message("Loaded shaders: " + lookupShaders.Count);
        }
        return shader;
    }
}
