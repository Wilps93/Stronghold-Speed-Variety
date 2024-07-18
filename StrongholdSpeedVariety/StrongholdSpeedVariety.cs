using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MelonLoader;

namespace StrongholdSpeedVariety
{
    class StrongholdSpeedVarietyMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("StrongholdSpeedVarietyMod has been loaded!");
        }
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("IncreaseFrameRate")]
    public static class GameSpeedPatchIncrease
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == 90)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_I4, 300)); // Изменяем тип операнда на int
                    found = true;
                    MelonLogger.Msg("IncreaseFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found)
                throw new ArgumentException("Cannot find upper limit in Director.IncreaseFrameRate");
            return list;
        }
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("SetEngineFrameRate")]
    public static class GameSpeedPatchSet
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 90.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_R8, 300.0));
                    found = true;
                    MelonLogger.Msg("SetEngineFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found)
                throw new ArgumentException("Cannot find upper limit in Director.SetEngineFrameRate");
            return list;
        }
    }
}
