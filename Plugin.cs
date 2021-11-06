using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;

namespace CardLoaderMod
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "cyantist.inscryption.customcardexample";
        private const string PluginName = "CustomCardModExample";
        private const string PluginVersion = "1.0.0.0";

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");

            AddBears();
            NewAbility ability1 = AddAbility();
            ChangeWolf(ability1.ability);

        }

        private void AddBears(){
            List<CardMetaCategory> metaCategories = new List<CardMetaCategory>();
            metaCategories.Add(CardMetaCategory.ChoiceNode);
            metaCategories.Add(CardMetaCategory.Rare);
            List<CardAppearanceBehaviour.Appearance> appearanceBehaviour = new List<CardAppearanceBehaviour.Appearance>();
            appearanceBehaviour.Add(CardAppearanceBehaviour.Appearance.RareCardBackground);
            byte[] imgBytes = System.IO.File.ReadAllBytes("BepInEx/plugins/CardLoader/Artwork/eightfuckingbears.png");
            Texture2D tex = new Texture2D(2,2);
            tex.LoadImage(imgBytes);
            new NewCard("Eight_Bears", metaCategories, CardComplexity.Simple, CardTemple.Nature,"8 fucking bears!",32,48,description:"Kill this abomination please",cost:3,appearanceBehaviour:appearanceBehaviour, tex:tex);
        }

        private NewAbility AddAbility()
    		{
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.triggerText = "New abilities?";
            info.rulebookName = "NEW";
            info.rulebookDescription = "New abilities? :O";
            info.metaCategories = new List<AbilityMetaCategory> {AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular};
            byte[] imgBytes = System.IO.File.ReadAllBytes("BepInEx/plugins/CardLoader/Artwork/new.png");
            Texture2D tex = new Texture2D(2,2);
            tex.LoadImage(imgBytes);
            NewAbility ability = new NewAbility(info,typeof(NewTestAbility),tex);
            NewTestAbility.ability = ability.ability;
            return ability;
    		}

        private void ChangeWolf(Ability ability){
          List<Ability> abilities = new List<Ability> {ability};
          new CustomCard("Wolf", baseAttack:10, abilities:abilities);
        }

        public class NewTestAbility : AbilityBehaviour
      	{
      		public override Ability Ability
      		{
      			get
      			{
      				return ability;
      			}
      		}

          public static Ability ability;

      		public override bool RespondsToResolveOnBoard()
      		{
      			return true;
      		}

      		public override IEnumerator OnResolveOnBoard()
      		{
      			yield return base.PreSuccessfulTriggerSequence();
      			yield return new WaitForSeconds(0.2f);
      			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
      			yield return new WaitForSeconds(0.25f);
      			if (RunState.Run.consumables.Count < 3)
      			{
      				RunState.Run.consumables.Add("PiggyBank");
      				Singleton<ItemsManager>.Instance.UpdateItems(false);
      			}
      			else
      			{
      				base.Card.Anim.StrongNegationEffect();
      				yield return new WaitForSeconds(0.2f);
      				Singleton<ItemsManager>.Instance.ShakeConsumableSlots(0f);
      			}
      			yield return new WaitForSeconds(0.2f);
      			yield return base.LearnAbility(0f);
      			yield break;
      		}
      	}
    }

}
