using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.DirectorAPI;
using static R2API.DirectorAPI.Helpers;
using static RoR2.DccsPool;

namespace RecoveredAndReformed
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency("com.groovesalad.GrooveSaladSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.plasmacore.PlasmaCoreSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.ClayMen", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.ArchaicWisp", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AncientWisp", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AccurateEnemies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("PlasmaCore.ForgottenRelics", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.Direseeker", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "prodzpod";
        public const string PluginName = "RecoveredAndReformed";
        public const string PluginVersion = "1.0.2";
        public static ManualLogSource Log;
        public static PluginInfo pluginInfo;
        public static Harmony Harmony;
        public static ConfigFile Config;
        public static ConfigEntry<string> MajorConstructSpawn;
        public static ConfigEntry<string> Assassin2Spawn;
        public static ConfigEntry<string> BellTowerSpawn;
        public static ConfigEntry<int> MajorConstructMinStage;
        public static ConfigEntry<int> Assassin2MinStage;
        public static ConfigEntry<int> BellTowerMinStage;
        public static ConfigEntry<bool> EliteMagmaWorm;
        public static ConfigEntry<bool> EliteOverloadingWorm;
        public static ConfigEntry<bool> EliteAWU;
        public static ConfigEntry<bool> EliteAurellionite;
        private ConfigEntry<bool> EliteArtifactShell;
        public static ConfigEntry<bool> EliteDireseeker;
        public static ConfigEntry<float> MajorConstructHealth;
        public static ConfigEntry<float> MajorConstructHealthStack;
        public static ConfigEntry<float> MajorConstructDamage;
        public static ConfigEntry<float> MajorConstructDamageStack;
        public static ConfigEntry<float> MajorConstructAttackSpeed;
        public static ConfigEntry<float> MajorConstructArmor;
        public static ConfigEntry<int> MajorConstructCost;
        public static ConfigEntry<float> Assassin2Health;
        public static ConfigEntry<float> Assassin2HealthStack;
        public static ConfigEntry<float> Assassin2Damage;
        public static ConfigEntry<float> Assassin2DamageStack;
        public static ConfigEntry<float> Assassin2AttackSpeed;
        public static ConfigEntry<float> Assassin2Speed;
        public static ConfigEntry<float> Assassin2Armor;
        public static ConfigEntry<int> Assassin2Cost;
        public static ConfigEntry<float> MagmaWormHealth;
        public static ConfigEntry<float> MagmaWormHealthStack;
        public static ConfigEntry<float> MagmaWormDamage;
        public static ConfigEntry<float> MagmaWormDamageStack;
        public static ConfigEntry<float> MagmaWormAttackSpeed;
        public static ConfigEntry<float> MagmaWormSpeed;
        public static ConfigEntry<float> MagmaWormArmor;
        public static ConfigEntry<int> MagmaWormCost;
        public static ConfigEntry<float> OverloadingWormHealth;
        public static ConfigEntry<float> OverloadingWormHealthStack;
        public static ConfigEntry<float> OverloadingWormDamage;
        public static ConfigEntry<float> OverloadingWormDamageStack;
        public static ConfigEntry<float> OverloadingWormAttackSpeed;
        public static ConfigEntry<float> OverloadingWormSpeed;
        public static ConfigEntry<float> OverloadingWormArmor;
        public static ConfigEntry<int> OverloadingWormCost;
        public static ConfigEntry<float> BellTowerHealth;
        public static ConfigEntry<float> BellTowerHealthStack;
        public static ConfigEntry<float> BellTowerDamage;
        public static ConfigEntry<float> BellTowerDamageStack;
        public static ConfigEntry<float> BellTowerAttackSpeed;
        public static ConfigEntry<float> BellTowerArmor;
        public static ConfigEntry<int> BellTowerCost;
        public static ConfigEntry<float> AcidLarvaHealth;
        public static ConfigEntry<float> AcidLarvaHealthStack;
        public static ConfigEntry<float> AcidLarvaDamage;
        public static ConfigEntry<float> AcidLarvaDamageStack;
        public static ConfigEntry<float> AcidLarvaAttackSpeed;
        public static ConfigEntry<float> AcidLarvaSpeed;
        public static ConfigEntry<float> AcidLarvaArmor;
        public static ConfigEntry<int> AcidLarvaCost;
        public static ConfigEntry<float> AWUHealth;
        public static ConfigEntry<float> AWUHealthStack;
        public static ConfigEntry<float> AWUDamage;
        public static ConfigEntry<float> AWUDamageStack;
        public static ConfigEntry<float> AWUAttackSpeed;
        public static ConfigEntry<float> AWUSpeed;
        public static ConfigEntry<float> AWUArmor;
        public static ConfigEntry<float> AurellioniteHealth;
        public static ConfigEntry<float> AurellioniteHealthStack;
        public static ConfigEntry<float> AurellioniteDamage;
        public static ConfigEntry<float> AurellioniteDamageStack;
        public static ConfigEntry<float> AurellioniteAttackSpeed;
        public static ConfigEntry<float> AurellioniteSpeed;
        public static ConfigEntry<float> AurellioniteArmor;
        public static ConfigEntry<float> ArtifactShellHealth;
        public static ConfigEntry<float> ArtifactShellHealthStack;
        public static ConfigEntry<float> ArtifactShellDamage;
        public static ConfigEntry<float> ArtifactShellDamageStack;
        public static ConfigEntry<float> ArtifactShellAttackSpeed;
        public static ConfigEntry<float> ArtifactShellArmor;
        public static ConfigEntry<float> DireseekerHealth;
        public static ConfigEntry<float> DireseekerHealthStack;
        public static ConfigEntry<float> DireseekerDamage;
        public static ConfigEntry<float> DireseekerDamageStack;
        public static ConfigEntry<float> DireseekerAttackSpeed;
        public static ConfigEntry<float> DireseekerSpeed;
        public static ConfigEntry<float> DireseekerArmor;
        public static ConfigEntry<float> MajorConstructDeathTimer;
        public static ConfigEntry<int> MajorConstructSpawnAmount;
        public static ConfigEntry<bool> MajorConstructSpawnSigmaInstead;
        public static ConfigEntry<float> MajorConstructBeamDistance;
        public static ConfigEntry<float> MajorConstructAimSpeed;
        public static ConfigEntry<float> Assassin2InvisDuration;
        public static ConfigEntry<float> Assassin2InvisEndlag;
        public static ConfigEntry<float> Assassin2DashDuration;
        public static ConfigEntry<float> Assassin2DashSpeed;
        public static ConfigEntry<float> Assassin2SlashDuration;
        public static ConfigEntry<float> Assassin2SlashDamage;
        public static ConfigEntry<float> Assassin2SlashKnockback;
        public static ConfigEntry<float> Assassin2SlashReach;
        public static ConfigEntry<bool> Assassin2Slash1;
        public static ConfigEntry<bool> Assassin2Slash2;
        public static ConfigEntry<float> Assassin2SlashExpose;
        public static ConfigEntry<float> Assassin2SlashSize;
        public static ConfigEntry<bool> Assassin2SlashMultihit;
        public static ConfigEntry<float> Assassin2ShurikenSize;
        public static ConfigEntry<bool> Assassin2Accurate;
        public static ConfigEntry<bool> Assassin2AccurateLoop;
        public static ConfigEntry<bool> EnableFamilyChange;
        public static ConfigEntry<string> TarFamily;
        public static ConfigEntry<string> AltWispFamily;
        public static ConfigEntry<string> SolusFamily;
        public static ConfigEntry<string> BlindFamily;
        public static ConfigEntry<string> BellFamily;
        public static ConfigEntry<bool> RemoveLarvaFamily;
        public static ConfigEntry<float> LarvaDetonateSelfDamage;
        public static ConfigEntry<float> AWUShieldTime;
        public static ConfigEntry<bool> DireseekerDropAtPlace;
        public static ConfigEntry<float> FamilyEventChance;
        public static ExpansionDef DLC1 = null;
        public static List<DirectorCardCategorySelection> customFamilies = new();

        public void Awake()
        {
            pluginInfo = Info;
            Log = Logger;
            Harmony = new Harmony(PluginGUID);
            Config = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, PluginGUID + ".cfg"), true);

            MajorConstructSpawn = Config.Bind("Spawns", "Iota Construct Spawn Scenes", "skymeadow, itskymeadow, sulfurpools, itgolemplains, golemplains, golemplains2, forgottenhaven, FBLScene", "List of scene names, separated by comma. By default, where Xi Construct spawns.");
            Assassin2Spawn = Config.Bind("Spawns", "Assassin Spawn Scenes", "blackbeach, blackbeach2, rootjungle, arena, shipgraveyard, wispgraveyard, itancientloft, ancientloft", "List of scene names, separated by comma.");
            BellTowerSpawn = Config.Bind("Spawns", "Bell Tower Spawn Scenes", "itdampcave, dampcavesimple, shipgraveyard, itskymeadow, skymeadow, foggyswamp, FBLScene", "List of scene names, separated by comma. Temporary config. Make sure to set FR Config > Disable Bell Tower = false before running.");
            MajorConstructMinStage = Config.Bind("Spawns", "Iota Construct Minimum Stage", 4, "Starting stage.");
            Assassin2MinStage = Config.Bind("Spawns", "Assassin Minimum Stage", 3, "Starting stage.");
            BellTowerMinStage = Config.Bind("Spawns", "Bell Tower Minimum Stage", 0, "Starting stage.");
            EliteMagmaWorm = Config.Bind("Spawns", "Enable Elite Magma Worms", false, "If true, allows spawning of elite magma worms.");
            EliteOverloadingWorm = Config.Bind("Spawns", "Enable Elite Overloading Worms", false, "If true, allows spawning of elite overloading worms.");
            EliteAWU = Config.Bind("Spawns", "Enable Elite Alloy Worship Unit", false, "If true, allows spawning of elite AWUs.");
            EliteAurellionite = Config.Bind("Spawns", "Enable Elite Aurellionite", false, "If true, allows spawning of elite aurellionites.");
            EliteArtifactShell = Config.Bind("Spawns", "Enable Elite Artifact Reliquary", false, "If true, allows spawning of elite artifact reliquaries.");
            EliteDireseeker = Config.Bind("Spawns", "Enable Elite Direseeker", false, "If true, allows spawning of elite direseekers.");

            MajorConstructHealth = Config.Bind("Basic Stats", "Iota Construct Base Health", 2100f, "");
            MajorConstructHealthStack = Config.Bind("Basic Stats", "Iota Construct Health Increase Per Level", 630f, "");
            MajorConstructDamage = Config.Bind("Basic Stats", "Iota Construct Base Damage", 25f, "");
            MajorConstructDamageStack = Config.Bind("Basic Stats", "Iota Construct Damage Increase Per Level", 5f, "");
            MajorConstructAttackSpeed = Config.Bind("Basic Stats", "Iota Construct Attack Speed", 1f, "");
            MajorConstructArmor = Config.Bind("Basic Stats", "Iota Construct Armor", 15f, "");
            MajorConstructCost = Config.Bind("Basic Stats", "Iota Construct Director Cost", 800, "");
            Assassin2Health = Config.Bind("Basic Stats", "Assassin Base Health", 120f, "");
            Assassin2HealthStack = Config.Bind("Basic Stats", "Assassin Health Increase Per Level", 36f, "");
            Assassin2Damage = Config.Bind("Basic Stats", "Assassin Base Damage", 10f, "");
            Assassin2DamageStack = Config.Bind("Basic Stats", "Assassin Damage Increase Per Level", 2f, "");
            Assassin2AttackSpeed = Config.Bind("Basic Stats", "Assassin Attack Speed", 1.2f, "");
            Assassin2Speed = Config.Bind("Basic Stats", "Assassin Speed", 12f, "");
            Assassin2Armor = Config.Bind("Basic Stats", "Assassin Armor", 0f, "");
            Assassin2Cost = Config.Bind("Basic Stats", "Assassin Director Cost", 28, "Default: 10.");
            MagmaWormHealth = Config.Bind("Basic Stats", "Magma Worm Base Health", 2400f, "");
            MagmaWormHealthStack = Config.Bind("Basic Stats", "Magma Worm Health Increase Per Level", 720f, "");
            MagmaWormDamage = Config.Bind("Basic Stats", "Magma Worm Base Damage", 10f, "");
            MagmaWormDamageStack = Config.Bind("Basic Stats", "Magma Worm Damage Increase Per Level", 2f, "");
            MagmaWormAttackSpeed = Config.Bind("Basic Stats", "Magma Worm Attack Speed", 1f, "");
            MagmaWormSpeed = Config.Bind("Basic Stats", "Magma Worm Speed", 20f, "");
            MagmaWormArmor = Config.Bind("Basic Stats", "Magma Worm Armor", 15f, "");
            MagmaWormCost = Config.Bind("Basic Stats", "Magma Worm Director Cost", 800, "");
            OverloadingWormHealth = Config.Bind("Basic Stats", "Overloading Worm Base Health", 12000f, "");
            OverloadingWormHealthStack = Config.Bind("Basic Stats", "Overloading Worm Health Increase Per Level", 3600f, "");
            OverloadingWormDamage = Config.Bind("Basic Stats", "Overloading Worm Base Damage", 50f, "");
            OverloadingWormDamageStack = Config.Bind("Basic Stats", "Overloading Worm Damage Increase Per Level", 10f, "");
            OverloadingWormAttackSpeed = Config.Bind("Basic Stats", "Overloading Worm Attack Speed", 1f, "");
            OverloadingWormSpeed = Config.Bind("Basic Stats", "Overloading Worm Speed", 20f, "");
            OverloadingWormArmor = Config.Bind("Basic Stats", "Overloading Worm Armor", 15f, "");
            OverloadingWormCost = Config.Bind("Basic Stats", "Overloading Worm Director Cost", 4000, "");
            BellTowerHealth = Config.Bind("Basic Stats", "Bell Tower Base Health", 3625f, "");
            BellTowerHealthStack = Config.Bind("Basic Stats", "Bell Tower Health Increase Per Level", 1088f, "");
            BellTowerDamage = Config.Bind("Basic Stats", "Bell Tower Base Damage", 15f, "");
            BellTowerDamageStack = Config.Bind("Basic Stats", "Bell Tower Damage Increase Per Level", 3f, "");
            BellTowerAttackSpeed = Config.Bind("Basic Stats", "Bell Tower Attack Speed", 1f, "");
            BellTowerArmor = Config.Bind("Basic Stats", "Bell Tower Armor", 15f, "");
            BellTowerCost = Config.Bind("Basic Stats", "Bell Tower Director Cost", 750, "");
            AcidLarvaHealth = Config.Bind("Basic Stats", "Acid Larva Base Health", 45f, "");
            AcidLarvaHealthStack = Config.Bind("Basic Stats", "Acid Larva Health Increase Per Level", 14f, "");
            AcidLarvaDamage = Config.Bind("Basic Stats", "Acid Larva Base Damage", 22f, "Default: 11.");
            AcidLarvaDamageStack = Config.Bind("Basic Stats", "Acid Larva Damage Increase Per Level", 4.4f, "Default: 2.2.");
            AcidLarvaAttackSpeed = Config.Bind("Basic Stats", "Acid Larva Attack Speed", 1f, "");
            AcidLarvaSpeed = Config.Bind("Basic Stats", "Acid Larva Speed", 1f, "");
            AcidLarvaArmor = Config.Bind("Basic Stats", "Acid Larva Armor", 0f, "");
            AcidLarvaCost = Config.Bind("Basic Stats", "Acid Larva Director Cost", 25, "");
            AWUHealth = Config.Bind("Basic Stats", "Alloy Worship Unit Base Health", 2500f, "");
            AWUHealthStack = Config.Bind("Basic Stats", "Alloy Worship Unit Health Increase Per Level", 750f, "");
            AWUDamage = Config.Bind("Basic Stats", "Alloy Worship Unit Base Damage", 15f, "");
            AWUDamageStack = Config.Bind("Basic Stats", "Alloy Worship Unit Damage Increase Per Level", 3f, "");
            AWUAttackSpeed = Config.Bind("Basic Stats", "Alloy Worship Unit Attack Speed", 1f, "");
            AWUSpeed = Config.Bind("Basic Stats", "Alloy Worship Unit Speed", 7f, "");
            AWUArmor = Config.Bind("Basic Stats", "Alloy Worship Unit Armor", 30f, "");
            AurellioniteHealth = Config.Bind("Basic Stats", "Aurellionite Base Health", 2100f, "");
            AurellioniteHealthStack = Config.Bind("Basic Stats", "Aurellionite Health Increase Per Level", 630f, "");
            AurellioniteDamage = Config.Bind("Basic Stats", "Aurellionite Base Damage", 40f, "");
            AurellioniteDamageStack = Config.Bind("Basic Stats", "Aurellionite Damage Increase Per Level", 8f, "");
            AurellioniteAttackSpeed = Config.Bind("Basic Stats", "Aurellionite Attack Speed", 1f, "");
            AurellioniteSpeed = Config.Bind("Basic Stats", "Aurellionite Speed", 5f, "");
            AurellioniteArmor = Config.Bind("Basic Stats", "Aurellionite Armor", 20f, "");
            ArtifactShellHealth = Config.Bind("Basic Stats", "Artifact Reliquary Base Health", 100000f, "");
            ArtifactShellHealthStack = Config.Bind("Basic Stats", "Artifact Reliquary Health Increase Per Level", 30000f, "");
            ArtifactShellDamage = Config.Bind("Basic Stats", "Artifact Reliquary Base Damage", 30f, "Default: 10.");
            ArtifactShellDamageStack = Config.Bind("Basic Stats", "Artifact Reliquary Damage Increase Per Level", 6f, "Default: 2.");
            ArtifactShellAttackSpeed = Config.Bind("Basic Stats", "Artifact Reliquary Attack Speed", 1f, "");
            ArtifactShellArmor = Config.Bind("Basic Stats", "Artifact Reliquary Armor", 100000f, "");
            DireseekerHealth = Config.Bind("Basic Stats", "Direseeker Base Health", 2800f, "");
            DireseekerHealthStack = Config.Bind("Basic Stats", "Direseeker Health Increase Per Level", 840f, "");
            DireseekerDamage = Config.Bind("Basic Stats", "Direseeker Base Damage", 20f, "");
            DireseekerDamageStack = Config.Bind("Basic Stats", "Direseeker Damage Increase Per Level", 4f, "");
            DireseekerAttackSpeed = Config.Bind("Basic Stats", "Direseeker Attack Speed", 1f, "");
            DireseekerSpeed = Config.Bind("Basic Stats", "Direseeker Speed", 11f, "");
            DireseekerArmor = Config.Bind("Basic Stats", "Direseeker Armor", 0f, "");

            MajorConstructDeathTimer = Config.Bind("Iota Construct", "Death Timer", 5f, "Amount of seconds to show death animation for");
            MajorConstructSpawnAmount = Config.Bind("Iota Construct", "Spawn Amount", 3, "Amount of enemies to spawn in pillar raise phase");
            MajorConstructSpawnSigmaInstead = Config.Bind("Iota Construct", "Spawn Sigma Constructs", true, "Spikestrip Compat");
            MajorConstructBeamDistance = Config.Bind("Iota Construct", "Beam Distance", 150f, "Used to be 999");
            MajorConstructAimSpeed = Config.Bind("Iota Construct", "Aim Speed", 10f, "Strafe around!");

            Assassin2InvisDuration = Config.Bind("Assassin", "Invisible Duration", 2f, "Default: 5");
            Assassin2InvisEndlag = Config.Bind("Assassin", "Invisible End Lag", 3f, "Default: 10");
            Assassin2DashDuration = Config.Bind("Assassin", "Dash Duration", 2.5f, "Default: 2.5");
            Assassin2DashSpeed = Config.Bind("Assassin", "Dash Speed", 3f, "Default: 4");
            Assassin2SlashDuration = Config.Bind("Assassin", "Slash Duration", 1.5f, "Default: 1.667");
            Assassin2SlashDamage = Config.Bind("Assassin", "Slash Damage", 1f, "Default: 4");
            Assassin2SlashKnockback = Config.Bind("Assassin", "Slash Knockback", 16f, "Default: 16");
            Assassin2SlashReach = Config.Bind("Assassin", "Slash Reach", 25f, "Default: 20");
            Assassin2Slash1 = Config.Bind("Assassin", "Enable Slash 1", true, "Enable the first slash hit.");
            Assassin2Slash2 = Config.Bind("Assassin", "Enable Slash 2", true, "Enable the second slash hit.");
            Assassin2SlashExpose = Config.Bind("Assassin", "Enable Slash Expose", 4f, "In seconds. Enable the slash hit to `expose` you.");

            Assassin2SlashSize = Config.Bind("Assassin", "Slash Hitbox Size Multiplier", 2f, "stop whiffing wtf");
            Assassin2SlashMultihit = Config.Bind("Assassin", "Slash Multihit", false, "This is the shuriken 5 hit");

            Assassin2ShurikenSize = Config.Bind("Assassin", "Shuriken Hitbox Size Multiplier", 2f, "stop whiffing wtf");

            Assassin2Accurate = Config.Bind("Assassin", "Shuriken Accurate Enemies Compat", true, "oh they mess you up good");
            Assassin2AccurateLoop = Config.Bind("Assassin", "Shuriken Accurate Enemies Compat (loop only)", false, "oh they mess you up good");

            FamilyEventChance = Config.Bind("Family Event", "Family Event Chance", 0.02f, "");
            EnableFamilyChange = Config.Bind("Family Event", "Enable Family Compats", true, "Enable all family event related compats.");
            TarFamily = Config.Bind("Family Event", "Enable Tar Family", "ancientloft, itancientloft, wispgraveyard, sulfurpools, goolake, itgoolake, drybasin", "Custom family event. List of scenes, separated by comma. blank to disable.");
            AltWispFamily = Config.Bind("Family Event", "Enable Alt Wisp Family", "ancientloft, itancientloft, frozenwall, snowyforest, forgottenhaven, FBLScene", "Custom family event. List of scenes, separated by comma. blank to disable. otherwise wisp enemies will be added to normal wisp group.");
            SolusFamily = Config.Bind("Family Event", "Enable Solus Family", "arena, shipgraveyard, itskymeadow, skymeadow", "Custom family event. List of scenes, separated by comma. blank to disable.");
            BlindFamily = Config.Bind("Family Event", "Enable Blind Family", "ancientloft, itancientloft, itfrozenwall, frozenwall, snowyforest, FBLScene", "Custom family event. List of scenes, separated by comma. blank to disable.");
            BellFamily = Config.Bind("Family Event", "Enable Bell Family", "itdampcave, dampcavesimple, shipgraveyard, itskymeadow, skymeadow, foggyswamp, FBLScene", "Custom family event. List of scenes, separated by comma. blank to disable. disables if BT is not loaded.");
            RemoveLarvaFamily = Config.Bind("Family Event", "Remove Larva Family Event", true, "jesus lol");

            LarvaDetonateSelfDamage = Config.Bind("Sneaky Rebalance", "Acid Larva Detonation Self Damage Fraction", 1f, "What if larvas died on first leap (no way), Default: 0.25");
            AWUShieldTime = Config.Bind("Sneaky Rebalance", "Alloy Worship Unit Shield Uptime", 2.5f, "Default: 6.");
            DireseekerDropAtPlace = Config.Bind("Sneaky Rebalance", "Direseeker Drops Item In Place", true, "Literally where did it drop what");

            On.EntityStates.AcidLarva.LarvaLeap.OnEnter += (orig, self) => { orig(self); self.detonateSelfDamageFraction = LarvaDetonateSelfDamage.Value; };

            Reworks.IotaConstruct();
            Reworks.Assassin2();
            DLC1 = vanilla<ExpansionDef>("DLC1/Common/DLC1");

            if (Mods("PlasmaCore.ForgottenRelics"))
            {
                Harmony.PatchAll(typeof(PatchVF2Start));
                addCoilGolemLogbook();
            }
            void addCoilGolemLogbook() { if (!FRCSharp.VF2ConfigManager.disableCoilGolem.Value) FRCSharp.VF2ContentPackProvider.contentPack.bodyPrefabs.Add(new GameObject[] { FRCSharp.VF2ContentPackProvider.coilGolemBody }); }

            // Stats change ----------------------------------------------------------------------------------------------------

            BodyCatalog.availability.CallWhenAvailable(() =>
            {
                CharacterBody bodyMajorConstruct = BodyCatalog.FindBodyPrefab("MajorConstructBody").GetComponent<CharacterBody>();
                CharacterBody bodyAssassin2 = BodyCatalog.FindBodyPrefab("Assassin2Body").GetComponent<CharacterBody>();
                CharacterBody bodyMagmaWorm = BodyCatalog.FindBodyPrefab("MagmaWormBody").GetComponent<CharacterBody>();
                CharacterBody bodyOverloadingWorm = BodyCatalog.FindBodyPrefab("ElectricWormBody").GetComponent<CharacterBody>();
                CharacterBody bodyBellTower = BodyCatalog.FindBodyPrefab("BellTowerMonsterBody")?.GetComponent<CharacterBody>();
                CharacterBody bodyAcidLarva = BodyCatalog.FindBodyPrefab("AcidLarvaBody").GetComponent<CharacterBody>();
                CharacterBody bodyAWU = BodyCatalog.FindBodyPrefab("SuperRoboBallBossBody").GetComponent<CharacterBody>();
                CharacterBody bodyAurellionite = BodyCatalog.FindBodyPrefab("TitanGoldBody").GetComponent<CharacterBody>();
                CharacterBody bodyArtifactShell = BodyCatalog.FindBodyPrefab("ArtifactShellBody").GetComponent<CharacterBody>();
                CharacterBody bodyDireseeker = BodyCatalog.FindBodyPrefab("DireseekerBossBody")?.GetComponent<CharacterBody>();

                bodyMajorConstruct.baseMaxHealth = MajorConstructHealth.Value;
                bodyMajorConstruct.levelMaxHealth = MajorConstructHealthStack.Value;
                bodyMajorConstruct.baseDamage = MajorConstructDamage.Value;
                bodyMajorConstruct.levelDamage = MajorConstructDamageStack.Value;
                bodyMajorConstruct.baseAttackSpeed = MajorConstructAttackSpeed.Value;
                bodyMajorConstruct.baseArmor = MajorConstructArmor.Value;
                bodyMajorConstruct.isChampion = true;

                if (!string.IsNullOrWhiteSpace(MajorConstructSpawn.Value))
                {
                    bodyMajorConstruct.GetComponent<DeathRewards>().bossDropTable = vanilla<ExplicitPickupDropTable>("DLC1/MajorAndMinorConstruct/dtBossMegaConstruct");
                    bodyMajorConstruct.GetComponent<DeathRewards>().logUnlockableDef = LegacyResourcesAPI.Load<UnlockableDef>("UnlockableDefs/Logs.MajorConstructBody");
                }

                bodyAssassin2.baseMaxHealth = Assassin2Health.Value;
                bodyAssassin2.levelMaxHealth = Assassin2HealthStack.Value;
                bodyAssassin2.baseDamage = Assassin2Damage.Value;
                bodyAssassin2.levelDamage = Assassin2DamageStack.Value;
                bodyAssassin2.baseAttackSpeed = Assassin2AttackSpeed.Value;
                bodyAssassin2.baseMoveSpeed = Assassin2Speed.Value;
                bodyAssassin2.baseArmor = Assassin2Armor.Value;

                if (!string.IsNullOrWhiteSpace(Assassin2Spawn.Value))
                {
                    bodyAssassin2.gameObject.AddComponent<DeathRewards>().logUnlockableDef = vanilla<UnlockableDef>("DLC1/Assassin2/Logs.Assassin2Body");
                }

                bodyMagmaWorm.baseMaxHealth = MagmaWormHealth.Value;
                bodyMagmaWorm.levelMaxHealth = MagmaWormHealthStack.Value;
                bodyMagmaWorm.baseDamage = MagmaWormDamage.Value;
                bodyMagmaWorm.levelDamage = MagmaWormDamageStack.Value;
                bodyMagmaWorm.baseAttackSpeed = MagmaWormAttackSpeed.Value;
                bodyMagmaWorm.baseMoveSpeed = MagmaWormSpeed.Value;
                bodyMagmaWorm.baseArmor = MagmaWormArmor.Value;

                bodyOverloadingWorm.baseMaxHealth = OverloadingWormHealth.Value;
                bodyOverloadingWorm.levelMaxHealth = OverloadingWormHealthStack.Value;
                bodyOverloadingWorm.baseDamage = OverloadingWormDamage.Value;
                bodyOverloadingWorm.levelDamage = OverloadingWormDamageStack.Value;
                bodyOverloadingWorm.baseAttackSpeed = OverloadingWormAttackSpeed.Value;
                bodyOverloadingWorm.baseMoveSpeed = OverloadingWormSpeed.Value;
                bodyOverloadingWorm.baseArmor = OverloadingWormArmor.Value;

                if (bodyBellTower != null)
                {
                    bodyBellTower.baseMaxHealth = BellTowerHealth.Value;
                    bodyBellTower.levelMaxHealth = BellTowerHealthStack.Value;
                    bodyBellTower.baseDamage = BellTowerDamage.Value;
                    bodyBellTower.levelDamage = BellTowerDamageStack.Value;
                    bodyBellTower.baseAttackSpeed = BellTowerAttackSpeed.Value;
                    bodyBellTower.baseArmor = BellTowerArmor.Value;
                }

                bodyAcidLarva.baseMaxHealth = AcidLarvaHealth.Value;
                bodyAcidLarva.levelMaxHealth = AcidLarvaHealthStack.Value;
                bodyAcidLarva.baseDamage = AcidLarvaDamage.Value;
                bodyAcidLarva.levelDamage = AcidLarvaDamageStack.Value;
                bodyAcidLarva.baseAttackSpeed = AcidLarvaAttackSpeed.Value;
                bodyAcidLarva.baseMoveSpeed = AcidLarvaSpeed.Value;
                bodyAcidLarva.baseArmor = AcidLarvaArmor.Value;

                bodyAWU.baseMaxHealth = AWUHealth.Value;
                bodyAWU.levelMaxHealth = AWUHealthStack.Value;
                bodyAWU.baseDamage = AWUDamage.Value;
                bodyAWU.levelDamage = AWUDamageStack.Value;
                bodyAWU.baseAttackSpeed = AWUAttackSpeed.Value;
                bodyAWU.baseMoveSpeed = AWUSpeed.Value;
                bodyAWU.baseArmor = AWUArmor.Value;

                bodyAurellionite.baseMaxHealth = AurellioniteHealth.Value;
                bodyAurellionite.levelMaxHealth = AurellioniteHealthStack.Value;
                bodyAurellionite.baseDamage = AurellioniteDamage.Value;
                bodyAurellionite.levelDamage = AurellioniteDamageStack.Value;
                bodyAurellionite.baseAttackSpeed = AurellioniteAttackSpeed.Value;
                bodyAurellionite.baseMoveSpeed = AurellioniteSpeed.Value;
                bodyAurellionite.baseArmor = AurellioniteArmor.Value;

                bodyArtifactShell.baseMaxHealth = ArtifactShellHealth.Value;
                bodyArtifactShell.levelMaxHealth = ArtifactShellHealthStack.Value;
                bodyArtifactShell.baseDamage = ArtifactShellDamage.Value;
                bodyArtifactShell.levelDamage = ArtifactShellDamageStack.Value;
                bodyArtifactShell.baseAttackSpeed = ArtifactShellAttackSpeed.Value;
                bodyArtifactShell.baseArmor = ArtifactShellArmor.Value;

                if (bodyDireseeker != null)
                {
                    bodyDireseeker.baseMaxHealth = DireseekerHealth.Value;
                    bodyDireseeker.levelMaxHealth = DireseekerHealthStack.Value;
                    bodyDireseeker.baseDamage = DireseekerDamage.Value;
                    bodyDireseeker.levelDamage = DireseekerDamageStack.Value;
                    bodyDireseeker.baseAttackSpeed = DireseekerAttackSpeed.Value;
                    bodyDireseeker.baseMoveSpeed = DireseekerSpeed.Value;
                    bodyDireseeker.baseArmor = DireseekerArmor.Value;
                }
            });

            CharacterSpawnCard cscMajorConstruct = vanilla<CharacterSpawnCard>("DLC1/MajorAndMinorConstruct/cscMajorConstruct");
            CharacterSpawnCard cscAssassin2 = vanilla<CharacterSpawnCard>("DLC1/Assassin2/cscAssassin2");
            CharacterSpawnCard cscMagmaWorm = vanilla<CharacterSpawnCard>("Base/MagmaWorm/cscMagmaWorm");
            CharacterSpawnCard cscOverloadingWorm = vanilla<CharacterSpawnCard>("Base/ElectricWorm/cscElectricWorm");
            CharacterSpawnCard cscAcidLarva = vanilla<CharacterSpawnCard>("DLC1/AcidLarva/cscAcidLarva");

            cscMajorConstruct.directorCreditCost = MajorConstructCost.Value;
            cscAssassin2.directorCreditCost = Assassin2Cost.Value;
            cscMagmaWorm.directorCreditCost = MagmaWormCost.Value;
            cscOverloadingWorm.directorCreditCost = OverloadingWormCost.Value;
            cscAcidLarva.directorCreditCost = AcidLarvaCost.Value;
            if (Mods("PlasmaCore.ForgottenRelics")) changeBTCost();
            void changeBTCost() => FRCSharp.VF2ContentPackProvider.cscBellTower.directorCreditCost = BellTowerCost.Value;

            // spawns ----------------------------------------------------------------------------------------------------

            DirectorCard majorConstructDC = new DirectorCard
            {
                spawnCard = cscMajorConstruct,
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                minimumStageCompletions = MajorConstructMinStage.Value - 1,
                preventOverhead = true
            };
            DirectorCard assassin2DC = new DirectorCard
            {
                spawnCard = cscAssassin2,
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                minimumStageCompletions = Assassin2MinStage.Value - 1,
                preventOverhead = true
            };
            if (EliteMagmaWorm.Value) enableElite(cscMagmaWorm);
            if (EliteOverloadingWorm.Value) enableElite(cscOverloadingWorm);
            if (EliteAWU.Value) enableElite(vanilla<CharacterSpawnCard>("Base/RoboBallBoss/cscSuperRoboBallBoss"));
            if (EliteAurellionite.Value) enableElite(vanilla<CharacterSpawnCard>("Base/Titan/cscTitanGold"));
            if (EliteArtifactShell.Value) On.EntityStates.ArtifactShell.WaitForIntro.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.characterBody.eliteBuffCount != 0) return;
                List<EliteDef> defs = new();
                foreach (var list in CombatDirector.eliteTiers.Where(x => RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly) || (x.CanSelect(SpawnCard.EliteRules.Default) && x.costMultiplier < Run.instance.loopClearCount * 10)).ToList().ConvertAll(x => x.eliteTypes))
                    foreach (var z in list) if (z != null && z.IsAvailable()) defs.Add(z);
                EliteDef def = Run.instance.spawnRng.NextElementUniform(defs);
                self.characterBody.AddBuff(def.eliteEquipmentDef.passiveBuffDef);
            };
            if (EliteDireseeker.Value && Mods("com.rob.Direseeker")) enableDireseeker();
            void enableDireseeker() => enableElite(DireseekerMod.Modules.SpawnCards.bossSpawnCard);
            void enableElite(CharacterSpawnCard csc) { csc.noElites = false; csc.eliteRules = SpawnCard.EliteRules.Default; };

            DirectorCardHolder majorConstructDCH = new() { Card = majorConstructDC, MonsterCategory = MonsterCategory.Champions };
            DirectorCardHolder assassin2DCH = new() { Card = assassin2DC, MonsterCategory = MonsterCategory.BasicMonsters };
            string[] iotalist = listify(MajorConstructSpawn.Value).ToArray();
            string[] assassinlist = listify(Assassin2Spawn.Value).ToArray();
            string[] btlist = listify(BellTowerSpawn.Value).ToArray();
            Dictionary<string, List<DirectorCardHolder>> manualDCCSAdd = new();
            if (!string.IsNullOrWhiteSpace(MajorConstructSpawn.Value))
            {
                AddNewMonsterToStagesWhere(majorConstructDCH, false, info => iotalist.Contains(info.CustomStageName) || iotalist.Contains(info.ToInternalStageName()));
                RoR2Application.onLoad += () => RoR2Content.mixEnemyMonsterCards.AddCard(majorConstructDCH);
                addManual("dccsConstructFamily", majorConstructDCH);
            }
            if (!string.IsNullOrWhiteSpace(Assassin2Spawn.Value))
            {
                AddNewMonsterToStagesWhere(assassin2DCH, false, info => assassinlist.Contains(info.CustomStageName) || assassinlist.Contains(info.ToInternalStageName()));
                RoR2Application.onLoad += () => RoR2Content.mixEnemyMonsterCards.AddCard(assassin2DCH);
            }
            if (Mods("PlasmaCore.ForgottenRelics")) ReaddBT();
            void ReaddBT()
            {
                if (FRCSharp.VF2ConfigManager.disableBellTower.Value) return;
                DirectorCard dc = GetDirectorCard(FRCSharp.VF2ContentPackProvider.cscBellTower);
                dc.minimumStageCompletions = BellTowerMinStage.Value;
                AddNewMonsterToStagesWhere(new() { Card = dc, MonsterCategory = MonsterCategory.Champions }, false, info => btlist.Contains(info.CustomStageName) || btlist.Contains(info.ToInternalStageName()));
            }

            // Families
            ClassicStageInfo.monsterFamilyChance = FamilyEventChance.Value;
            void addManual(string k, DirectorCardHolder v)
            {
                if (!manualDCCSAdd.ContainsKey(k)) manualDCCSAdd.Add(k, new());
                manualDCCSAdd[k].Add(v);
            }
            IL.RoR2.ClassicStageInfo.RebuildCards += il =>
            {
                ILCursor c = new(il);
                c.GotoNext(MoveType.AfterLabel, x => x.MatchLdloc(7), x => x.MatchBrfalse(out _));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<ClassicStageInfo>>(self => HandleDccs(self.modifiableMonsterCategories));
            };
            void HandleDccs(DirectorCardCategorySelection dccs)
            {
                foreach (var k in manualDCCSAdd.Keys) if (dccs.name.Contains(k)) foreach (var v in manualDCCSAdd[k])
                {
                    Log.LogDebug($"Adding {v.Card.spawnCard.name} to {k}");
                    if (!dccs.categories.Any(x => x.name == GetVanillaMonsterCategoryName(v.MonsterCategory))) dccs.AddCategory(GetVanillaMonsterCategoryName(v.MonsterCategory), 1);
                    dccs.AddCard(v);
                }
            }

            string wispFamily = string.IsNullOrWhiteSpace(AltWispFamily.Value) ? "dccsWispFamily" : "dccsAltWispFamily";
            if (EnableFamilyChange.Value)
            {
                if (Mods("com.plasmacore.PlasmaCoreSpikestripContent")) addSigma();
                void addSigma()
                {
                    if (!PlasmaCoreSpikestripContent.Content.Monsters.SigmaConstruct.instance.IsEnabled) return;
                    addManual("dccsConstructFamily", new() { Card = PlasmaCoreSpikestripContent.Content.Monsters.SigmaConstruct.instance.DirectorCard, MonsterCategory = MonsterCategory.Minibosses });
                }
                if (Mods("com.groovesalad.GrooveSaladSpikestripContent") && !string.IsNullOrWhiteSpace(TarFamily.Value)) addPot();
                void addPot()
                {
                    if (!GrooveSaladSpikestripContent.Content.PotMobile.instance.IsEnabled) return;
                    addManual("dccsTarFamily", new() { Card = GrooveSaladSpikestripContent.Content.PotMobile.instance.DirectorCard, MonsterCategory = MonsterCategory.BasicMonsters });
                }
                if (Mods("PlasmaCore.ForgottenRelics")) addFR();
                void addFR()
                {
                    if (!FRCSharp.VF2ConfigManager.disableFrostWisp.Value) addManual(wispFamily, new()
                    {
                        Card = GetDirectorCard(FRCSharp.VF2ContentPackProvider.cscFrostWisp),
                        MonsterCategory = MonsterCategory.Minibosses
                    });
                    if (!FRCSharp.VF2ConfigManager.disableCoilGolem.Value)
                    {
                        DirectorCard dc = GetDirectorCard(FRCSharp.VF2ContentPackProvider.cscCoilGolem);
                        dc.spawnDistance = DirectorCore.MonsterSpawnDistance.Far;
                        addManual("dccsGolemFamily", new() { Card = dc, MonsterCategory = MonsterCategory.Minibosses });
                        addManual("fdccsGolemSandy", new() { Card = dc, MonsterCategory = MonsterCategory.Minibosses });
                    }
                    if (!FRCSharp.VF2ConfigManager.disableBellTower.Value && !string.IsNullOrWhiteSpace(BellFamily.Value))
                    {
                        DirectorCard dc = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/Bell/cscBell"));
                        dc.preventOverhead = true;
                        AddFamilyEvent("Bell", new() { new() { Card = dc, MonsterCategory = MonsterCategory.Minibosses } }, listify(BellFamily.Value)).AddCard(new()
                        {
                            Card = GetDirectorCard(FRCSharp.VF2ContentPackProvider.cscBellTower),
                            MonsterCategory = MonsterCategory.Minibosses
                        });
                    }
                }
                if (RemoveLarvaFamily.Value) onRebuildCards += self => { if (self.monsterDccsPool.poolCategories.Length >= 2) 
                        self.monsterDccsPool.poolCategories[1].includedIfConditionsMet = self.monsterDccsPool.poolCategories[1].includedIfConditionsMet.Where(x => x.dccs.name != "dccsAcidLarvaFamily").ToArray(); };
                RoR2Application.onLoad += () => EntityStates.RoboBallBoss.Weapon.FireSuperDelayKnockup.shieldDuration = AWUShieldTime.Value;
                if (DireseekerDropAtPlace.Value) Harmony.PatchAll(typeof(PatchDireseeker));
                LanguageAPI.Add("FAMILY_BELL", "<style=cWorldEvent>[WARNING] The clock is ticking...</style>");
                if (Mods("com.Moffein.ClayMen") && !string.IsNullOrWhiteSpace(TarFamily.Value)) addClaymen();
                void addClaymen() => addManual("dccsTarFamily", ClayMen.ClayMenContent.ClayManCard);
                if (Mods("com.Moffein.AncientWisp")) addAncientWisp();
                void addAncientWisp() => addManual(wispFamily, AncientWisp.AWContent.AncientWispCard);
                if (Mods("com.Moffein.ArchaicWisp")) addArchaicWisp();
                void addArchaicWisp() => addManual(wispFamily, ArchaicWisp.ArchaicWispContent.ArchaicWispCard);
                AddFamilyToStages(vanilla<FamilyDirectorCardCategorySelection>("Base/Common/dccsParentFamily"), new() { "itskymeadow", "skymeadow", "forgottenhaven", "slumberingsatellite", "artifactworld" });
                AddFamilyToStages(vanilla<FamilyDirectorCardCategorySelection>("DLC1/Common/dccsConstructFamily"), new() { "itskymeadow", "skymeadow", "FBLScene", "forgottenhaven", "golemplains", "golemplains2" });
                AddFamilyToStages(vanilla<FamilyDirectorCardCategorySelection>("Base/Common/dccsLemurianFamily"), new() { "drybasin", "goldshores" });
                AddFamilyToStages(vanilla<FamilyDirectorCardCategorySelection>("Base/Common/dccsGolemFamily"), new() { "forgottenhaven" });
                AddFamilyToStages(vanilla<FamilyDirectorCardCategorySelection>("Base/Common/dccsGolemFamilyNature"), new() { "goldshores", "FBLScene" });
                addManual("dccsGupFamily", new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("DLC1/Gup/cscGeepBody")), MonsterCategory = MonsterCategory.Minibosses });
                addManual("dccsGupFamily", new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("DLC1/Gup/cscGipBody")), MonsterCategory = MonsterCategory.BasicMonsters });
            }
            if (!string.IsNullOrWhiteSpace(TarFamily.Value)) AddFamilyEvent("Tar", new() {
                new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/ClayBoss/cscClayBoss")), MonsterCategory = MonsterCategory.Champions },
                new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/ClayBruiser/cscClayBruiser")), MonsterCategory = MonsterCategory.Minibosses },
                new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("DLC1/ClayGrenadier/cscClayGrenadier")), MonsterCategory = MonsterCategory.Minibosses }
            }, listify(TarFamily.Value));
            LanguageAPI.Add("FAMILY_TAR", "<style=cWorldEvent>[WARNING] The ground oozes with tar...</style>");
            if (!string.IsNullOrWhiteSpace(AltWispFamily.Value)) AddFamilyEvent("AltWisp", new() 
            { 
                new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/Wisp/cscLesserWisp")), MonsterCategory = MonsterCategory.BasicMonsters }
            }, listify(AltWispFamily.Value));
            LanguageAPI.Add("FAMILY_ALTWISP", "<style=cWorldEvent>[WARNING] The air begins to burn,</style> <style=cDeath>but...</style>");
            if (!string.IsNullOrWhiteSpace(SolusFamily.Value))
            {
                DirectorCard vultureDC = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/Vulture/cscVulture"));
                vultureDC.preventOverhead = true;
                AddFamilyEvent("Solus", new()
                {
                    new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/RoboBallBoss/cscRoboBallBoss")), MonsterCategory = MonsterCategory.Champions },
                    new() { Card = vultureDC, MonsterCategory = MonsterCategory.BasicMonsters },
                    new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("Base/RoboBallBoss/cscRoboBallMini")), MonsterCategory = MonsterCategory.BasicMonsters }
                }, listify(SolusFamily.Value));
            }
            LanguageAPI.Add("FAMILY_SOLUS", "<style=cWorldEvent>[WARNING] The flying ones are restless today...</style>");
            if (!string.IsNullOrWhiteSpace(BlindFamily.Value)) {
                DirectorCard magmaWormDC = GetDirectorCard(cscMagmaWorm);
                DirectorCard electricWormDC = GetDirectorCard(cscOverloadingWorm);
                magmaWormDC.spawnDistance = DirectorCore.MonsterSpawnDistance.Far;
                electricWormDC.spawnDistance = DirectorCore.MonsterSpawnDistance.Far;
                AddFamilyEvent("Blind", new()
                {
                    new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("DLC1/Vermin/cscVermin")), MonsterCategory = MonsterCategory.BasicMonsters },
                    new() { Card = GetDirectorCard(vanilla<CharacterSpawnCard>("DLC1/FlyingVermin/cscFlyingVermin")), MonsterCategory = MonsterCategory.BasicMonsters },
                    new() { Card = magmaWormDC, MonsterCategory = MonsterCategory.Champions },
                    new() { Card = electricWormDC, MonsterCategory = MonsterCategory.Champions }
                }, listify(BlindFamily.Value));
            }
            LanguageAPI.Add("FAMILY_BLIND", "<style=cWorldEvent>[WARNING] The sightless erupts from the ground...</style>");

            On.RoR2.ClassicStageInfo.RebuildCards += (orig, self) =>
            {
                if (onRebuildCards != null) onRebuildCards(self);
                orig(self);
            };
        }

        public static event Action<ClassicStageInfo> onRebuildCards;
        public static FamilyDirectorCardCategorySelection AddFamilyEvent(string name, List<DirectorCardHolder> list, List<string> stages)
        {
            FamilyDirectorCardCategorySelection dccs = ScriptableObject.CreateInstance<FamilyDirectorCardCategorySelection>();
            dccs.name = $"dccs{name}Family";
            dccs.selectionChatString = "FAMILY_" + name.ToUpper();
            dccs.categories = new DirectorCardCategorySelection.Category[] { new() { name = GetVanillaMonsterCategoryName(MonsterCategory.BasicMonsters), cards = Array.Empty<DirectorCard>(), selectionWeight = 4 }, new() { name = GetVanillaMonsterCategoryName(MonsterCategory.Minibosses), cards = Array.Empty<DirectorCard>(), selectionWeight = 2 }, new() { name = GetVanillaMonsterCategoryName(MonsterCategory.Champions), cards = new DirectorCard[]  { }, selectionWeight = 2 }, new() { name = GetVanillaMonsterCategoryName(MonsterCategory.Special), cards = Array.Empty<DirectorCard>(), selectionWeight = 1 } };
            foreach (var card in list)
            {
                Log.LogDebug($"Adding {card.Card.spawnCard.name} to Family {name}");
                dccs.AddCard(card);
            }
            dccs.categories = dccs.categories.Where(x => x.cards.Length > 0).ToArray();
            AddFamilyToStages(dccs, stages);
            customFamilies.Add(dccs);
            return dccs;
        }

        public static void AddFamilyToStages(FamilyDirectorCardCategorySelection dccs, List<string> stages)
        {
            onRebuildCards += self =>
            {
                if (stages.Contains(SceneCatalog.mostRecentSceneDef.cachedName))
                {
                    if (self.monsterDccsPool.poolCategories.Length == 1) HG.ArrayUtils.ArrayAppend(ref self.monsterDccsPool.poolCategories, new() { name = MonsterPoolCategories.Family, categoryWeight = 1, alwaysIncluded = Array.Empty<PoolEntry>(), includedIfConditionsMet = Array.Empty<ConditionalPoolEntry>(), includedIfNoConditionsMet = Array.Empty<PoolEntry>() });
                    HG.ArrayUtils.ArrayAppend(ref self.monsterDccsPool.poolCategories[1].includedIfConditionsMet, new() { requiredExpansions = new[] { DLC1 }, dccs = dccs, weight = 1 });
                }
            };
        }

        public static DirectorCard GetDirectorCard(CharacterSpawnCard csc) => new()
        {
            spawnCard = csc,
            selectionWeight = 1,
            preventOverhead = false,
            minimumStageCompletions = 0,
            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
        };

        public static bool Mods(params string[] arr)
        {
            for (int i = 0; i < arr.Length; i++) if (!Chainloader.PluginInfos.ContainsKey(arr[i])) return false;
            return true;
        }

        public static List<string> listify(string list) => list.Split(',').ToList().ConvertAll(x => x.Trim());
        public static Dictionary<string, string> pairify(string list) 
        {
            Dictionary<string, string> ret = new();
            foreach (var s in list.Split(',').ToList())
            {
                List<string> kv = s.Split('-').ToList().ConvertAll(x => x.Trim());
                ret.Add(kv[0], kv[1]);
            }
            return ret;
        }
        public static T vanilla<T>(string path) => Addressables.LoadAssetAsync<T>($"RoR2/{path}.asset").WaitForCompletion();
    }
}
