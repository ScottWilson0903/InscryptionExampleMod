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
            ChangeWolf();
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.ability = (Ability)100;
            info.powerLevel = 0;
            info.triggerText = "New abilities?";
            info.rulebookName = "NEW";
            info.metaCategories = new List<AbilityMetaCategory> {AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular};
            byte[] imgBytes = System.IO.File.ReadAllBytes("BepInEx/plugins/CardLoader/Artwork/new.png");
            Texture2D tex = new Texture2D(2,2);
            tex.LoadImage(imgBytes);
            new NewAbility((Ability)100,info,typeof(NewTestAbility),tex);

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

        private void ChangeWolf(){
          List<Ability> abilities = new List<Ability> {(Ability)100};
          new CustomCard("Wolf", baseAttack:10, abilities:abilities);
        }

        public class NewTestAbility : AbilityBehaviour
      	{
      		// Token: 0x17000294 RID: 660
      		// (get) Token: 0x060013EF RID: 5103 RVA: 0x0004409A File Offset: 0x0004229A
      		public override Ability Ability
      		{
      			get
      			{
      				return (Ability)100;
      			}
      		}

      		// Token: 0x060013F0 RID: 5104 RVA: 0x0000F2FE File Offset: 0x0000D4FE
      		public override bool RespondsToResolveOnBoard()
      		{
      			return true;
      		}

      		// Token: 0x060013F1 RID: 5105 RVA: 0x0004409E File Offset: 0x0004229E
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
