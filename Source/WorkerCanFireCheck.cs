using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ToolkitPatchLib
{
    public static class WorkerCanFireCheck
    {
        public static bool CheckDefaultFireNowConditions(IncidentWorker worker, IncidentParms parms, IncidentDef incidentDef)
        {
            if (!worker.def.TargetAllowed(parms.target))
            {
                ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Bad Target");
                return false;
            }

            if (GenDate.DaysPassed < worker.def.earliestDay)
            {
                ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Too early");
                return false;
            }

            if (Find.Storyteller.difficulty.difficulty < worker.def.minDifficulty)
            {
                ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Not Difficult Enough");
                return false;
            }

            if (worker.def.allowedBiomes != null)
            {
                BiomeDef biome = Find.WorldGrid[parms.target.Tile].biome;
                if (!worker.def.allowedBiomes.Contains(biome))
                {
                    ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Wrong Biome. Allowed: [{string.Join(",", worker.def.allowedBiomes.Select(x => x.label).ToArray())}]");
                    return false;
                }
            }

            if (worker.def.minPopulation > 0 && PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Count<Pawn>() < worker.def.minPopulation)
            {
                ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Not Enough Peeps");
                return false;
            }
            Dictionary<IncidentDef, int> lastFireTicks = parms.target.StoryState.lastFireTicks;
            int ticksGame = Find.TickManager.TicksGame;
            int num;
            if (lastFireTicks.TryGetValue(worker.def, out num))
            {
                float num2 = (float)(ticksGame - num) / 60000f;
                if (num2 < worker.def.minRefireDays)
                {
                    ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Too Soon Since Last");
                    return false;
                }
            }
            List<IncidentDef> refireCheckIncidents = worker.def.RefireCheckIncidents;
            if (refireCheckIncidents != null)
            {
                for (int j = 0; j < refireCheckIncidents.Count; j++)
                {
                    if (lastFireTicks.TryGetValue(refireCheckIncidents[j], out num))
                    {
                        float num3 = (float)(ticksGame - num) / 60000f;
                        if (num3 < worker.def.minRefireDays)
                        {
                            ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Too Soon Since last 2");
                            return false;
                        }
                    }
                }
            }
            if (worker.def.minGreatestPopulation > 0 && Find.StoryWatcher.statsRecord.greatestPopulation < worker.def.minGreatestPopulation)
            {
                ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Event Failed: Not Had Enough Peeps Ever");
                return false;
            }

            return true;
        }
    }
}
