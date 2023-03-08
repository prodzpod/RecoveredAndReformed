using EntityStates;
using EntityStates.Assassin2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RecoveredAndReformed
{
    public class Reworks
    {
        public static Dictionary<CharacterBody, List<GameObject>> spawnedConstructs = new();
        public static void IotaConstruct()
        {
            // Death Timer
            On.EntityStates.GenericCharacterDeath.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (Main.MajorConstructDeathTimer.Value >= 0 && self is EntityStates.MajorConstruct.Death)
                {
                    if (self.fixedAge <= Main.MajorConstructDeathTimer.Value || !NetworkServer.active) return;
                    self.DestroyBodyAsapServer();
                }
            };
            string constructToSpawn = Main.MajorConstructSpawnSigmaInstead.Value ? "SigmaConstructBody" : "MinorConstructBody";
            CharacterSpawnCard card = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMinorConstruct");
            if (Main.Mods("com.plasmacore.PlasmaCoreSpikestripContent") && Main.MajorConstructSpawnSigmaInstead.Value) card = slasma();
            static CharacterSpawnCard slasma() => PlasmaCoreSpikestripContent.Content.Monsters.SigmaConstruct.instance.CharacterSpawnCard;
            On.EntityStates.MajorConstruct.Weapon.FireLaser.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.characterBody.name.Contains("MajorConstruct"))
                {
                    self.maxDistance = Main.MajorConstructBeamDistance.Value;
                    self.aimMaxSpeed = Main.MajorConstructAimSpeed.Value;
                }
            };
            On.EntityStates.MajorConstruct.Stance.LoweredToRaised.OnEnter += (orig, self) =>
            {
                orig(self);
                if (Main.MajorConstructSpawnAmount.Value <= 0 || !NetworkServer.active) return;
                foreach (HurtBox hurtBox in new SphereSearch() { origin = self.gameObject.transform.position, radius = 37.5f, mask = LayerIndex.entityPrecise.mask }.RefreshCandidates().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
                {
                    CharacterBody body = hurtBox.healthComponent.body;
                    if (body && body.name == constructToSpawn) return;
                }
                Quaternion rot = Quaternion.AngleAxis(120f, Vector3.up);
                Vector3 cur = self.transform.forward * 16f;
                if (!spawnedConstructs.ContainsKey(self.characterBody)) spawnedConstructs.Add(self.characterBody, new());
                for (int i = 0; i < Main.MajorConstructSpawnAmount.Value; i++)
                {
                    Vector3 pos = cur + self.transform.position;
                    GameObject obj = card.DoSpawn(pos, Quaternion.Euler(self.transform.forward), new(card, new() { position = pos, placementMode = DirectorPlacementRule.PlacementMode.Direct }, Run.instance.spawnRng) { teamIndexOverride = self.characterBody.teamComponent.teamIndex }).spawnedInstance;
                    NetworkServer.Spawn(obj);
                    spawnedConstructs[self.characterBody].Add(obj);
                    cur = rot * cur;
                }
            };
            GlobalEventManager.onCharacterDeathGlobal += report =>
            {
                if (spawnedConstructs.ContainsKey(report.victimBody)) foreach (var body in spawnedConstructs[report.victimBody])
                        body?.GetComponent<CharacterMaster>()?.bodyInstanceObject?.GetComponent<HealthComponent>()?.Suicide(report.attackerBody.gameObject);
                spawnedConstructs.Remove(report.victimBody);
            };
            LanguageAPI.Add("MAJORCONSTRUCT_BODY_NAME", "Iota Construct");
            LanguageAPI.Add("MAJORCONSTRUCT_BODY_SUBTITLE", "Released From The Void");
            LanguageAPI.Add("MAJORCONSTRUCT_BODY_LORE", "<style=cMono>Welcome to DataScrapper (v3.2rc1-DEV BUILD)\r\n$ Scraping memory... done. [List#23376190, Length: 3752]\r\n$ Resolving... done.\r\n$ Combining for relevant data... done.\r\nComplete!\r\nOutputting [test.c]...</style>\r\n\r\n/*for(int i = 0; i < N; i++)\r\n{\r\n    printf(\"\n\");\r\n    for(int j = 0; j < N; j++)\r\n    {\r\n        printf(\"%d \", arr[i][j]); //range setting and random number generation (rand from stdlib.h)\r\n    }\r\n}*/ //toggle parts of code using comments\r\n\r\nreturn 0; //return 0\r\n");
        }
        public static void Assassin2()
        {
            RoR2Application.onLoad += () =>
            {
                ChargeDash.baseDuration = Main.Assassin2DashDuration.Value - 1f;
                DashStrike.slashDuration = Main.Assassin2SlashDuration.Value;
                DashStrike.damageCoefficient = Main.Assassin2SlashDamage.Value;
                DashStrike.forceMagnitude = Main.Assassin2SlashKnockback.Value;
                DashStrike.maxSlashDistance = Main.Assassin2SlashReach.Value;
                Hide.hiddenDuration = Main.Assassin2InvisDuration.Value;
                Hide.fullDuration = Main.Assassin2InvisDuration.Value + Main.Assassin2InvisEndlag.Value;

                AISkillDriver[] ai = MasterCatalog.FindMasterPrefab("Assassin2Master").GetComponents<AISkillDriver>();
                ai[1].maxDistance = 30;
                ai[1].activationRequiresTargetLoS = true;
                ai[1].activationRequiresAimTargetLoS = true;
                ai[1].selectionRequiresAimTarget = true;
                ai[1].activationRequiresAimConfirmation = true;

                ai[2].aimType = AISkillDriver.AimType.AtCurrentEnemy;
                ai[2].movementType = AISkillDriver.MovementType.StrafeMovetarget;

                ai[3].shouldSprint = true;
            };
            GlobalEventManager.onServerDamageDealt += report => { if (Main.Assassin2SlashExpose.Value > 0 && (report?.attackerBody?.name?.Contains("Assassin2") ?? false)) report?.victimBody?.AddTimedBuff(RoR2Content.Buffs.DeathMark, Main.Assassin2SlashExpose.Value); };
            On.EntityStates.Assassin2.DashStrike.HandleSlash += (orig, self, hash, a, b) => { if ((self.slashCount == 0 && Main.Assassin2Slash1.Value) || (self.slashCount == 1 && Main.Assassin2Slash2.Value)) orig(self, hash, a, b); else self.slashCount++; };
            IL.EntityStates.Assassin2.DashStrike.HandleSlash += il =>
            {
                ILCursor c = new(il);
                c.GotoNext(x => x.MatchLdnull(), x => x.MatchCallOrCallvirt<OverlapAttack>(nameof(OverlapAttack.Fire)));
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<OverlapAttack>>(oa =>
                {
                    for (int i = 0; i < oa.hitBoxGroup.hitBoxes.Length; i++) oa.hitBoxGroup.hitBoxes[i].transform.localScale *= Main.Assassin2SlashSize.Value;
                });
                if (!Main.Assassin2SlashMultihit.Value)
                {
                    c.GotoNext(x => x.MatchRet());
                    c.Remove();
                }
            };
            On.EntityStates.Assassin2.ThrowShuriken.OnEnter += (orig, self) => 
            { 
                orig(self); 
                self.projectilePrefab.GetComponent<SphereCollider>().radius = 0.2f * Main.Assassin2ShurikenSize.Value;
            };
            IL.EntityStates.GenericProjectileBaseState.FireProjectile += il =>
            {
                ILCursor c = new(il);
                c.GotoNext(x => x.MatchStloc(0));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, GenericProjectileBaseState, Ray>>((orig, self) =>
                {
                    if (self is ThrowShuriken && Main.Mods("com.Moffein.AccurateEnemies")) return handleAccurate();
                    else return orig;
                    Ray handleAccurate()
                    {
                        if (!Main.Assassin2Accurate.Value && (!Main.Assassin2AccurateLoop.Value || Run.instance.loopClearCount == 0)) return orig;
                        return AccurateEnemies.Util.PredictAimrayPS(orig, self.characterBody.teamComponent.teamIndex, AccurateEnemies.AccurateEnemiesPlugin.basePredictionAngle, self.projectilePrefab, AccurateEnemies.Util.GetMasterAITargetHurtbox(self.characterBody.master));
                    }
                });
            };
        }
    }   
}