﻿
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Starstorm2.Cores.Skins;

namespace Starstorm2.Cores
{
    public class SkinsCore 
    {
        public SkinsCore() {

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
            //todo: add nem and exe here?
        }

        private static void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig) {
            orig();
            try {
                VanillaSurvivorSkins.RegisterVanillaSurvivorSkins();
            }
            catch (Exception ex) {
                LogCore.LogError("error registering vanilla survivor skins\n" + ex);
            }
        }

        public static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root, UnlockableDef unlockDef) {
            LoadoutAPI.SkinDefInfo skinDefInfo = new LoadoutAPI.SkinDefInfo {
                BaseSkins = Array.Empty<SkinDef>(),
                GameObjectActivations = new SkinDef.GameObjectActivation[0],
                Icon = skinIcon,
                MeshReplacements = new SkinDef.MeshReplacement[0],
                MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
                Name = skinName,
                NameToken = skinName,
                ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
                RendererInfos = rendererInfos,
                RootObject = root,
                UnlockableDef = unlockDef
            };

            SkinDef skin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);

            return skin;
        }

        public static SkinDef.MeshReplacement[] CreateMeshReplacements(CharacterModel.RendererInfo[] rendererInfos, params Mesh[] orderedMeshes) {
            List<SkinDef.MeshReplacement> replacements = new List<SkinDef.MeshReplacement>();

            for (int i = 0; i < orderedMeshes.Length; i++) {
                if (orderedMeshes[i] == null)
                    continue;

                replacements.Add(new SkinDef.MeshReplacement {
                    mesh = orderedMeshes[i],
                    renderer = rendererInfos[i].renderer
                });
            }
            return replacements.ToArray();
        }

        /// <summary>
        /// create an array of all gameobjects that are activated/deactivated by skins, then for each skin pass in the specific objects that will be active
        /// </summary>
        /// <param name="allObjects">array of all gameobjects that are activated/deactivated by skins</param>
        /// <param name="activatedObjects">specific objects that will be active</param>
        /// <returns></returns>
        public static SkinDef.GameObjectActivation[] createGameObjectActivations(GameObject[] allObjects, params GameObject[] activatedObjects) {

            List<SkinDef.GameObjectActivation> GameObjectActivations = new List<SkinDef.GameObjectActivation>();

            for (int i = 0; i < allObjects.Length; i++) {

                bool activate = activatedObjects.Contains(allObjects[i]);

                GameObjectActivations.Add(new SkinDef.GameObjectActivation {
                    gameObject = allObjects[i],
                    shouldActivate = activate
                });
            }

            return GameObjectActivations.ToArray();
        }
    }
}
