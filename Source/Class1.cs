using RimWorld;
using Verse;

namespace LiegemanMods
{
    [StaticConstructorOnStartup]
    public static class ArmyOfTheRim
    {
        static ArmyOfTheRim() //our constructor
        {
            Log.Message("Hello World!"); //Outputs "Hello World!" to the dev console.
        }
    }

    public class IncidentWorker_CaravanArrivalQuartermaster : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Log.Message("Trying to execute");
            Faction militaryFaction = Find.FactionManager.FirstFactionOfDef(MilitaryDefOf.ArmyOfTheRim_Military);

            if (militaryFaction == null)
            {
                Log.Message("No military faction, bailing");
                return false;
            }
            Map map = (Map)parms.target;
            var traderKind = DefDatabase<TraderKindDef>.GetNamed("Orbital_Quartermaster");
            Log.Message($"Got traderkind {traderKind.label}");
            var tradeShip = new TradeShip(traderKind, militaryFaction);
            Log.Message("Got tradeship");
            map.passingShipManager.AddShip(tradeShip);
            Log.Message("Added ship");
            tradeShip.GenerateThings();

            var pawns = PawnsFinder.AllMapsWorldAndTemporary_Alive;
            var supplyCount = 0;
            foreach (var pawn in pawns)
            {
                Log.Message($"{pawn.Name}");
                if (pawn.royalty != null)
                {
                    var rank = pawn.royalty.GetCurrentTitleInFaction(militaryFaction);
                    if (rank != null)
                    {
                        Log.Message(rank.Label);
                        supplyCount++;
                    }
                }
            }
            Log.Message($"Supply count = {supplyCount}");
            Thing thing = ThingMaker.MakeThing(ThingDefOf.MealSurvivalPack);
            Log.Message(thing.def.label);
            thing.stackCount = supplyCount * 30 * 2;
            TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(map), map, thing);
            return true;
        }

        // protected bool TryResolveParmsGeneral(IncidentParms parms)
        // {
        //     return true;
        // }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Faction militaryFaction = Find.FactionManager.FirstFactionOfDef(MilitaryDefOf.ArmyOfTheRim_Military);
            Log.Message("Can we fire this event?");
            Log.Message(base.CanFireNowSub(parms) ? "yes can fire" : "no cant fire");
            Log.Message(militaryFaction == null ? "yes found faction" : "no did not find faction");
            // Log.Message(FactionCanBeGroupSource(militaryFaction, (Map)parms.target));
            if (!base.CanFireNowSub(parms) || militaryFaction == null)
            {
                return false;
            }
            return true;
        }

        // protected override float TraderKindCommonality(TraderKindDef traderKind, Map map, Faction faction)
        // {
        //     return traderKind.CalculatedCommonality;
        // }

        // protected override void SendLetter(IncidentParms parms, List<Pawn> pawns, TraderKindDef traderKind)
        // {
        //     TaggedString letterLabel = "LetterLabelQuartermasterArrival".Translate().CapitalizeFirst();
        //     TaggedString letterText = "LetterQuartermasterArrival".Translate(parms.faction.Named("FACTION")).CapitalizeFirst();
        //     SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms, pawns[0]);
        // }
    }

    [DefOf]
    public class MilitaryDefOf
    {
        public static FactionDef ArmyOfTheRim_Military;
        public static TraderKindDef Orbital_Quartermaster;

        static MilitaryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MilitaryDefOf));
        }
    }

}

