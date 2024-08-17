// Decompiled with JetBrains decompiler
// Type: ResearchHistory.HarmonyPatcher
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

#nullable disable
namespace ResearchHistory
{
  [StaticConstructorOnStartup]
  public class HarmonyPatcher
  {
    static HarmonyPatcher()
    {
      Harmony harmony = new Harmony("cozarkian.researchtimeline");
      harmony.PatchAll();
      if (ModLister.HasActiveModWithName("PawnsChooseResearch"))
        harmony.Patch((MethodBase) AccessTools.Method(GenTypes.GetTypeInAnyAssembly("PawnsChooseResearch.ResearchRecord"), "SetResearchPlan"), postfix: new HarmonyMethod(typeof (Patch_RecordResearch), "RecordContributors_PCR"));
      else
        harmony.Patch((MethodBase) AccessTools.Method(typeof (JobDriver_Research), "MakeNewToils"), new HarmonyMethod(typeof (Patch_RecordResearch), "RecordContributor"));
    }
  }
}
