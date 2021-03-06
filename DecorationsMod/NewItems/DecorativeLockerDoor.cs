﻿using DecorationsMod.Controllers;
using SMLHelper;
using SMLHelper.Patchers;
using System.Collections.Generic;
using UnityEngine;

namespace DecorationsMod.NewItems
{
    public class DecorativeLockerDoor : DecorationItem
    {
        private GameObject CargoCrateContainer = null;

        public DecorativeLockerDoor() // Feeds abstract class
        {
            this.ClassID = "DecorativeLockerDoor"; // 078b41f8-968e-4ca3-8a7e-4e3d7d98422c
            this.ResourcePath = "WorldEntities/Doodads/Debris/Wrecks/Decoration/submarine_locker_05";

            this.GameObject = Resources.Load<GameObject>(this.ResourcePath);

            this.TechType = TechTypePatcher.AddTechType(this.ClassID,
                                                        LanguageHelper.GetFriendlyWord("DecorativeLockerName"),
                                                        LanguageHelper.GetFriendlyWord("DecorativeLockerDescription"),
                                                        true);

            this.IsHabitatBuilder = true;

            this.Recipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[1]
                    {
                        new IngredientHelper(TechType.Titanium, 2)
                    }),
                _techType = this.TechType
            };
        }

        public override void RegisterItem()
        {
            if (this.IsRegistered == false)
            {
                this.CargoCrateContainer = Resources.Load<GameObject>("Submarine/Build/Locker");

                // Add model controler
                var decorativeLockerController = this.GameObject.AddComponent<DecorativeLockerController>();

                // Add to the custom buidables
                CraftDataPatcher.customBuildables.Add(this.TechType);
                CraftDataPatcher.AddToCustomGroup(TechGroup.Miscellaneous, TechCategory.Misc, this.TechType);

                // Set the buildable prefab
                CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(this.ClassID, $"{DecorationItem.DefaultResourcePath}{this.ClassID}", this.TechType, this.GetPrefab));

                // Set the custom icon
                CustomSpriteHandler.customSprites.Add(new CustomSprite(this.TechType, new Atlas.Sprite(ImageUtils.LoadTextureFromFile("./QMods/DecorationsMod/Assets/decorativelockerdooricon.png")))); //AssetsHelper.Assets.LoadAsset<Sprite>("decorativelockericon")));

                // Associate recipe to the new TechType
                CraftDataPatcher.customTechData[this.TechType] = this.Recipe;

                this.IsRegistered = true;
            }
        }

        public override GameObject GetPrefab()
        {
            GameObject prefab = GameObject.Instantiate(this.GameObject);
            GameObject container = GameObject.Instantiate(this.CargoCrateContainer);
            GameObject model = prefab.FindChild("submarine_locker_05");

            prefab.name = this.ClassID;

            // Update container renderers
            GameObject cargoCrateModel = container.FindChild("model");
            Renderer[] cargoCrateRenderers = cargoCrateModel.GetComponentsInChildren<Renderer>();
            container.transform.parent = prefab.transform;
            foreach (Renderer rend in cargoCrateRenderers)
            {
                rend.enabled = false;
            }
            container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            container.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            container.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            container.SetActive(true);

            // Update colliders
            GameObject builderTrigger = container.FindChild("Builder Trigger");
            GameObject objectTrigger = container.FindChild("Collider");
            BoxCollider builderCollider = builderTrigger.GetComponent<BoxCollider>();
            builderCollider.isTrigger = false;
            builderCollider.enabled = false;
            BoxCollider objectCollider = objectTrigger.GetComponent<BoxCollider>();
            objectCollider.isTrigger = false;
            objectCollider.enabled = false;

            // Delete constructable bounds
            ConstructableBounds cb = container.GetComponent<ConstructableBounds>();
            GameObject.DestroyImmediate(cb);

            // Update TechTag
            var techTag = prefab.AddComponent<TechTag>();
            techTag.type = this.TechType;

            // Update prefab ID
            var prefabId = prefab.GetComponent<PrefabIdentifier>();
            prefabId.ClassId = this.ClassID;

            // Remove rigid body
            Rigidbody rb = prefab.GetComponent<Rigidbody>();
            GameObject.DestroyImmediate(rb);

            // Add collider
            BoxCollider collider = model.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.4f, 2.0f, 0.5f);
            collider.center = new Vector3(0.0f, 1.0f, 0.0f);

            // Update large world entity
            LargeWorldEntity lwe = prefab.GetComponent<LargeWorldEntity>();
            lwe.cellLevel = LargeWorldEntity.CellLevel.Near;

            // Set as constructible
            Constructable constructible = prefab.AddComponent<Constructable>();
            constructible.techType = this.TechType;
            constructible.allowedOnWall = false;
            constructible.allowedInBase = true;
            constructible.allowedInSub = true;
            constructible.allowedOutside = false;
            constructible.allowedOnCeiling = false;
            constructible.allowedOnGround = true;
            constructible.allowedOnConstructables = false;
            constructible.rotationEnabled = true;
            constructible.deconstructionAllowed = true;
            constructible.controlModelState = true;
            constructible.model = model;

            // Add constructable bounds
            ConstructableBounds bounds = prefab.AddComponent<ConstructableBounds>();

            return prefab;
        }
    }
}
