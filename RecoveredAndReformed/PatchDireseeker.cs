using DireseekerMod.States.Missions.DireseekerEncounter;
using HarmonyLib;
using RoR2;

namespace RecoveredAndReformed
{
    [HarmonyPatch(typeof(Listening), nameof(Listening.FixedUpdate))]
    public class PatchDireseeker
    {
        public static void Postfix(Listening __instance)
        {
            if (__instance.gameObject.GetComponent<CombatSquad>().membersList.Count == 0) return;
            __instance.transform.Find("DropPosition").position = __instance.gameObject.GetComponent<CombatSquad>().membersList[0].GetBody().corePosition;
        }
    }
}
