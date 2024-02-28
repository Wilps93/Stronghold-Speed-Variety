using System;
using System.Collections.Generic;
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