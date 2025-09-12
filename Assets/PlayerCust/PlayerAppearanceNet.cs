// PlayerAppearanceNet.cs
using UnityEngine;
using Fusion;

public class PlayerAppearanceNet : NetworkBehaviour {
  [SerializeField] ShroomCustomizerMPB avatar; // assign the in-game model’s customizer

  void Awake() {
    if (!avatar) avatar = GetComponentInChildren<ShroomCustomizerMPB>(true);
  }

  public override void Spawned() {
    // Only the local owner sends their look when they spawn
    if (Object.HasInputAuthority) {
      var saved = CharacterSelection.Data;     // what you set in the customization scene
      RpcApplyLook( ShroomNet.ToNet(saved) );  // broadcast to everyone (including self)
    }
  }

  [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
  private void RpcApplyLook(ShroomNetData data) {
    if (avatar) avatar.ApplyData( ShroomNet.ToLocal(data) );
  }

  // Optional: call this again whenever the local player changes their look mid-game
  public void SendCurrentLook(ShroomData d) {
    if (Object.HasInputAuthority) RpcApplyLook( ShroomNet.ToNet(d) );
  }
}
