using RimWorld;
using System;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers;
using TwitchToolkit.Store;
using Verse;

namespace ToolkitPatchLib
{
    public abstract class WagerIncidentHelper<T> : IncidentHelperVariables where T : IncidentWorker
    {
        public int pointsWager = 0;
        public IIncidentTarget target = null;
        public IncidentWorker worker = null;
        public IncidentParms parms = null;
        private bool separateChannel = false;
        public override Viewer Viewer { get; set; }

        private IncidentCategoryDef categoryDef;
        private IncidentDef incidentDef;
        private RaidStrategyDef raidStratDef = null;
        private PawnsArrivalModeDef pawnArrivalModeDef = null;

        public WagerIncidentHelper(IncidentCategoryDef categoryDef, IncidentDef incidentDef)
        {
            this.categoryDef = categoryDef;
            this.incidentDef = incidentDef;
        }

        public WagerIncidentHelper(IncidentCategoryDef categoryDef, IncidentDef incidentDef, RaidStrategyDef raidStrat, PawnsArrivalModeDef arrivalMode)
        {
            this.categoryDef = categoryDef;
            this.incidentDef = incidentDef;
            raidStratDef = raidStrat;
            pawnArrivalModeDef = arrivalMode;
        }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            ToolkitPatchLogger.Log(incidentDef.defName, "Checking if possible..");
            this.separateChannel = separateChannel;
            this.Viewer = viewer;
            string[] command = message.Split(' ');
            if (command.Length < 3)
            {
                Toolkit.client.SendMessage($"@{viewer.username} syntax is {this.storeIncident.syntax}", separateChannel);
                return false;
            }

            if (!VariablesHelpers.PointsWagerIsValid(
                    command[2],
                    viewer,
                    ref pointsWager,
                    ref storeIncident,
                    separateChannel
                ))
            {
                return false;
            }

            ToolkitPatchLogger.Log(incidentDef.defName, $"Got the points wager of '{pointsWager}' and incident of '{storeIncident.abbreviation}:{storeIncident.defName}'");

            target = Current.Game.AnyPlayerHomeMap;
            if (target == null)
            {
                return false;
            }

            ToolkitPatchLogger.Log(incidentDef.defName, $"Target found '{target.Tile}'");

            parms = StorytellerUtility.DefaultParmsNow(categoryDef, target);
            parms.points = IncidentHelper_PointsHelper.RollProportionalGamePoints(storeIncident, pointsWager, parms.points);
            if (raidStratDef != null)
            {
                parms.raidStrategy = raidStratDef;
            }
            if (pawnArrivalModeDef != null)
            {
                parms.raidArrivalMode = pawnArrivalModeDef;
            }

            worker = Activator.CreateInstance<T>();
            worker.def = incidentDef;

            bool success = worker.CanFireNow(parms);
            if (!success)
            {
                WorkerCanFireCheck.CheckDefaultFireNowConditions(worker, parms, incidentDef);
            }

            ToolkitPatchLogger.Log(incidentDef.defName, $"Can fire with params '{parms.ToString()}' on worker {worker.ToString()}? {success}");

            return success;
        }

        public override void TryExecute()
        {
            bool success = worker.TryExecute(parms);
            ToolkitPatchLogger.Log(incidentDef.defName, $"Executed. Result: {success}");
            if (success)
            {
                Viewer.TakeViewerCoins(pointsWager);
                Viewer.CalculateNewKarma(this.storeIncident.karmaType, pointsWager);
                VariablesHelpers.SendPurchaseMessage($"Starting {storeIncident.label} with {pointsWager} points wagered and {(int)parms.points} points purchased by {Viewer.username}", separateChannel);
                return;
            }
            Toolkit.client.SendMessage($"@{Viewer.username} could not generate parms for {storeIncident.label}.", separateChannel);
        }


    }
}
