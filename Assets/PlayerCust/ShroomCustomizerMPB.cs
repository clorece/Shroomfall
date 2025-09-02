using UnityEngine;

public class ShroomCustomizerMPB : MonoBehaviour
{
    [Header("Renderers")]
    public Renderer body;
    public Renderer cap;
    public Renderer band;
    public Renderer mouthQuad;

    [Header("Variants")]
    public GameObject[] eyeVariants;
    public Texture2D[] mouthOptions;

    // one block per renderer (reused)
    MaterialPropertyBlock _mpbBody, _mpbCap, _mpbBand, _mpbMouth;

    MaterialPropertyBlock GetBlock(ref MaterialPropertyBlock b)
    {
        if (b == null) b = new MaterialPropertyBlock();
        else b.Clear();
        return b;
    }

    // ---- Colors ----
    public void SetBodyColor(Color c)
    {
        if (!body) return;
        var b = GetBlock(ref _mpbBody);
        body.GetPropertyBlock(b);
        b.SetColor("_BaseColor", c);   // URP
        b.SetColor("_Color", c);       // Built-in fallback
        body.SetPropertyBlock(b);
    }

    public void SetCapColor(Color c)
    {
        if (cap)
        {
            var b = GetBlock(ref _mpbCap);
            cap.GetPropertyBlock(b);
            b.SetColor("_BaseColor", c);
            b.SetColor("_Color", c);
            cap.SetPropertyBlock(b);
        }
        if (band)
        {
            var b = GetBlock(ref _mpbBand);
            band.GetPropertyBlock(b);
            b.SetColor("_BaseColor", c);
            b.SetColor("_Color", c);
            band.SetPropertyBlock(b);
        }
    }

    // ---- Eyes ----
    public void SetEyesIndex(int i)
    {
        if (eyeVariants == null || eyeVariants.Length == 0) return;
        i = (i % eyeVariants.Length + eyeVariants.Length) % eyeVariants.Length;
        for (int k = 0; k < eyeVariants.Length; k++)
            if (eyeVariants[k]) eyeVariants[k].SetActive(k == i);
    }

    // ---- Mouth (texture swap on the mouth plane) ----
    public void SetMouthIndex(int i)
    {
        if (!mouthQuad || mouthOptions == null || mouthOptions.Length == 0) return;
        i = (i % mouthOptions.Length + mouthOptions.Length) % mouthOptions.Length;

        var tex = mouthOptions[i];
        var b = GetBlock(ref _mpbMouth);
        mouthQuad.GetPropertyBlock(b);
        b.SetTexture("_BaseMap", tex); // URP
        b.SetTexture("_MainTex", tex); // Built-in fallback
        // ensure visible (white, full alpha)
        b.SetColor("_BaseColor", Color.white);
        b.SetColor("_Color", Color.white);
        mouthQuad.SetPropertyBlock(b);
    }
}
