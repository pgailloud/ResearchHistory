// Decompiled with JetBrains decompiler
// Type: ResearchHistory.Window_ResearchHistory
// Assembly: ResearchHistory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 538D0BD4-7E6B-47F4-AD54-96E249EB7830
// Assembly location: F:\Jeux\Steam\steamapps\workshop\content\294100\2688698217\1.3\Assemblies\ResearchHistory.dll

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

#nullable disable
namespace ResearchHistory
{
  public class Window_ResearchHistory : Window
  {
    private static Listing_Standard listing = new Listing_Standard();
    public static HashSet<string> researchers = new HashSet<string>()
    {
      (string) "ResTime_AllResearchers".Translate(),
      (string) "ResTime_Starting".Translate()
    };
    private static HashSet<string> finProjects = new HashSet<string>();
    private static HashSet<string> startedProjects = new HashSet<string>();
    private static List<string> indivProjects = new List<string>();
    public static string selPawn = (string) "ResTime_AllResearchers".Translate();
    private static Vector2 scrollPos = Vector2.zero;

    public Window_ResearchHistory()
    {
      this.layer = WindowLayer.Dialog;
      this.forcePause = false;
      this.absorbInputAroundWindow = false;
      this.soundAppear = SoundDefOf.DialogBoxAppear;
      this.soundClose = SoundDefOf.Click;
      this.doCloseButton = true;
      this.doCloseX = true;
      this.preventCameraMotion = false;
      this.onlyOneOfTypeAllowed = true;
      this.resizeable = true;
      this.closeOnCancel = true;
      this.closeOnAccept = false;
      this.draggable = true;
    }

    public static void ResetHistory()
    {
      Window_ResearchHistory.researchers.Clear();
      Window_ResearchHistory.researchers.Add((string) "ResTime_AllResearchers".Translate());
      Window_ResearchHistory.researchers.Add((string) "ResTime_Starting".Translate());
      Window_ResearchHistory.FindResearchers();
      Window_ResearchHistory.finProjects.Clear();
      Window_ResearchHistory.startedProjects.Clear();
      Window_ResearchHistory.indivProjects.Clear();
      Window_ResearchHistory.selPawn = (string) "ResTime_AllResearchers".Translate();
      Window_ResearchHistory.scrollPos = Vector2.zero;
    }

    public override Vector2 InitialSize => new Vector2(800f, 600f);

    public override void PreOpen()
    {
      Log.Message("ResTime: Starting Pre-Open");
      base.PreOpen();
      if (ResearchHistory.projectsCompleted == null || ResearchHistory.projectsCompleted.Keys.Count == 0)
      {
        Log.Error("[ResTime] Research history doesn't exist");
      }
      else
      {
        Window_ResearchHistory.FindResearchers();
        if (!(Window_ResearchHistory.selPawn != (string) "ResTime_AllResearchers".Translate()))
          return;
        this.SetIndivProjects();
      }
    }

    public static void FindResearchers()
    {
      foreach (string key in ResearchHistory.projectsCompleted.Keys)
      {
        if (!Window_ResearchHistory.finProjects.Contains(key))
        {
          ProjectHistory projectHistory = ResearchHistory.projectsCompleted[key];
          Window_ResearchHistory.finProjects.Add(key);
          if (projectHistory.finalResearcher != null)
            Window_ResearchHistory.researchers.Add(projectHistory.finalResearcher);
          if (projectHistory.contributors != null)
            Window_ResearchHistory.researchers.AddRange<string>(projectHistory.contributors);
        }
      }
      Window_ResearchHistory.startedProjects.Clear();
      if (ResearchHistory.projectsStarted.Count <= 0)
        return;
      Window_ResearchHistory.startedProjects = ResearchHistory.projectsStarted.Keys.ToHashSet<string>();
      foreach (string key in ResearchHistory.projectsStarted.Keys)
        Window_ResearchHistory.researchers.AddRange<string>(ResearchHistory.projectsStarted[key].contributors);
    }

    public IEnumerable<FloatMenuOption> GetFloatMenuOptions()
    {
      foreach (string researcher in Window_ResearchHistory.researchers)
      {
        string pawn = researcher;
        yield return new FloatMenuOption(pawn, (Action) (() =>
        {
          Window_ResearchHistory.selPawn = pawn;
          this.SetIndivProjects();
        }));
      }
      Log.Message("ResTime: No errors populating pawn list");
    }

    public void SetIndivProjects()
    {
      Window_ResearchHistory.indivProjects.Clear();
      if (Window_ResearchHistory.selPawn == (string) "ResTime_AllResearchers".Translate())
        return;
      if (Window_ResearchHistory.selPawn == (string) "ResTime_Starting".Translate())
      {
        foreach (string finProject in Window_ResearchHistory.finProjects)
        {
          if (ResearchHistory.projectsCompleted[finProject].startingTech)
            Window_ResearchHistory.indivProjects.Add(finProject);
        }
      }
      else
      {
        foreach (string finProject in Window_ResearchHistory.finProjects)
        {
          ProjectHistory projectHistory = ResearchHistory.projectsCompleted[finProject];
          if (projectHistory.finalResearcher != null || projectHistory.contributors.Contains(Window_ResearchHistory.selPawn))
            Window_ResearchHistory.indivProjects.Add(finProject);
        }
        foreach (string startedProject in Window_ResearchHistory.startedProjects)
        {
          if (ResearchHistory.projectsStarted[startedProject].contributors.Contains(Window_ResearchHistory.selPawn))
            Window_ResearchHistory.indivProjects.Add(startedProject);
        }
      }
    }

    public override void DoWindowContents(Rect inRect)
    {
      Window_ResearchHistory.listing.Begin(inRect);
      Text.Font = GameFont.Medium;
      Window_ResearchHistory.listing.Label("ResTime_ResearchHistory".Translate());
      Window_ResearchHistory.listing.GapLine();
      Text.Font = GameFont.Small;
      if (Widgets.ButtonText(new Rect(0.0f, Window_ResearchHistory.listing.CurHeight, 120f, 30f), Window_ResearchHistory.selPawn, true, true, true))
        Find.WindowStack.Add((Window) new FloatMenu(this.GetFloatMenuOptions().ToList<FloatMenuOption>()));
      Window_ResearchHistory.listing.Gap(30f);
      Window_ResearchHistory.listing.GapLine();
      Window_ResearchHistory.listing.End();
      Rect outRect = new Rect(0.0f, 80f, inRect.width - 10f, inRect.height - 120f);
      Rect rect = new Rect(0.0f, 0.0f, outRect.width - 30f, Mathf.Max((float) (Window_ResearchHistory.finProjects.Count + Window_ResearchHistory.startedProjects.Count) * 24f, outRect.height));
      Widgets.BeginScrollView(outRect, ref Window_ResearchHistory.scrollPos, rect);
      Window_ResearchHistory.listing.Begin(rect);
      if (Window_ResearchHistory.selPawn == (string) "ResTime_AllResearchers".Translate())
        Window_ResearchHistory.AllResearchers(rect);
      else
        Window_ResearchHistory.IndivResearcher(rect);
      Text.Anchor = TextAnchor.UpperLeft;
      Window_ResearchHistory.listing.End();
      Widgets.EndScrollView();
      Window_ResearchHistory.listing.End();
    }

    public static void AllResearchers(Rect viewRect)
    {
      Window_ResearchHistory.listing.Indent(15f);
      Window_ResearchHistory.listing.Label("ResTime_Date".Translate());
      Window_ResearchHistory.listing.Outdent(15f);
      foreach (string finProject in Window_ResearchHistory.finProjects)
        Window_ResearchHistory.listing.Label(ResearchHistory.projectsCompleted[finProject].date);
      foreach (string startedProject in Window_ResearchHistory.startedProjects)
        Window_ResearchHistory.listing.Label("ResTime_Started".Translate());
      Widgets.DrawLineVertical(viewRect.width * 0.2f, 0.0f, viewRect.height);
      Window_ResearchHistory.listing.Begin(new Rect(viewRect.width * 0.2f, 0.0f, viewRect.width * 0.3f, viewRect.height));
      Text.Anchor = TextAnchor.MiddleCenter;
      Window_ResearchHistory.listing.Label("ResTime_Project".Translate());
      Text.Anchor = TextAnchor.MiddleLeft;
      Window_ResearchHistory.listing.Indent(10f);
      foreach (string finProject in Window_ResearchHistory.finProjects)
        Window_ResearchHistory.listing.Label(finProject);
      foreach (string startedProject in Window_ResearchHistory.startedProjects)
        Window_ResearchHistory.listing.Label(startedProject);
      Window_ResearchHistory.listing.Outdent(10f);
      Window_ResearchHistory.listing.End();
      Widgets.DrawLineVertical(viewRect.width * 0.5f, 0.0f, viewRect.height);
      Window_ResearchHistory.listing.Begin(new Rect(viewRect.width * 0.5f, 0.0f, viewRect.width * 0.2f, viewRect.height));
      Text.Anchor = TextAnchor.MiddleCenter;
      Window_ResearchHistory.listing.Label("ResTime_Final".Translate());
      Text.Anchor = TextAnchor.MiddleLeft;
      Window_ResearchHistory.listing.Indent(10f);
      foreach (string finProject in Window_ResearchHistory.finProjects)
      {
        string finalResearcher = ResearchHistory.projectsCompleted[finProject].finalResearcher;
        if (ResearchHistory.projectsCompleted[finProject].startingTech)
          Window_ResearchHistory.listing.Label("ResTime_Starting".Translate());
        else if (finalResearcher != null)
          Window_ResearchHistory.listing.Label(finalResearcher);
        else
          Window_ResearchHistory.listing.Label(" ");
      }
      foreach (string startedProject in Window_ResearchHistory.startedProjects)
        Window_ResearchHistory.listing.Label("ResTime_Ongoing".Translate());
      Window_ResearchHistory.listing.Outdent(10f);
      Window_ResearchHistory.listing.End();
      Widgets.DrawLineVertical(viewRect.width * 0.7f, 0.0f, viewRect.height);
      Window_ResearchHistory.listing.Begin(new Rect(viewRect.width * 0.7f, 0.0f, viewRect.width * 0.3f, viewRect.height));
      Text.Anchor = TextAnchor.MiddleCenter;
      Window_ResearchHistory.listing.Label("ResTime_Contributors".Translate());
      Text.Anchor = TextAnchor.MiddleLeft;
      Window_ResearchHistory.listing.Indent(10f);
      foreach (string finProject in Window_ResearchHistory.finProjects)
      {
        List<string> list = ResearchHistory.projectsCompleted[finProject].contributors.ToList<string>();
        if (list == null || list.Count == 0)
          Window_ResearchHistory.listing.Label(" ");
        else if (list.Count == 1)
        {
          Window_ResearchHistory.listing.Label(list[0]);
        }
        else
        {
          string label = list[0];
          for (int index = 1; index < list.Count; ++index)
            label = label + ", " + list[index];
          Window_ResearchHistory.listing.Label(label);
        }
      }
      foreach (string startedProject in Window_ResearchHistory.startedProjects)
      {
        List<string> list = ResearchHistory.projectsStarted[startedProject].contributors.ToList<string>();
        if (list == null || list.Count == 0)
          Window_ResearchHistory.listing.Label(" ");
        else if (list.Count == 1)
        {
          Window_ResearchHistory.listing.Label(list[0]);
        }
        else
        {
          string label = list[0];
          for (int index = 1; index < list.Count; ++index)
            label = label + ", " + list[index];
          Window_ResearchHistory.listing.Label(label);
        }
      }
      Window_ResearchHistory.listing.Outdent(10f);
    }

    public static void IndivResearcher(Rect viewRect)
    {
      Window_ResearchHistory.listing.Indent(15f);
      Window_ResearchHistory.listing.Label("ResTime_Date".Translate());
      Window_ResearchHistory.listing.Outdent(15f);
      foreach (string indivProject in Window_ResearchHistory.indivProjects)
      {
        if (ResearchHistory.projectsStarted.ContainsKey(indivProject))
          Window_ResearchHistory.listing.Label("ResTime_Ongoing".Translate());
        else
          Window_ResearchHistory.listing.Label(ResearchHistory.projectsCompleted[indivProject].date);
      }
      Widgets.DrawLineVertical(viewRect.width * 0.2f, 0.0f, viewRect.height);
      Window_ResearchHistory.listing.Begin(new Rect(viewRect.width * 0.2f, 0.0f, viewRect.width * 0.3f, viewRect.height));
      Text.Anchor = TextAnchor.MiddleCenter;
      Window_ResearchHistory.listing.Label("ResTime_Project".Translate());
      Text.Anchor = TextAnchor.MiddleLeft;
      Window_ResearchHistory.listing.Indent(10f);
      foreach (string indivProject in Window_ResearchHistory.indivProjects)
        Window_ResearchHistory.listing.Label(indivProject);
      Window_ResearchHistory.listing.Outdent(10f);
      Window_ResearchHistory.listing.End();
      Widgets.DrawLineVertical(viewRect.width * 0.5f, 0.0f, viewRect.height);
      Window_ResearchHistory.listing.Begin(new Rect(viewRect.width * 0.5f, 0.0f, viewRect.width * 0.2f, viewRect.height));
      Text.Anchor = TextAnchor.MiddleCenter;
      Window_ResearchHistory.listing.Label("ResTime_Role".Translate());
      Text.Anchor = TextAnchor.MiddleLeft;
      Window_ResearchHistory.listing.Indent(10f);
      for (int index = 0; index < Window_ResearchHistory.indivProjects.Count; ++index)
      {
        string label = (string) "ResTime_Contributed".Translate();
        string indivProject = Window_ResearchHistory.indivProjects[index];
        if (ResearchHistory.projectsCompleted.ContainsKey(indivProject))
        {
          ProjectHistory projectHistory = ResearchHistory.projectsCompleted[indivProject];
          if (projectHistory.startingTech)
            label = " ";
          else if (projectHistory.finalResearcher != null && projectHistory.finalResearcher == Window_ResearchHistory.selPawn)
            label = (string) "ResTime_Completed".Translate();
        }
        Window_ResearchHistory.listing.Label(label);
      }
      Window_ResearchHistory.listing.Outdent(10f);
      Window_ResearchHistory.listing.End();
      Widgets.DrawLineVertical(viewRect.width * 0.7f, 0.0f, viewRect.height);
      Window_ResearchHistory.listing.Begin(new Rect(viewRect.width * 0.7f, 0.0f, viewRect.width * 0.3f, viewRect.height));
      Text.Anchor = TextAnchor.MiddleCenter;
      Window_ResearchHistory.listing.Label("ResTime_Contributors".Translate());
      Text.Anchor = TextAnchor.MiddleLeft;
      Window_ResearchHistory.listing.Indent(10f);
      for (int index1 = 0; index1 < Window_ResearchHistory.indivProjects.Count; ++index1)
      {
        string label = " ";
        string indivProject = Window_ResearchHistory.indivProjects[index1];
        List<string> stringList = new List<string>();
        if (ResearchHistory.projectsCompleted.ContainsKey(indivProject))
        {
          ProjectHistory projectHistory = ResearchHistory.projectsCompleted[indivProject];
          if (!projectHistory.startingTech)
          {
            if (projectHistory.finalResearcher != null)
              stringList.Add(projectHistory.finalResearcher);
            stringList.AddRange((IEnumerable<string>) projectHistory.contributors);
          }
        }
        else
          stringList.AddRange((IEnumerable<string>) ResearchHistory.projectsStarted[indivProject].contributors);
        if (stringList.Contains(Window_ResearchHistory.selPawn))
          stringList.Remove(Window_ResearchHistory.selPawn);
        int index2 = 1;
        if (stringList.Count > 0)
          label = stringList[0];
        for (; index2 < stringList.Count; ++index2)
          label = label + ", " + stringList[index2];
        Window_ResearchHistory.listing.Label(label);
      }
      Window_ResearchHistory.listing.Outdent(10f);
    }
  }
}
