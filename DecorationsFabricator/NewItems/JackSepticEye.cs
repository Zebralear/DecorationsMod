﻿using SMLHelper;
using SMLHelper.Patchers;
using System.Collections.Generic;
using UnityEngine;

namespace DecorationsFabricator.NewItems
{
    public class JackSepticEye : DecorationItem
    {
        public JackSepticEye()
        {
            this.ClassID = "JackSepticEyeDoll"; // 07a05a2f-de55-4c60-bfda-cedb3ab72b88
            this.ResourcePath = "Submarine/Build/jacksepticeye";
            
            this.GameObject = Resources.Load<GameObject>(this.ResourcePath);

            if (ConfigSwitcher.JackSepticEye_asBuildable)
            {
                this.TechType = TechTypePatcher.AddTechType(this.ClassID,
                                                            LanguageHelper.GetFriendlyWord("JackSepticEyeName"),
                                                            LanguageHelper.GetFriendlyWord("JackSepticEyeDescription"),
                                                            true);
                this.IsHabitatBuilder = true;
            }
            else
            {
                this.TechType = TechType.JackSepticEye;
                KnownTechPatcher.unlockedAtStart.Add(this.TechType);
            }

            this.Recipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[2]
                    {
                        new IngredientHelper(TechType.Titanium, 1),
                        new IngredientHelper(TechType.Glass, 1)
                    }),
                _techType = this.TechType
            };
        }

        public override void RegisterItem()
        {
            if (this.IsRegistered == false)
            {
                if (ConfigSwitcher.JackSepticEye_asBuildable)
                {
                    // Add the new TechType to the buildables
                    CraftDataPatcher.customBuildables.Add(this.TechType);
                    CraftDataPatcher.AddToCustomGroup(TechGroup.Miscellaneous, TechCategory.Misc, this.TechType);
                    CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(this.ClassID, $"{DecorationItem.DefaultResourcePath}{this.ClassID}", this.TechType, this.GetPrefab));
                }
                else
                {
                    // Retrieve collider
                    GameObject model = this.GameObject.FindChild("jacksepticeye");
                    Collider collider = model.GetComponentInChildren<Collider>();

                    // We can pick this item
                    var pickupable = this.GameObject.AddComponent<Pickupable>();
                    pickupable.isPickupable = true;
                    pickupable.randomizeRotationWhenDropped = true;

                    // We can place this item
                    var placeTool = this.GameObject.AddComponent<PlaceTool>();
                    placeTool.allowedInBase = true;
                    placeTool.allowedOnBase = true;
                    placeTool.allowedOnCeiling = false;
                    placeTool.allowedOnConstructable = true;
                    placeTool.allowedOnGround = true;
                    placeTool.allowedOnRigidBody = true;
                    placeTool.allowedOnWalls = false;
                    placeTool.allowedOutside = false;
                    placeTool.rotationEnabled = true;
                    placeTool.enabled = true;
                    placeTool.hasAnimations = false;
                    placeTool.hasBashAnimation = false;
                    placeTool.hasFirstUseAnimation = false;
                    placeTool.mainCollider = collider;
                    placeTool.pickupable = pickupable;

                    // Add the new TechType to the hand-equipments
                    CraftDataPatcher.customEquipmentTypes.Add(this.TechType, EquipmentType.Hand);
                    CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(this.ClassID, this.ResourcePath, this.TechType, this.GetPrefab));
                }
                
                // Set the custom icon
                CustomSpriteHandler.customSprites.Add(new CustomSprite(this.TechType, SpriteManager.Get(TechType.JackSepticEye)));

                if (ConfigSwitcher.JackSepticEye_asBuildable)
                {
                    // Associate recipe to the new TechType
                    CraftDataPatcher.customTechData[this.TechType] = this.Recipe;
                }

                this.IsRegistered = true;
            }
        }

        public override GameObject GetPrefab()
        {
            GameObject prefab = GameObject.Instantiate(this.GameObject);

            prefab.name = this.ClassID;

            if (!ConfigSwitcher.JackSepticEye_asBuildable)
            {
                // Add fabricating animation
                var fabricating = prefab.FindChild("jacksepticeye").AddComponent<VFXFabricating>();
                fabricating.localMinY = -0.1f;
                fabricating.localMaxY = 0.6f;
                fabricating.posOffset = new Vector3(0f, 0f, 0.04f);
                fabricating.eulerOffset = new Vector3(0f, 0f, 0f);
                fabricating.scaleFactor = 1f;
            }
            
            return prefab;
        }
    }
}