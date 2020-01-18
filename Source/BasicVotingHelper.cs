using RimWorld;
using System;
using TwitchToolkit.Votes;

namespace ToolkitPatchLib
{
    public abstract class BasicVotingHelper<T> : BaseVotingHelper where T : IncidentWorker
    {
        public BasicVotingHelper(IncidentCategoryDef cat, IncidentDef incidentDef, bool shouldForceFire = false) : base(cat, incidentDef, shouldForceFire)
        {
        }

        public override IncidentWorker GetWorker()
        {
            return Activator.CreateInstance<T>();
        }
        
    }

    public abstract class BasicVotingHelper : BaseVotingHelper
    {
        public BasicVotingHelper(IncidentCategoryDef cat, IncidentDef incidentDef, bool shouldForceFire = false) : base(cat, incidentDef, shouldForceFire)
        {
        }

        public override IncidentWorker GetWorker()
        {
            return (IncidentWorker)Activator.CreateInstance(incidentDef.workerClass);
        }

    }

    public abstract class BaseVotingHelper : VotingHelper 
    {
        private IncidentCategoryDef category;
        private IncidentWorker worker = null;
        private IncidentParms parms = null;
        private bool shouldForceFire = false;
        protected IncidentDef incidentDef;

        public BaseVotingHelper(IncidentCategoryDef cat, IncidentDef incidentDef, bool shouldForceFire = false)
        {
            category = cat;
            this.incidentDef = incidentDef;
            this.shouldForceFire = shouldForceFire;
        }

        public abstract IncidentWorker GetWorker();

        public override bool IsPossible()
        {
            ToolkitPatchLogger.Log(incidentDef.defName, "Checking if possible..");
            worker = GetWorker();
            worker.def = incidentDef;

            float points = StorytellerUtility.DefaultThreatPointsNow(target);
            parms = StorytellerUtility.DefaultParmsNow(category, target);
            parms.forced = shouldForceFire;
            parms.points = points;

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
        }
    }
}
