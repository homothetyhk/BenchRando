using System.Reflection;

namespace BenchRando
{
    public static class BenchNames
    {
        public static string[] ToArray()
        {
            return typeof(BenchNames).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral)
            .Select(f => (string)f.GetRawConstantValue())
            .ToArray();
        }

        public const string Bench_Dirtmouth = "Bench-Dirtmouth";
        public const string Bench_Mato = "Bench-Mato";
        public const string Bench_Crossroads_Hot_Springs = "Bench-Crossroads_Hot_Springs";
        public const string Bench_Crossroads_Stag = "Bench-Crossroads_Stag";
        public const string Bench_Salubra = "Bench-Salubra";
        public const string Bench_Ancestral_Mound = "Bench-Ancestral_Mound";
        public const string Bench_Black_Egg_Temple = "Bench-Black_Egg_Temple";
        public const string Bench_Waterfall = "Bench-Waterfall";
        public const string Bench_Stone_Sanctuary = "Bench-Stone_Sanctuary";
        public const string Bench_Greenpath_Toll = "Bench-Greenpath_Toll";
        public const string Bench_Greenpath_Stag = "Bench-Greenpath_Stag";
        public const string Bench_Lake_of_Unn = "Bench-Lake_of_Unn";
        public const string Bench_Sheo = "Bench-Sheo";
        public const string Bench_Archives = "Bench-Archives";
        public const string Bench_Queens_Station = "Bench-Queen's_Station";
        public const string Bench_Leg_Eater = "Bench-Leg_Eater";
        public const string Bench_Bretta = "Bench-Bretta";
        public const string Bench_Mantis_Village = "Bench-Mantis_Village";
        public const string Bench_Quirrel = "Bench-Quirrel";
        public const string Bench_City_Toll = "Bench-City_Toll";
        public const string Bench_City_Storerooms = "Bench-City_Storerooms";
        public const string Bench_Watchers_Spire = "Bench-Watcher's_Spire";
        public const string Bench_Kings_Station = "Bench-King's_Station";
        public const string Bench_Pleasure_House = "Bench-Pleasure_House";
        public const string Bench_Waterways = "Bench-Waterways";
        public const string Bench_Godhome_Atrium = "Bench-Godhome_Atrium";
        public const string Bench_Godhome_Roof = "Bench-Godhome_Roof";
        public const string Bench_Hall_of_Gods = "Bench-Hall_of_Gods";
        public const string Bench_Deepnest_Hot_Springs = "Bench-Deepnest_Hot_Springs";
        public const string Bench_Failed_Tramway = "Bench-Failed_Tramway";
        public const string Bench_Beasts_Den = "Bench-Beast's_Den";
        public const string Bench_Basin_Toll = "Bench-Basin_Toll";
        public const string Bench_Hidden_Station = "Bench-Hidden_Station";
        public const string Bench_Oro = "Bench-Oro";
        public const string Bench_Camp = "Bench-Camp";
        public const string Bench_Colosseum = "Bench-Colosseum";
        public const string Bench_Hive = "Bench-Hive";
        public const string Bench_Peak_Dark_Room = "Bench-Peak_Dark_Room";
        public const string Bench_Crystal_Guardian = "Bench-Crystal_Guardian";
        public const string Bench_Grounds_Stag = "Bench-Grounds_Stag";
        public const string Bench_Grey_Mourner = "Bench-Grey_Mourner";
        public const string Bench_Gardens_Cornifer = "Bench-Gardens_Cornifer";
        public const string Bench_Gardens_Toll = "Bench-Gardens_Toll";
        public const string Bench_Gardens_Stag = "Bench-Gardens_Stag";
        public const string Bench_Palace_Entrance = "Bench-Palace_Entrance";
        public const string Bench_Palace_Atrium = "Bench-Palace_Atrium";
        public const string Bench_Palace_Balcony = "Bench-Palace_Balcony";
        public const string Bench_Upper_Tram = "Bench-Upper_Tram";
        public const string Bench_Lower_Tram = "Bench-Lower_Tram";

        public const string Bench_Stag_Nest = "Bench-Stag_Nest";
        public const string Bench_Cliffs_Overhang = "Bench-Cliffs_Overhang";
        public const string Bench_Jonis_Repose = "Bench-Joni's_Repose";
        public const string Bench_Nightmare_Lantern = "Bench-Nightmare_Lantern";
        public const string Bench_Blasted_Plains = "Bench-Blasted_Plains";
        public const string Bench_Baldur_Cavern = "Bench-Baldur_Cavern";
        public const string Bench_Crossroads_Center = "Bench-Crossroads_Center";
        public const string Bench_Myla = "Bench-Myla";
        public const string Bench_Grubfather = "Bench-Grubfather";
        public const string Bench_Crossroads_Elevator = "Bench-Crossroads_Elevator";
        public const string Bench_Fungal_Road = "Bench-Fungal_Road";
        public const string Bench_Pilgrims_Start = "Bench-Pilgrim's_Start";
        public const string Bench_Canyon_Depths = "Bench-Canyon_Depths";
        public const string Bench_Canyons_End = "Bench-Canyon's_End";
        public const string Bench_Overgrown_Atrium = "Bench-Overgrown_Atrium";
        public const string Bench_Overgrown_Mound = "Bench-Overgrown_Mound";
        public const string Bench_Millibelle = "Bench-Millibelle";
        public const string Bench_Fungal_Core = "Bench-Fungal_Core";
        public const string Bench_Fungal_Tower = "Bench-Fungal_Tower";
        public const string Bench_Cloths_Ambush = "Bench-Cloth's_Ambush";
        public const string Bench_Pilgrims_End = "Bench-Pilgrim's_End";
        public const string Bench_Mantis_Hub = "Bench-Mantis_Hub";
        public const string Bench_Prophets_Gate = "Bench-Prophet's_Gate";
        public const string Bench_City_Entrance = "Bench-City_Entrance";
        public const string Bench_Inner_Sanctum = "Bench-Inner_Sanctum";
        public const string Bench_Outer_Sanctum = "Bench-Outer_Sanctum";
        public const string Bench_City_Fountain = "Bench-City_Fountain";
        public const string Bench_Nailsmith = "Bench-Nailsmith";
        public const string Bench_Flooded_Stag = "Bench-Flooded_Stag";
        public const string Bench_Zotes_Skyway = "Bench-Zote's_Skyway";
        public const string Bench_Watchers_Skyway = "Bench-Watcher's_Skyway";
        public const string Bench_Tower_of_Love = "Bench-Tower_of_Love";
        public const string Bench_Hive_Hideaway = "Bench-Hive_Hideaway";
        public const string Bench_Pure_Altar = "Bench-Pure_Altar";
        public const string Bench_Lurkers_Overlook = "Bench-Lurker's_Overlook";
        public const string Bench_Edge_Summit = "Bench-Edge_Summit";
        public const string Bench_Bardoon = "Bench-Bardoon";
        public const string Bench_Bardoons_Tail = "Bench-Bardoon's_Tail";
        public const string Bench_West_Lake_Shore = "Bench-West_Lake_Shore";
        public const string Bench_East_Lake_Shore = "Bench-East_Lake_Shore";
        public const string Bench_Spirits_Glade = "Bench-Spirits'_Glade";
        public const string Bench_Crypts = "Bench-Crypts";
        public const string Bench_Nosks_Lair = "Bench-Nosk's_Lair";
        public const string Bench_Weavers_Den = "Bench-Weaver's_Den";
        public const string Bench_Distant_Reservoir = "Bench-Distant_Reservoir";
        public const string Bench_Deepnest_Gate = "Bench-Deepnest_Gate";
        public const string Bench_Distant_Stag = "Bench-Distant_Stag";
        public const string Bench_Deepnest_Maze = "Bench-Deepnest_Maze";
        public const string Bench_Abyss_Workshop = "Bench-Abyss_Workshop";
        public const string Bench_Far_Basin = "Bench-Far_Basin";
        public const string Bench_Basin_Hub = "Bench-Basin_Hub";
        public const string Bench_Palace_Grounds = "Bench-Palace_Grounds";
        public const string Bench_Traitors_Grave = "Bench-Traitor's_Grave";
        public const string Bench_Fort_Loodle = "Bench-Fort_Loodle";
        public const string Bench_Far_Gardens = "Bench-Far_Gardens";
        public const string Bench_Dark_Gardens = "Bench-Dark_Gardens";
        public const string Bench_Gardens_Atrium = "Bench-Gardens_Atrium";
        public const string Bench_Peak_Entrance = "Bench-Peak_Entrance";
        public const string Bench_Crown_Ascent = "Bench-Crown_Ascent";
        public const string Bench_Crystallized_Mound = "Bench-Crystallized_Mound";
        public const string Bench_Crusher_Refuge = "Bench-Crusher_Refuge";
        public const string Bench_Western_Peak = "Bench-Western_Peak";
        public const string Bench_Quirrel_Peak = "Bench-Quirrel_Peak";
        public const string Bench_Peak_Ravine = "Bench-Peak_Ravine";
        public const string Bench_Unns_Chamber = "Bench-Unn's_Chamber";
        public const string Bench_Gulka_Gulley = "Bench-Gulka_Gulley";
        public const string Bench_Hunters_Hideout = "Bench-Hunter's_Hideout";
        public const string Bench_Durandas_Trial = "Bench-Duranda's_Trial";
        public const string Bench_Greenpath_Entrance = "Bench-Greenpath_Entrance";
        public const string Bench_Defenders_Repose = "Bench-Defender's_Repose";
        public const string Bench_Hermits_Approach = "Bench-Hermit's_Approach";
        public const string Bench_Waterways_Entrance = "Bench-Waterways_Entrance";
        public const string Bench_Ismas_Grove = "Bench-Isma's_Grove";
        public const string Bench_Acid_Sluice_East = "Bench-Acid_Sluice_East";
        public const string Bench_Acid_Sluice_West = "Bench-Acid_Sluice_West";
        public const string Bench_Fort_Flukefey = "Bench-Fort_Flukefey";
        public const string Bench_Destroyed_Tram = "Bench-Destroyed_Tram";
        public const string Bench_Kingsmould_Duelist = "Bench-Kingsmould_Duelist";
        public const string Bench_Palace_West = "Bench-Palace_West";
        public const string Bench_Sawblade_Choir = "Bench-Sawblade_Choir";
        public const string Bench_Palace_East = "Bench-Palace_East";
        public const string Bench_Thorny_Respite = "Bench-Thorny_Respite";
        public const string Bench_Palace_Workshop = "Bench-Palace_Workshop";
        public const string Bench_Throne_Approach = "Bench-Throne_Approach";
        public const string Bench_Path_Midpoint = "Bench-Path_Midpoint";
    }
}
