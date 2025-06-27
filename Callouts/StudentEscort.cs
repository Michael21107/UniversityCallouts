﻿using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;

namespace UniversityCallouts.Callouts
{
    [CalloutInfo("Student Escort", CalloutProbability.Low)]
    public class StudentEscort : Callout
    {
        //Private References
        private Vector3 CarSpawn;
        private Vector3 PedSpawn;
        private Vector3 Destination;

        private float PedHeading;
        private float CarHeading;

        private Blip PedBlip;
        private Blip DestinationBlip;
        private Ped Ped;
        private Vehicle Car;

        private bool OnScene = false;
        private bool AtLocation = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            CarSpawn = new Vector3(-1685.103f, 78.41851f, 63.9855f);
            CarHeading = 112.0373f;

            PedSpawn = new Vector3(-1684.251f, 77.36402f, 64.39139f);
            PedHeading = 111.9856f;

            Destination = new Vector3(-1671.686f, 174.0843f, 61.75573f);

            //Set callout position
            this.CalloutPosition = PedSpawn;

            // LSPDFR
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 30f);
            AddMinimumDistanceCheck(20f, CalloutPosition);

            //Create Callout message
            CalloutMessage = "Student Escort";

            //Create friendly name
            FriendlyName = "student escort request";

            //Last Line
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Create Vehicle
            Car = new Vehicle("elegy2", CarSpawn, CarHeading);
            Car.MakePersistent();

            //Create Peds
            Ped = new Ped(PedSpawn, PedHeading);
            Ped.MakePersistent();
            Ped.BlockPermanentEvents = true;
            Ped.Tasks.EnterVehicle(Car, -1);
            Game.LogTrivial("Ped created");

            //Create Blip
            PedBlip = Ped.AttachBlip();
            PedBlip.Sprite = BlipSprite.Friend;
            PedBlip.Color = Color.Blue;

            //Draw Route
            PedBlip.EnableRoute(Color.Orange);

            //Draw Help
            Game.DisplayHelp("A student has requested an escort on campus, please respond and escort the student.");

            //Last Line
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            //First Line
            base.OnCalloutNotAccepted();
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
            if (DestinationBlip.Exists()) { DestinationBlip.Delete(); }
            if (Car.Exists()) { Car.Dismiss(); }
        }

        public override void Process()
        {
            //First Line
            base.Process();

            if (!OnScene & Game.LocalPlayer.Character.Position.DistanceTo(Car) <= 15f)
            {
                OnScene = true;
                PedBlip.DisableRoute();
                Ped.Tasks.LeaveVehicle(Car, LeaveVehicleFlags.None);
                Ped.Tasks.StandStill(-1);
                Game.DisplayHelp("Press the ~y~END~w~ key to end the call at any time.");

                //DRAW BLIP
                DestinationBlip = new Blip(Destination);
                DestinationBlip.Sprite = BlipSprite.Friend;
                DestinationBlip.Color = Color.Blue;

                //Wait 5 Seconds
                GameFiber.Sleep(5000);

                //Instructions
                Game.DisplayNotification("~y~[INFO]~w~ Walk the ped to their destination.");

                //MAKE PED FOLLOW
                Ped.Tasks.FollowToOffsetFromEntity(Game.LocalPlayer.Character, new Vector3(0.25f, 0.25f, 0.25f)); 
            }

            if (OnScene & Game.LocalPlayer.Character.Position.DistanceTo(Destination) <= 5f)
            {
                GameFiber.Sleep(3000);
                Game.DisplayNotification("~y~[INFO]~w~ The student has been escorted safely");
                Ped.Delete();
                Car.Delete();
                Functions.PlayScannerAudio("WE_ARE_CODE FOUR");
                this.End();
            }

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
            {
                GameFiber.Sleep(3000);
                this.End();
            }
        }

        public override void End()
        {
            //First Line
            base.End();
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
            if (DestinationBlip.Exists()) { DestinationBlip.Delete(); }
            if (Car.Exists()) { Car.Dismiss(); }
        }
    }
}

