using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarFog : MonoBehaviour
{
    [Range(0, 3)]
    public float Lerp = 0;//ʹ������������������Ĵ�С
    public Texture2D MaskTex;
    public Shader ScreanShader;
    public Material GetMaterial
    {
        get
        {
            if (_material == null) _material = new Material(ScreanShader);
            return _material;
        }
    }
    private Material _material = null;
    //src���������ȡ������Ƭ��dest�Ǵ������ͼƬ
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        GetMaterial.SetTexture("_MainTex", src);
        GetMaterial.SetTexture("_MaskTex", MaskTex);
        GetMaterial.SetFloat("_Lerp", Lerp);
        Graphics.Blit(src, dest, GetMaterial);
    }
}