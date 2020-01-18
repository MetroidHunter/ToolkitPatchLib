using RimWorld;
using System;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace ToolkitPatchLib
{
    public abstract class NormalIncidentHelper<T> : BaseNormalIncidentHelper where T : IncidentWorker
    {
        public NormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef) : base(incidentCategoryDef, incidentDef)
        { }

        public NormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef, bool shouldForceFire = false) : base(incidentCategoryDef, incidentDef, shouldForceFire)
        { }

        public NormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef, IIncidentTarget target, bool shouldForceFire = false) : base(incidentCategoryDef, incidentDef, target, shouldForceFire)
        { }

        public override IncidentWorker GetWorker()
        {
            return Activator.CreateInstance<T>();
        }
    }

    public abstract class NormalIncidentHelper : BaseNormalIncidentHelper
    {
        public NormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef) : base(incidentCategoryDef, incidentDef)
        { }

        public NormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef, bool shouldForceFire = false) : base(incidentCategoryDef, incidentDef, shouldForceFire)
        { }

        public NormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef, IIncidentTarget target, bool shouldForceFire = false) : base(incidentCategoryDef, incidentDef, target, shouldForceFire)
        { }

        public override IncidentWorker GetWorker()
        {
            return (IncidentWorker)Activator.CreateInstance(incidentDef.workerClass);
        }
    }

    public abstract class BaseNormalIncidentHelper : IncidentHelper
    {
        private IncidentCategoryDef incidentCategoryDef;
        private IncidentParms parms = null;
        protected IncidentDef incidentDef;
        protected IncidentWorker worker = null;
        protected IIncidentTarget target = null;

        private bool shouldForceFire = false;

        public BaseNormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef) : this (incidentCategoryDef, incidentDef, Helper.AnyPlayerMap)
        {
        }

        public BaseNormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef, bool shouldForceFire = false) : this(incidentCategoryDef, incidentDef, Helper.AnyPlayerMap, shouldForceFire)
        {
        }

        public BaseNormalIncidentHelper(IncidentCategoryDef incidentCategoryDef, IncidentDef incidentDef, IIncidentTarget target, bool shouldForceFire = false)
        {
            this.incidentDef = incidentDef;
            this.incidentCategoryDef = incidentCategoryDef;
            this.shouldForceFire = true;
            this.target = target;
        }

        public abstract IncidentWorker GetWorker();

        public override bool IsPossible()
        {
            ToolkitPatchLogger.Log(incidentDef.defName, "Checking if possible..");
            worker = GetWorker();
            worker.def = incidentDef;

            if (target != null)
            {
                parms = StorytellerUtility.DefaultParmsNow(incidentCategoryDef, target);
                parms.forced = shouldForceFire;

                bool success = worker.CanFireNow(parms);
                if (!success)
                {
                    WorkerCanFireCheck.CheckDefaultFireNowConditions(worker, parms, incidentDef);
                }
                ToolkitPatchLogger.Log(incidentDef.defName, $"Can fire with params '{parms.ToString()}' on worker {worker.ToString()}? {success}");
                return success;
            }

            ToolkitPatchLogger.ErrorLog(incidentDef.defName, $"Failed to get target. Cannot fire");
            return false;
        }

        public override void TryExecute()
        {
            bool success = worker.TryExecute(parms);
            ToolkitPatchLogger.Log(incidentDef.defName, $"Executed. Result: {success}");
        }

    }
}
