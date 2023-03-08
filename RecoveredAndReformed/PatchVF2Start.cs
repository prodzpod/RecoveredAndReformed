using FRCSharp;
using HarmonyLib;

namespace RecoveredAndReformed
{

    [HarmonyPatch(typeof(VF2Plugin), nameof(VF2Plugin.Start))]
    public class PatchVF2Start
    {
        public static bool trueDisableBellTower;
        public static void Prefix() { trueDisableBellTower = VF2ConfigManager.disableBellTower.Value; }
        public static void Postfix() { VF2ConfigManager.disableBellTower.Value = trueDisableBellTower; }
    }
}
