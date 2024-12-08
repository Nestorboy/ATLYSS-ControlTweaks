using HarmonyLib;
using Mirror;
using UnityEngine;

namespace Nessie.ATLYSS.ControlTweaks.Patches;

public static class WalkPatches
{
    private static ValueChange<bool> _walkToggleState;
    private static bool _isWalkingToggled;

    [HarmonyPatch(typeof(PlayerMove), "<Handle_MovementControl>g__Apply_MovementParams|87_2")]
    private static class WalkPatch
    {
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable InconsistentNaming
        [HarmonyPrefix]
        private static void ApplyWalkModifier(PlayerMove __instance, out Vector3 __state) // ReSharper restore InconsistentNaming
        {
            __state = __instance._worldSpaceInput;
            if (IsWalking()) __instance._worldSpaceInput *= ControlTweaksPlugin.WalkSpeed;
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable InconsistentNaming
        [HarmonyPostfix]
        private static void RevertWalkModifier(PlayerMove __instance, Vector3 __state) // ReSharper restore InconsistentNaming
        {
            __instance._worldSpaceInput = __state;
        }
    }

    [HarmonyPatch(typeof(PlayerVisual), nameof(PlayerVisual.Iterate_AnimationCondition))]
    private static class AnimationPatches
    {
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable InconsistentNaming
        [HarmonyPostfix]
        private static void PatchWalkAnimation(PlayerVisual __instance) // ReSharper restore InconsistentNaming
        {
            if (!NetworkClient.active || !__instance._playerRaceModel || !__instance._visualAnimator)
                return;

            float speed = __instance._playerRaceModel._baseMovementMultiplier * __instance._movementAnimSpeedMod;
            if (IsWalking()) speed *= ControlTweaksPlugin.WalkSpeed;

            __instance._visualAnimator.SetFloat("_movAnimSpd", Mathf.Max(0.75f, speed));
        }
    }

    private static bool IsWalking()
    {
        if (!ControlTweaksPlugin.WalkToggle)
        {
            _walkToggleState.SetValue(false);
            _isWalkingToggled = false;
            return Input.GetKey(ControlTweaksPlugin.WalkKey);
        }

        _walkToggleState.SetValue(Input.GetKey(ControlTweaksPlugin.WalkKey));
        if (_walkToggleState && _walkToggleState.Changed)
        {
            _isWalkingToggled = !_isWalkingToggled;
        }

        return _isWalkingToggled;
    }
}