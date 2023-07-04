#if !DEBUG
using HarmonyLib;
using SNetwork;

namespace Hikaria.WeaponDataLoader.Patches
{

    [HarmonyPatch]
    internal class Patch_BlockNormalLobby
    {
        [HarmonyPatch(typeof(SNet_SyncManager), nameof(SNet_SyncManager.SetGenerationChecksum))]
        [HarmonyPrefix]
        private static void SNet_SyncManager__SetGenerationChecksum__Prefix(ref ulong checksum)
        {
            checksum += 1UL;
        }

        [HarmonyPatch(typeof(SNet_SessionHub), nameof(SNet_SessionHub.SlaveSendSessionQuestion))]
        [HarmonyPrefix]
        private static void SNet_SessionHub__SlaveSendSessionQuestion__Prefix()
        {
            if (SNet.GameRevision == CellBuildData.GetRevision())
            {
                SNet.GameRevision = 0;
            }
        }

        [HarmonyPatch(typeof(SNet_SessionHub), nameof(SNet_SessionHub.SlaveSendSessionQuestion))]
        [HarmonyPostfix]
        private static void SNet_SessionHub__SlaveSendSessionQuestion__Postfix()
        {
            SNet.GameRevision = CellBuildData.GetRevision();
        }

        [HarmonyPatch(typeof(SNet_SessionHub), nameof(SNet_SessionHub.SlaveWantsToJoin))]
        [HarmonyPrefix]
        private static void SNet_SessionHub__SlaveWantsToJoin__Prefix()
        {
            if (SNet.GameRevision == CellBuildData.GetRevision())
            {
                SNet.GameRevision = 0;
            }
        }

        [HarmonyPatch(typeof(SNet_SessionHub), nameof(SNet_SessionHub.SlaveWantsToJoin))]
        [HarmonyPostfix]
        private static void SNet_SessionHub__SlaveWantsToJoin__Postfix()
        {
            SNet.GameRevision = CellBuildData.GetRevision();
        }
    }
}
#endif
