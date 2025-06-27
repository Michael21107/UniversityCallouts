using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System;

namespace UniversityCallouts.Callouts
{
    [CalloutInfo("Weapon Violation", CalloutProbability.Low)]
    public class WeaponViolation : Callout
    {
        //Private References
        private Vector3 PedSpawn;
        private float PedHeading;

        private Blip PedBlip;

        private Ped Ped;

        private bool OnScene = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            PedSpawn = new Vector3(-1649.166f, 224.3113f, 60.68501f);
            PedHeading = 22.75008f;

            //Set callout position
            this.CalloutPosition = PedSpawn;

            // LSPDFR
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 30f);
            AddMinimumDistanceCheck(20f, CalloutPosition);

            //Create Callout message
            CalloutMessage = "Reports of an individual with a weapon";

            //Create friendly name
            FriendlyName = "reports of an individual with a weapon";
            
            //Last Line
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Create Peds
            Ped = new Ped(PedSpawn, PedHeading);
            Ped.MakePersistent();
            Ped.BlockPermanentEvents = true;
            Ped.Tasks.Wander();
            Game.LogTrivial("Ped created");

            //Create Blip
            PedBlip = Ped.AttachBlip();
            PedBlip.Sprite = BlipSprite.Friend;
            PedBlip.Color = Color.Orange;

            //Draw Route
            PedBlip.EnableRoute(Color.Orange);

            //Draw Help
            Game.DisplayHelp("There are reports of an individual on campus who has a firearm. Please respond and investigate.");

            //Last Line
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            //First Line
            base.OnCalloutNotAccepted();
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
        }

        public override void Process()
        {
            //First Line
            base.Process();

            if (!OnScene & Game.LocalPlayer.Character.Position.DistanceTo(Ped) <= 15f)
            {
                OnScene = true;
                PedBlip.DisableRoute();
                Game.DisplayHelp("Press the ~y~END~w~ key to end the call.");
                StartScenario();
            }

            if (Functions.IsPedArrested(Ped))
            {
                this.End();
            }

            if (Ped.IsDead)
            {
                this.End();
            }

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
            {
                this.End();
            }
        }

        public override void End()
        {
            //First Line
            base.End();
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
            Functions.PlayScannerAudio("WE_ARE_CODE FOUR");
        }

        public void StartScenario()
        {
            //Pick a random option to happen
            int result = new Random().Next(1, 3);

            if (result == 1)
            {
                //Result 1 will be the ped turns and fires at the officer
                Ped.Inventory.GiveNewWeapon(WeaponHash.APPistol, 999, true);
                Ped.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1, FiringPattern.BurstFirePistol);
            }
            else if (result == 2)
            {
                //Result 2 will be the ped reacts and flees, causing a person
                Ped.Inventory.GiveNewWeapon(WeaponHash.APPistol, 999, true);
                Ped.Tasks.ReactAndFlee(Game.LocalPlayer.Character);
                LHandle Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Ped);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                Functions.SetPursuitCopsCanJoin(Pursuit, true);
            }
            else if (result == 3)
            {
                //Result 3 will be the ped has no weapon and it was a false call
                Ped.Inventory.Weapons.Clear();
            }
        }
    }
}

