using UnityEngine;

public class ShroomCustomizer : MonoBehaviour
{
    public Renderer body;  // will auto-find by name if left empty
    public Renderer cap;   // will auto-find by name if left empty

    MaterialPropertyBlock _mpb;
    static readonly int _BaseColor = Shader.PropertyToID("_BaseColor"); // URP
    static readonly int _Color     = Shader.PropertyToID("_Color");     // Built-in

    void Awake() {
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        AutoBind();
    }
    void AutoBind() {
        if (body && cap) return;
        foreach (var r in GetComponentsInChildren<Renderer>(true)) {
            var n = r.name.ToLower();
            if (!body && n.Contains("body")) body = r;
            else if (!cap && n.Contains("cap")) cap = r;
        }
    }

    // ---- CAP COLOR ----
    public void SetCapColorHex(string hex) {
        if (ColorUtility.TryParseHtmlString(hex, out var c)) SetCapColor(c);
    }
    public void SetCapColor(Color c) {
        if (!cap) return;
        cap.GetPropertyBlock(_mpb);
        _mpb.SetColor(_BaseColor, c); // URP/Lit
        _mpb.SetColor(_Color,     c); // Built-in/Standard
        cap.SetPropertyBlock(_mpb);
    }

    // ---- BODY MATERIAL SWAP (by name from Resources/BodyMats) ----
    public void SetBodyMatByName(string nameInResources) {
        if (!body) return;
        var mat = Resources.Load<Material>("BodyMats/" + nameInResources);
        if (!mat) { Debug.LogWarning("No material: " + nameInResources); return; }

        // If Body has just one material slot, this replaces it.
        // If it has multiple, you can extend this, but most likely it's one.
        body.sharedMaterial = mat;
    }
}
