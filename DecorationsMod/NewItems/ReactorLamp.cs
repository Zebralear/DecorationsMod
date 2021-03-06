﻿using DecorationsMod.Controllers;
using SMLHelper;
using SMLHelper.Patchers;
using System.Collections.Generic;
using UnityEngine;

namespace DecorationsMod.NewItems
{
    public class ReactorLamp : DecorationItem
    {
        public ReactorLamp() // Feeds abstract class
        {
            this.ClassID = "ReactorLamp";
            this.ResourcePath = $"{DecorationItem.DefaultResourcePath}{this.ClassID}";

            this.GameObject = AssetsHelper.Assets.LoadAsset<GameObject>("nuclearreactorrod_white");

            this.TechType = TechTypePatcher.AddTechType(this.ClassID,
                                                        LanguageHelper.GetFriendlyWord("ReactorLampName"),
                                                        LanguageHelper.GetFriendlyWord("ReactorLampDescription"),
                                                        true);

            this.IsHabitatBuilder = true;

            this.Recipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[4]
                    {
                        new IngredientHelper(TechType.ComputerChip, 1),
                        new IngredientHelper(TechType.Glass, 1),
                        new IngredientHelper(TechType.Titanium, 1),
                        new IngredientHelper(TechType.Diamond, 1)
                        
                    }),
                _techType = this.TechType
            };
        }

        public override void RegisterItem()
        {
            if (this.IsRegistered == false)
            {
                // Retrieve model node
                GameObject model = this.GameObject.FindChild("model");
                
                // Move model
                model.transform.localPosition = new Vector3(model.transform.localPosition.x, model.transform.localPosition.y, model.transform.localPosition.z);

                // Disable light at start
                var reactorRodLight = this.GameObject.GetComponentInChildren<Light>();
                reactorRodLight.enabled = false;

                // Add prefab identifier
                var prefabId = this.GameObject.AddComponent<PrefabIdentifier>();
                prefabId.ClassId = this.ClassID;

                // Add large world entity
                var lwe = this.GameObject.AddComponent<LargeWorldEntity>();
                lwe.cellLevel = LargeWorldEntity.CellLevel.Near;

                // Add tech tag
                var techTag = this.GameObject.AddComponent<TechTag>();
                techTag.type = this.TechType;

                // Add box collider
                var collider = this.GameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3(0.2f, 0.36f, 0.2f);
                collider.center = new Vector3(collider.center.x, collider.center.y + 0.18f, collider.center.z);

                // Set proper shaders (for crafting animation)
                Shader shader = Shader.Find("MarmosetUBER");
                Texture normalTexture = AssetsHelper.Assets.LoadAsset<Texture>("nuclear_reactor_rod_normal");
                Texture specTexture = AssetsHelper.Assets.LoadAsset<Texture>("nuclear_reactor_rod_spec");
                Texture illumTexture = AssetsHelper.Assets.LoadAsset<Texture>("nuclear_reactor_rod_illum_white");

                List<Renderer> renderers = new List<Renderer>();
                this.GameObject.GetComponentsInChildren<Renderer>(renderers);
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.name.CompareTo("nuclear_reactor_rod_mesh") == 0)
                    {
                        // Associate MarmosetUBER shader
                        renderer.sharedMaterial.shader = shader;
                        
                        // Update normal map
                        renderer.sharedMaterial.SetTexture("_BumpMap", normalTexture);

                        // Update spec map
                        renderer.sharedMaterial.SetTexture("_SpecTex", specTexture);

                        // Update emission map
                        renderer.sharedMaterial.SetTexture("_Illum", illumTexture);

                        // Increase emission map strength
                        renderer.sharedMaterial.SetColor("_GlowColor", new Color(2.5f, 2.5f, 2.5f, 1.0f));
                        renderer.sharedMaterial.SetFloat("_GlowStrength", 1.5f);

                        // Enable emission
                        renderer.sharedMaterial.EnableKeyword("MARMO_NORMALMAP");
                        renderer.sharedMaterial.EnableKeyword("MARMO_EMISSION");
                    }
                }

                // Add sky applier
                var skyapplier = this.GameObject.AddComponent<SkyApplier>();
                skyapplier.renderers = this.GameObject.GetComponentsInChildren<Renderer>();
                skyapplier.anchorSky = Skies.Auto;
                
                // Add contructable
                var constructible = this.GameObject.AddComponent<Lamp_C>();
                constructible.allowedInBase = true;
                constructible.allowedInSub = true;
                constructible.allowedOutside = true;
                constructible.allowedOnCeiling = true;
                constructible.allowedOnGround = true;
                constructible.allowedOnConstructables = true;
                constructible.allowedOnWall = true;
                constructible.controlModelState = true;
                constructible.deconstructionAllowed = true;
                constructible.rotationEnabled = false;
                constructible.model = model;
                constructible.techType = this.TechType;
                constructible.enabled = true;

                // Add constructable bounds
                var bounds = this.GameObject.AddComponent<ConstructableBounds>();
                bounds.bounds.position = new Vector3(bounds.bounds.position.x, bounds.bounds.position.y + 0.003f, bounds.bounds.position.z);

                // Add lamp brightness controler
                var lampBrightness = this.GameObject.AddComponent<ReactorLampBrightness>();

                // Add new TechType to the buildables
                CraftDataPatcher.customBuildables.Add(this.TechType);
                CraftDataPatcher.AddToCustomGroup(TechGroup.ExteriorModules, TechCategory.ExteriorOther, this.TechType);

                // Set the buildable prefab
                CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(this.ClassID, $"{DecorationItem.DefaultResourcePath}{this.ClassID}", this.TechType, this.GetPrefab));
                
                // Set the custom sprite
                CustomSpriteHandler.customSprites.Add(new CustomSprite(this.TechType, AssetsHelper.Assets.LoadAsset<Sprite>("reactorrod_white")));

                // Associate recipe to the new TechType
                CraftDataPatcher.customTechData[this.TechType] = this.Recipe;

                this.IsRegistered = true;
            }
        }

        public override GameObject GetPrefab()
        {
            GameObject prefab = GameObject.Instantiate(this.GameObject);

            prefab.name = this.ClassID;
            // Scale prefab
            prefab.transform.localScale *= 1.5f;

            return prefab;
        }
    }
}
