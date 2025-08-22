using UnityEngine;

public class ShroomCustomizerMPB : MonoBehaviour
{
    [Header("Separate renderers")]
    public Renderer body;        // Body object (URP Lit / Standard)
    public Renderer cap;         // Cap (top only)
    public Renderer band;        // Strap around the chest (always = cap color)
    public Renderer mouthQuad;   // Unlit/Transparent material

    [Header("Face options")]
    public GameObject[] eyeVariants;   // eyesDead, eyesLarge, eyesSmall, etc.
    public Texture2D[] mouthOptions;   // transparent PNGs for the mouth

    // MPBs let us override material values per-renderer without duplicating materials
    MaterialPropertyBlock _mpbBody, _mpbCap, _mpbBand, _mpbMouth;

    // Property IDs (URP + Built-in). We write both so it works in either pipeline.
    static readonly int BaseColor = Shader.PropertyToID("_BaseColor"); // URP Lit/Simple Lit
    static readonly int ColorProp = Shader.PropertyToID("_Color");     // Built-in Standard
    static readonly int BaseMap   = Shader.PropertyToID("_BaseMap");
    static readonly int MainTex   = Shader.PropertyToID("_MainTex");

    void Awake() {
        _mpbBody = new MaterialPropertyBlock();
        _mpbCap  = new MaterialPropertyBlock();
        _mpbBand = new MaterialPropertyBlock();
        _mpbMouth= new MaterialPropertyBlock();
    }

    void Start() {
        // Make sure band starts as the same color as the cap material's initial color
        if (cap && band) {
            var c = ReadColorFromShared(cap);
            ApplyColor(band, _mpbBand, c);
        }
    }

    // ---------- Colors ----------
    public void SetBodyColor(Color c) {
        ApplyColor(body, _mpbBody, c);
    }

    public void SetCapColor(Color c) {
        ApplyColor(cap,  _mpbCap,  c);  // set cap
        ApplyColor(band, _mpbBand, c);  // strap follows cap
    }

    // Helper to apply a color via MPB to a specific renderer+slot
    void ApplyColor(Renderer r, MaterialPropertyBlock mpb, Color c) {
        if (!r) return;
        r.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColor, c);  // URP
        mpb.SetColor(ColorProp, c);  // Built-in
        r.SetPropertyBlock(mpb);
    }

    // Read a color from the shared material (used to init the band once)
    Color ReadColorFromShared(Renderer r) {
        if (!r || !r.sharedMaterial) return Color.white;
        var m = r.sharedMaterial;
        if (m.HasProperty(BaseColor)) return m.GetColor(BaseColor);
        if (m.HasProperty(ColorProp)) return m.GetColor(ColorProp);
        return Color.white;
    }

    // Hex helpers for UI buttons or your color wheel wrapper
    public void SetBodyHex(string hex) { if (ColorUtility.TryParseHtmlString(hex, out var c)) SetBodyColor(c); }
    public void SetCapHex (string hex) { if (ColorUtility.TryParseHtmlString(hex, out var c)) SetCapColor(c);  }

    // ---------- Eyes (3D variants as GameObjects) ----------
    int _currentEyes = 0;
    public void SetEyesIndex(int i) {
        if (eyeVariants == null || eyeVariants.Length == 0) return;
        i = Mathf.Clamp(i, 0, eyeVariants.Length - 1);
        for (int k = 0; k < eyeVariants.Length; k++)
            if (eyeVariants[k]) eyeVariants[k].SetActive(k == i);
        _currentEyes = i;
    }

    // ---------- Mouth (swap texture on single quad) ----------
    int _currentMouth = 0;
    public void SetMouthIndex(int i) {
        if (!mouthQuad || mouthOptions == null || mouthOptions.Length == 0) return;
        i = Mathf.Clamp(i, 0, mouthOptions.Length - 1);
        mouthQuad.GetPropertyBlock(_mpbMouth);
        _mpbMouth.SetTexture(BaseMap, mouthOptions[i]); // URP
        _mpbMouth.SetTexture(MainTex, mouthOptions[i]); // Built-in
        mouthQuad.SetPropertyBlock(_mpbMouth);
        _currentMouth = i;
    }
}
