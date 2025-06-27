using LSPD_First_Response.Mod.API;
using Rage;

namespace UniversityCallouts
{
    public class Main : Plugin
    {
        private static string modname = "University Callouts";
        private static string version = "1.5";
        private static string author = "Abel Gaming";

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Settings.LoadSettings();
        }

        public override void Finally()
        {
            Game.LogTrivial("Plugin has been cleaned up.");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                //Display Notification
                Game.DisplayNotification("~b~" + modname + " ~w~has loaded successfully");

                //Log
                Game.LogTrivial(modname + " by " + author + " " + version + " loaded.");

                //Register Callouts
                RegisterCallouts();
            }
        }

        private static void RegisterCallouts()
        {
            //Register Callouts Here
            if (Settings.UnderageDrinking) { Functions.RegisterCallout(typeof(Callouts.UnderageDrinking)); }
            if (Settings.StudentsFighting) { Functions.RegisterCallout(typeof(Callouts.StudentsFighting)); }
            if (Settings.NoiseComplaint) { Functions.RegisterCallout(typeof(Callouts.NoiseComplaint)); }
            if (Settings.StudentEscort) { Functions.RegisterCallout(typeof(Callouts.StudentEscort)); }
            if (Settings.Stalking) { Functions.RegisterCallout(typeof(Callouts.StalkingReport)); }
            if (Settings.WeaponViolation) { Functions.RegisterCallout(typeof(Callouts.WeaponViolation)); }
        }
    }
}

/* 1.5 Changes
 * Added .INI file to enable/disable callouts
 * 
*/