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
            var found90 = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found90 && instruction.opcode == OpCodes.Ldc_I4_S && (sbyte)instruction.operand == 90)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_I4, 1000)); // Change operand type to int
                    found90 = true;
                    MelonLogger.Msg("IncreaseFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found90)
                throw new ArgumentException("Cannot find upper limit in Director.IncreaseFrameRate");
            return InjectStep(list);
        }

        private static IEnumerable<CodeInstruction> InjectStep(IEnumerable<CodeInstruction> instructions)
        {
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 5.0)
                {
                    // Replace with logic to choose step based on current frame rate
                    list.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load `this` (Director instance)
                    list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameSpeedPatchIncrease), nameof(GetStep))));
                }
                else
                {
                    list.Add(instruction);
                }
            }
            return list;
        }

        public static double GetStep(Director director)
        {
            double currentFrameRate = director.EngineFrameRate;
            if (currentFrameRate >= 10 && currentFrameRate < 90)
                return 5.0;
            if (currentFrameRate >= 90 && currentFrameRate < 200)
                return 10.0;
            if (currentFrameRate >= 200 && currentFrameRate <= 1000)
                return 100.0;
            return 5.0; // default step
        }
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("DecreaseFrameRate")]
    public static class GameSpeedPatchDecrease
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Decrease(IEnumerable<CodeInstruction> instructions)
        {
            var found5 = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found5 && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 5.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load `this` (Director instance)
                    list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameSpeedPatchDecrease), nameof(GetStep))));
                    found5 = true;
                    MelonLogger.Msg("DecreaseFrameRate patched!");
                }
                else
                {
                    list.Add(instruction);
                }
            }
            if (!found5)
                throw new ArgumentException("Cannot find upper limit in Director.DecreaseFrameRate");
            return list;
        }

        public static double GetStep(Director director)
        {
            double currentFrameRate = director.EngineFrameRate;
            if (currentFrameRate >= 10 && currentFrameRate <= 90)
                return 5.0;
            if (currentFrameRate > 90 && currentFrameRate <= 200)
                return 10.0;
            if (currentFrameRate > 200 && currentFrameRate <= 1000)
                return 100.0;
            return 5.0; // default step
        }
    }

    [HarmonyPatch(typeof(Director))]
    [HarmonyPatch("SetEngineFrameRate")]
    public static class GameSpeedPatchSet
    {
        // New method for inserting IL code
        private static IEnumerable<CodeInstruction> InjectEngineFrameRateLog(IEnumerable<CodeInstruction> instructions)
        {
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                list.Add(instruction);

                // Insert instruction to output to console after saving engineFrameTime
                if (instruction.opcode == OpCodes.Stfld && ((FieldInfo)instruction.operand).Name == "engineFrameTime")
                {
                    // Load reference to the current Director object
                    list.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    // Call EngineFrameRate getter
                    list.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Director), "EngineFrameRate").GetGetMethod()));
                    // Convert to string
                    list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Convert), "ToString", new Type[] { typeof(int) })));
                    // Call MelonLogger.Msg to output to console
                    list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MelonLogger), "Msg", new Type[] { typeof(string) })));
                }
            }
            return list;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;
            var list = new List<CodeInstruction>();
            foreach (var instruction in instructions)
            {
                if (!found && instruction.opcode == OpCodes.Ldc_R8 && (double)instruction.operand == 90.0)
                {
                    list.Add(new CodeInstruction(OpCodes.Ldc_R8, 1000.0));
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

            // Insert code to output engineFrameTime to console
            return InjectEngineFrameRateLog(list);
        }
    }
}