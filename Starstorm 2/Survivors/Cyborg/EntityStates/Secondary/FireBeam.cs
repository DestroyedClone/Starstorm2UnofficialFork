﻿using RoR2.UI;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Starstorm2.Survivors.Cyborg.Components;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class FireBeam : BaseState
    {
        public static string muzzleString = "Lowerarm.L_end";
        public static float perfectChargeDamageMultiplier = 1.333334f;
        public static float minDamageCoefficient = 2.5f;
        public static float maxDamageCoefficient = 7.5f;
        public static float minForce = 1000f;
        public static float maxForce = 2000f;
        public static float baseDuration = 0.5f;
        public static GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");
        public static GameObject tracerEffectPrefab;
        public static GameObject perfectTracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/TracerHuntressSnipe.prefab").WaitForCompletion();
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightning.prefab").WaitForCompletion();

        public static string attackSoundString = "CyborgPrimary";
        public static string fullSoundString = "CyborgSecondary";
        public static string perfectSoundString = "CyborgUtility";

        public GameObject crosshairPrefab;
        public float charge;
        public bool perfectCharge;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private CyborgChargeComponent chargeComponent;

        private float duration;
        
        public override void OnEnter()
        {
            base.OnEnter();
            chargeComponent = base.GetComponent<CyborgChargeComponent>();
            if (chargeComponent)
            {
                chargeComponent.chargeFraction = this.charge;
                chargeComponent.perfectCharge = this.perfectCharge;
            }
            duration = FireBeam.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Override", "FireM2", "FireArrow.playbackRate", this.duration);
            if (crosshairPrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairPrefab, CrosshairUtils.OverridePriority.Sprint);
            }

            string sound = FireBeam.attackSoundString;
            if (perfectCharge)
            {
                sound = FireBeam.perfectSoundString;
            }
            else if (charge >= 1f)
            {
                sound = FireBeam.fullSoundString;
            }
            Util.PlaySound(sound, base.gameObject);

            if (base.isAuthority)
            {
                float dmg = Mathf.Lerp(FireBeam.minDamageCoefficient, FireBeam.maxDamageCoefficient, charge) * this.damageStat * (perfectCharge ? FireBeam.perfectChargeDamageMultiplier : 1f);
                float force = Mathf.Lerp(FireBeam.minForce, FireBeam.maxForce, charge);

                Ray r = base.GetAimRay();
                BulletAttack bullet = new BulletAttack
                {
                    aimVector = r.direction,
                    origin = r.origin,
                    damage = dmg,
                    damageType = DamageType.Generic,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = force,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 1f,
                    radius = Mathf.Lerp(1f, 2f, charge),
                    weapon = base.gameObject,
                    tracerEffectPrefab = perfectCharge ? FireBeam.perfectTracerEffectPrefab : FireBeam.tracerEffectPrefab,
                    hitEffectPrefab = FireBeam.hitEffectPrefab
                };
                if (perfectCharge) bullet.stopperMask = LayerIndex.world.mask;
                bullet.Fire();
            }
            base.characterBody.AddSpreadBloom(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge > this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (this.chargeComponent)
            {
                chargeComponent.ResetCharge();
            }
            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
