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
using CardLoaderPlugin;

namespace CardLoaderMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("cyantist.inscryption.cardloader", BepInDependency.DependencyFlags.HardDependency)]
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
            new CustomCard("Wolf", baseAttack:10);
        }
    }
}
