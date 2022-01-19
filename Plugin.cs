using BepInEx;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DiskCardGame;
using UnityEngine;
using APIPlugin;

namespace CardLoaderMod
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "cyantist.inscryption.examplemod";
        private const string PluginName = "ExampleMod";
        private const string PluginVersion = "1.6.0.0";

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");

            AddBears();
            AddAbility();
            ChangeWolf();
            AddRegion();
        }

        private void AddBears()
        {
            // metaCategories determine the card pools
            List<CardMetaCategory> metaCategories = new List<CardMetaCategory>
            {
                CardMetaCategory.ChoiceNode,
                CardMetaCategory.Rare
            };

            List<CardAppearanceBehaviour.Appearance> appearanceBehaviour = new List<CardAppearanceBehaviour.Appearance>
            {
                CardAppearanceBehaviour.Appearance.RareCardBackground,
                CardAppearanceBehaviour.Appearance.AnimatedPortrait // Optional. If not provided for a talking card, it will be added automatically
            };

            List<SpecialTriggeredAbility> specialAbilities = new List<SpecialTriggeredAbility>
            {
                SpecialTriggeredAbility.TalkingCardChooser // Required for talking cards!
            };

            // Load the image into a Texture2D object
            byte[] imgBytes = File.ReadAllBytes(Path.Combine(this.Info.Location.Replace("ExampleMod.dll", ""),"Artwork/eightfuckingbears.png"));
            Texture2D tex = new Texture2D(2,2);
            tex.LoadImage(imgBytes);

            // Add the card
            NewCard.Add("Eight_Bears", "8 fucking bears!", 32, 48, metaCategories, CardComplexity.Simple, CardTemple.Nature, description: "Kill this abomination please",
                        bloodCost: 3, appearanceBehaviour: appearanceBehaviour, defaultTex: tex, specialAbilities: specialAbilities);

            // Add the talking card behaviour. The name must be the same as the card name!
            NewTalkingCard.Add<EightBearsTalkingCard>("Eight_Bears", EightBearsTalkingCard.GetDictionary());
        }

        private NewAbility AddAbility()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.powerLevel = 0;
            info.rulebookName = "Example Ability";
            info.rulebookDescription = "Example ability which adds a PiggyBank!";

            // metaCategories determine the pools
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };

            // Create the 'learned' dialogue
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line();
            line.text = "New abilities? I didn't authorise homebrew!";
            lines.Add(line);
            info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            // Load the image into a Texture2D object
            byte[] imgBytes = File.ReadAllBytes(Path.Combine(this.Info.Location.Replace("ExampleMod.dll", ""), "Artwork/new.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);

            NewAbility ability = new NewAbility(info, typeof(NewTestAbility), tex, AbilityIdentifier.GetID(PluginGuid, info.rulebookName));
            NewTestAbility.ability = ability.ability;
            return ability;
        }

        private void ChangeWolf()
        {
            List<Ability> abilities = new List<Ability> { NewTestAbility.ability };
            new CustomCard("Wolf") { baseAttack = 10, abilities = abilities };
        }

        private void AddRegion()
        {
            // Load the default first region (NOTE: For Kaycee's Mod, use 'regions[0]')
            RegionData regionData = ResourceBank.Get<RegionProgression>("Data/Map/RegionProgression").regions[0][0];

            regionData.name = "Example_Region";

            // Clear default encounters
            regionData.encounters.Clear();

            // Modify the colors to be red/green
            regionData.boardLightColor = new Color(1f, 0f, 0f);
            regionData.cardsLightColor = new Color(0f, 1f, 0f);

            // Adds the region to tier 0
            new NewRegion(regionData, 0);


            // Add eight turns with four 'Eight_Bears' cards
            List<List<EncounterBlueprintData.CardBlueprint>> turns = new List<List<EncounterBlueprintData.CardBlueprint>>();
            for (int i = 0; i < 8; i++)
            {
                List<EncounterBlueprintData.CardBlueprint> turn = new List<EncounterBlueprintData.CardBlueprint>();
                for (int j = 0; j < 4; j++)
                {
                    turn.Add(new EncounterBlueprintData.CardBlueprint() {
                        // We have to get it directly from the new card list because it is not yet loaded into data - will be fixed in the future
                        card = NewCard.cards.Find(card => card.name == "Eight_Bears")
                    });
                }
                turns.Add(turn);
            }
            EncounterBlueprintData bearsBlueprint = ScriptableObject.CreateInstance<EncounterBlueprintData>();

            // Which totem to use for totem battles (Bears are tribeless, so using squirrel here)
            bearsBlueprint.dominantTribes = new List<Tribe>() { Tribe.Squirrel };
            bearsBlueprint.turns = turns;

            // Add the encounter as 'regular' encounter of the example region
            // Usually it's better to add it to the encounters field instead, if you create the region in the same mod
            new NewEncounter("Bear_Spam", bearsBlueprint, "Example_Region", true, false);
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
                // Add piggy bank, only if there is room for it
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

    public class EightBearsTalkingCard : PaperTalkingCard
    {
        // Static method for easy access
        public static DialogueEvent.Speaker Speaker => (DialogueEvent.Speaker) 100;

        // Only important for multi-speaker dialogs
        public override DialogueEvent.Speaker SpeakerType => Speaker;

        // IDs should point to dictionary entries in GetDictionary().
        // Required:

        public override string OnDrawnDialogueId => "TalkingEightBearsDrawn";

        public override string OnDrawnFallbackDialogueId => "TalkingEightBearsDrawn2";

        public override string OnPlayFromHandDialogueId => "TalkingEightBearsPlayed";

        public override string OnAttackedDialogueId => "TalkingEightBearsAttacked";

        public override string OnBecomeSelectablePositiveDialogueId => "TalkingEightBearsPositiveSelectable";

        public override string OnBecomeSelectableNegativeDialogueId => "TalkingEightBearsNegativeSelectable";

        public override Dictionary<Opponent.Type, string> OnDrawnSpecialOpponentDialogueIds => new Dictionary<Opponent.Type, string>();

        // Optional:

        public override string OnSacrificedDialogueId => "TalkingEightBearsSacrificed";

        public override string OnSelectedForCardMergeDialogueId => "TalkingEightBearsMerged";

        public override string OnSelectedForCardRemoveDialogueId => "TalkingEightBearsRemoved";

        public override string OnSelectedForDeckTrialDialogueId => "TalkingEightBearsDeckTrial";

        public override string OnDiscoveredInExplorationDialogueId => "TalkingEightBearsDiscovered";

        public static Dictionary<string, DialogueEvent> GetDictionary()
        {
            Dictionary<string, DialogueEvent> events = new Dictionary<string, DialogueEvent>();
            events.Add("TalkingEightBearsDrawn", new DialogueEvent()
            {
                id = "TalkingEightBearsDrawn",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsDrawn2", new DialogueEvent()
            {
                id = "TalkingEightBearsDrawn2",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsPlayed", new DialogueEvent()
            {
                id = "TalkingEightBearsPlayed",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsAttacked", new DialogueEvent()
            {
                id = "TalkingEightBearsAttacked",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsPositiveSelectable", new DialogueEvent()
            {
                id = "TalkingEightBearsPositiveSelectable",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsNegativeSelectable", new DialogueEvent()
            {
                id = "TalkingEightBearsNegativeSelectable",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsSacrificed", new DialogueEvent()
            {
                id = "TalkingEightBearsSacrificed",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsMerged", new DialogueEvent()
            {
                id = "TalkingEightBearsMerged",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsRemoved", new DialogueEvent()
            {
                id = "TalkingEightBearsRemoved",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsDeckTrial", new DialogueEvent()
            {
                id = "TalkingEightBearsDeckTrial",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            events.Add("TalkingEightBearsDiscovered", new DialogueEvent()
            {
                id = "TalkingEightBearsDiscovered",
                speakers = new List<DialogueEvent.Speaker>() { Speaker },
                mainLines = new DialogueEvent.LineSet()
                {
                    lines = new List<DialogueEvent.Line>()
                    {
                        new DialogueEvent.Line { text = "*Bear Noises*" }
                    }
                }
            });
            return events;
        }
    }
}
