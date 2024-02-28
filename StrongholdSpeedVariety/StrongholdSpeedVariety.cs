using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MelonLoader;

// Max speed which I found to be stable is 127. 128 and above will cause that increasing speed will stop working.
// Possibly because of some value type, but I cannot find it.
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
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (found is false && instruction.opcode == OpCodes.Ldc_I4_S && Int32.Parse(instruction.operand.ToString()) == 90)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_I4_S,125));
                    found = true;
                    MelonLogger.Msg("IncreaseFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find upper limit in Director.IncreaseFrameRate");
            return list;
        }
    }
    
    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("SetEngineFrameRate")]
    public static class GameSpeedPatchSet
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (found is false && instruction.opcode == OpCodes.Ldc_R8 && Double.Parse(instruction.operand.ToString()) == 90.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_R8, 125.0));
                    found = true;
                    MelonLogger.Msg("SetEngineFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find upper limit in Director.SetEngineFrameRate");
            return list;
        }
    }
}