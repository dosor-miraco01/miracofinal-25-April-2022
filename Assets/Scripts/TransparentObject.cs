using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public bool scanChildren = true;

    private List<TransparentRendererObject> _renderers = new List<TransparentRendererObject>();

    private void Awake()
    {
        var main = FindObjectOfType<TransparentController>();

        if (main != null)
        {
            var r = GetComponent<Renderer>();
            if (r != null) _renderers.Add(TransparentRendererObject.Create(r));
            var items = GetComponentsInChildren<Renderer>();
            foreach (var re in items)
                _renderers.Add(TransparentRendererObject.Create(re, main.transparentShader));
        }
    }

    public void SetValue(float v)
    {
        //foreach (var it in _renderers)
        //    it.SetValue(v);
    }// method
}


struct TransparentRendererObject
{
    public Renderer renderer;
    private Material[] materials;
    private float[] values;
    private float[] rsValues;
    private Shader transparentShader;
    private Shader[] shaders;
    public static TransparentRendererObject Create(Renderer r, Shader transparentShader = null)
    {
        TransparentRendererObject obj = new TransparentRendererObject();
        obj.renderer = r;
        obj.ScanMaterials();
        obj.transparentShader = transparentShader;
        return obj;
    }

    private void ScanMaterials()
    {
        if (materials == null)
        {
            materials = renderer.materials;
            values = new float[materials.Length];
            rsValues = new float[materials.Length];
            shaders = new Shader[materials.Length];
            Color c = Color.white;

            for (int i = 0; i < materials.Length; i++)
            {
                c = materials[i].color;
                shaders[i] = materials[i].shader;
                values[i] = c.a;
                rsValues[i] =
#if AC_URP
                materials[i].GetFloat("_Surface");
#else
                materials[i].GetFloat("_Mode");
#endif
            }
        }
    }

    public void SetValue(float tran)
    {
        Color c = Color.white;

        for (int i = 0; i < materials.Length; i++)
        {
            c = materials[i].color;
            c.a = tran;
            materials[i].color = c;
            //if (tran < 1.0f) materials[i].shader = transparentShader;
            //else materials[i].shader = shaders[i];
            //if (tran < 1.0f) ChangeWallTransparency(materials[i], rsValues[i], true); // materials[i].SetFloat("__surface", 1.0f);
            //else ChangeWallTransparency(materials[i], rsValues[i], false);//materials[i].SetFloat("__surface", surfaceTypeValues[i]);
        }
    }

    public void ChangeWallTransparency(Material m, float defaultValue, bool transparent)
    {

        if (transparent)
        {
#if AC_URP
            m.SetFloat("_Surface", 1);
            m.SetFloat("_Blend", 0);
#else
            m.SetFloat("_Mode", 3);
#endif
        }
        else
        {
#if AC_URP
            m.SetFloat("_Surface", 0);
#else
            m.SetFloat("_Mode", defaultValue);
#endif
        }
        SetupMaterialBlendMode(m);
    }

    void SetupMaterialBlendMode(Material material)
    {
        float blendMode = material.GetFloat("_Mode");
        switch (blendMode)
        {
            case 0:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case 1:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case 2:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case 3:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }//method

#if AC_URP
    void SetupMaterialBlendMode(Material material)
    {

        bool alphaClip = material.GetFloat("_AlphaClip") == 1;
        if (alphaClip)
            material.EnableKeyword("_ALPHATEST_ON");
        else
            material.DisableKeyword("_ALPHATEST_ON");
        float surfaceType = material.GetFloat("_Surface");
        if (surfaceType == 0)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
            material.SetShaderPassEnabled("ShadowCaster", true);
        }
        else
        {
            float blendMode = material.GetFloat("_Blend");
            //   UnityEditor.BaseShaderGUI.BlendMode
            switch (blendMode)
            {
                case 0:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case 1:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case 2:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case 3:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
            }
        }
    }
#endif
}
