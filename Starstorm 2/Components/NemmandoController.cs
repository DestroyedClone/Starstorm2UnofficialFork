﻿using RoR2;
using RoR2.Projectile;
using Starstorm2.Cores;
using UnityEngine;

namespace Starstorm2.Components
{
    public class NemmandoController : MonoBehaviour
    {
        public bool chargingDecisiveStrike = false;
        public bool rolling = false;
        private bool gunHolstered;
        private Transform gun;

        public GameObject cameraCoverInstance;
        
        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.GetComponentInChildren<CharacterModel>();

            Invoke("HolsterGun", 0.5f);

            this.FixProjectile();
        }

        private void LateUpdate()
        {
            // i hate to do it like this but changing the bone name doesn't work for some reason?
            if (this.gunHolstered)
            {
                this.gun.localPosition = new Vector3(-0.002f, -0.0015f, 0);
                this.gun.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }

        private void HolsterGun()
        {
            if (this.characterBody)
            {
                if (this.characterBody.skillLocator.secondary.skillDef.skillNameToken == "NEMMANDO_SECONDARY_CONCUSSION_NAME" && this.characterBody.skillLocator.special.skillNameToken == "NEMMANDO_SPECIAL_EPIC_NAME")
                {
                    this.gunHolstered = true;
                    this.gun = this.childLocator.FindChild("Gun");
                    this.gun.parent = this.childLocator.FindChild("Pelvis");

                    this.characterBody.crosshairPrefab = Utils.LoadCrosshair("SimpleDot");
                }
            }
        }

        public void ActivateThrusters()
        {
            this.childLocator.FindChild("JetMuzzleL").gameObject.SetActive(true);
            this.childLocator.FindChild("JetMuzzleR").gameObject.SetActive(true);
        }

        public void DeactivateThrusters()
        {
            this.childLocator.FindChild("JetMuzzleL").gameObject.SetActive(false);
            this.childLocator.FindChild("JetMuzzleR").gameObject.SetActive(false);
        }

        private void FixProjectile()
        {
            // i need this code to run in start but i don't know how to navigate this plugin so i'm doing the hackiest thing and throwing it in here instead.
            // cry about it.
            // code is fixing phase round lightning interaction
            GameObject swordBeam = Modules.Projectiles.swordBeam;

            var overlapAttack = swordBeam.GetComponent<ProjectileOverlapAttack>();
            if (overlapAttack) overlapAttack.damageCoefficient = 1f;

            if (swordBeam.GetComponent<ProjectileProximityBeamController>()) Starstorm.Destroy(swordBeam.GetComponent<ProjectileProximityBeamController>());
        }

        public void CoverScreen()
        {
            if (this.cameraCoverInstance) return;

            Transform cameraTransform = this.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController.transform;
            this.cameraCoverInstance = GameObject.Instantiate(Modules.Assets.nemmandoCameraCover);

            this.cameraCoverInstance.transform.parent = cameraTransform;
            this.cameraCoverInstance.transform.localPosition = Vector3.zero;
            this.cameraCoverInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);

            this.ApplyOverlays();
        }

        public void UncoverScreen()
        {
            if (this.cameraCoverInstance) GameObject.Destroy(this.cameraCoverInstance);
        }

        private void ApplyOverlays()
        {
            // this is by far the worst way to do this. i promise i will clean it up

            foreach (CharacterModel i in FindObjectsOfType<CharacterModel>())
            {
                if (i)
                {
                    TemporaryOverlay temporaryOverlay = i.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 5f * (Cores.States.Nemmando.ChargedSlashAttack.baseDuration / this.characterBody.attackSpeed);
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Modules.Assets.pureBlackMaterial;
                    temporaryOverlay.AddToCharacerModel(i.transform.GetComponent<CharacterModel>());
                }
            }
        }
    }
}