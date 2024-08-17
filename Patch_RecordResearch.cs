// Decompiled with JetBrains decompiler
// Type: ResearchHistory.Patch_RecordResearch
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using HarmonyLib;
using RimWorld;
using Verse;

#nullable disable
namespace ResearchHistory
{
  [HarmonyPatch]
  public class Patch_RecordResearch
  {
    [HarmonyPatch(typeof (ResearchManager), "FinishProject")]
    [HarmonyPostfix]
    public static void RecordResearch(ResearchProjectDef proj, Pawn researcher)
    {
      ProjectHistory projectHistory = new ProjectHistory();
      if (researcher != null)
        projectHistory.finalResearcher = researcher.LabelShort;
      if (Find.TickManager.TicksGame == 0)
        projectHistory.startingTech = true;
      Map map = researcher?.MapHeld ?? Current.Game.RandomPlayerHomeMap;
      projectHistory.date = GenDate.DateShortStringAt((long) GenDate.TickGameToAbs(Find.TickManager.TicksGame), Find.WorldGrid.LongLatOf(map.Tile));
      if (ResearchHistory.projectsStarted.ContainsKey((string) proj.LabelCap))
      {
        projectHistory.contributors = ResearchHistory.projectsStarted[(string) proj.LabelCap].contributors;
        if (researcher != null)
          projectHistory.contributors.Remove(researcher.LabelShort);
        ResearchHistory.projectsStarted.Remove((string) proj.LabelCap);
      }
      ResearchHistory.projectsCompleted[(string) proj.LabelCap] = projectHistory;
    }

        private static void RecordContributor(JobDriver_Research __instance)
        {
            ResearchProjectDef currentProj = (ResearchProjectDef)AccessTools.Field(typeof(ResearchManager), "currentProj").GetValue(Find.ResearchManager);
          //Reflextion --> (ResearchProjectDef) AccessTools.Field(typeof(ResearchManager), "currentProj").GetValue(Find.ResearchManager)
      if (currentProj == null || currentProj.IsFinished)
        return;
      Pawn actor = __instance.GetActor();
      string labelCap = (string) currentProj.LabelCap;
      ProjectHistory projectHistory = ResearchHistory.projectsStarted.ContainsKey(labelCap) ? ResearchHistory.projectsStarted[labelCap] : new ProjectHistory();
      projectHistory.contributors.Add(actor.LabelShort);
      ResearchHistory.projectsStarted[labelCap] = projectHistory;
    }

    public static void RecordContributors_PCR(Pawn trackedPawn, ResearchProjectDef myProject)
    {
      if (trackedPawn == null || myProject == null || myProject.IsFinished)
        return;
      string labelCap = (string) myProject.LabelCap;
      ProjectHistory projectHistory = ResearchHistory.projectsStarted.ContainsKey(labelCap) ? ResearchHistory.projectsStarted[labelCap] : new ProjectHistory();
      projectHistory.contributors.Add(trackedPawn.LabelShort);
      ResearchHistory.projectsStarted[labelCap] = projectHistory;
    }
  }
}
