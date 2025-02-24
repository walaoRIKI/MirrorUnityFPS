using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class HeldBase
{
    [Header("Effect")]
    public EffectPlayer fireEffect;

    /// <summary>
    /// To send RPC to client to play this effect
    /// </summary>
    [Server]
    public void ServerPlayerEffect(EffectType effectType) {

        PlayerEffect(effectType);

    }

    [ClientRpc]
    public void PlayerEffect(EffectType effectType) {

        EffectPlayer targetEffect;

        switch (effectType) {
            case EffectType.Fire:
                targetEffect = fireEffect;
                break;
            default:
                targetEffect = null;
                break;
        }

        if(targetEffect != null) {

            targetEffect.PlayerEffect();

        }

    }

    [Server]
    protected void PlayHitDecal(Collider hitCollider, Vector3 pos, Quaternion rot) {

        DecalSpawner.instance.SpawnDecal(hitCollider, pos, rot);

    }
}
