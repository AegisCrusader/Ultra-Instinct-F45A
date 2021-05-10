using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace UltraInstinctF45A
{
    public class Main : VTOLMOD
    {
        private static GameObject plane;

        public static bool buttonMade = false;
        public static CampaignSave trueSave = null;
        public static string path = "afighter";
        public static MFDPage mfd;
        public static MFD lastMFD = null;
        public static WeaponManager wm;
        public static MFDPortalManager MFDP;
        public static Text text1;
        public static Text text2;
        public static PlayerVehicle selectedVehicle;

        private static bool enableSuperThrust = true;

        private static ModuleEngine engine;
        private static FlightAssist flightAssist;
        private static FuelTank fuelTank;
        private static BlackoutEffect blackoutEffect;
        private static Countermeasure counterMeasure;
        private static AirBrakeController airBrakeController;
        private static GearAnimator gearAnimator;
        private static CanopyAnimator canopyAnimator;
        // This method is run once, when Unity is done initialising this game object
        public override void ModLoaded()
        {
            //This is an event the VTOLAPI calls when the game is done loading a scene

            VTOLAPI.SceneLoaded += SceneLoaded;
            VTOLAPI.MissionReloaded += MissionReloaded;
            base.ModLoaded();

        }
        public IEnumerator main()
        {
            while (VTMapManager.fetch == null || !VTMapManager.fetch.scenarioReady || FlightSceneManager.instance.switchingScene)
            {
                yield return null;
            }
            buttonMade = false;
            Debug.Log("AAAAAAAA");
            wm = VTOLAPI.GetPlayersVehicleGameObject().GetComponent<WeaponManager>();
            flightAssist = VTOLAPI.GetPlayersVehicleGameObject().GetComponentInChildren<FlightAssist>();
            Main.flightAssist.assistEnabled = false;
            {
                Main.MFDP = wm.gameObject.GetComponentInChildren<MFDPortalManager>();
                MFDPStoresManagement MFDP = (MFDPStoresManagement)Main.MFDP.gameObject.GetComponentInChildren<MFDPortalManager>().pages[5];
                Debug.Log("got MFPD");
                GameObject toCopy = null;
                foreach (var resource in Resources.FindObjectsOfTypeAll<VRInteractable>())
                {
                    if (resource.interactableName == "Weapon Bay Door Overrides")
                    {
                        toCopy = resource.gameObject;
                        break;
                    }
                }
                if (toCopy == null)
                {
                    Debug.LogError("To copy is null");
                }
                //THANK YOU TEMPERZ87 FOR BUTTON CODE
                //create button object
                GameObject emptyButton = Instantiate(toCopy, MFDP.displayObj.gameObject.transform);
                //get transform object (position)
                RectTransform rt = emptyButton.GetComponent<RectTransform>();
                Main.text1 = emptyButton.gameObject.GetComponentInChildren<Text>();
                rt.localPosition = new Vector3(rt.localPosition.x - 50, rt.localPosition.y, rt.localPosition.z);
                rt.localScale = new Vector3(rt.localScale.x * 0.85f, rt.localScale.y * 0.85f, rt.localScale.z * 0.85f);
                rt.GetComponentInChildren<Image>().color = Color.black;
                Debug.Log("instantiate");
                VRInteractable interactable = emptyButton.GetComponentInChildren<VRInteractable>();
                Debug.Log("vr interactable");
                Text text = emptyButton.GetComponentInChildren<Text>();
                Debug.Log("text");
                text.text = "FltAst";
                Debug.Log("flightassist");
                interactable.OnInteract = new UnityEvent();
                Debug.Log("new UnityEvent()");
                interactable.interactableName = "Toggle Flight Assist";
                Debug.Log("toggle Flight Assist");
                interactable.OnInteract.AddListener(new UnityAction(() =>
                {
                    Main.flightAssist.assistEnabled = !Main.flightAssist.assistEnabled;
                    Main.MFDP.PlayInputSound();
                    Main.text1.color = Main.flightAssist.assistEnabled ? Color.green : Color.red;
                }));
                Debug.Log("listener");
                Main.text1.color = Main.flightAssist.assistEnabled ? Color.green : Color.red;

                GameObject emptyButton2 = Instantiate(toCopy, MFDP.displayObj.gameObject.transform);
                RectTransform rt2 = emptyButton2.GetComponent<RectTransform>();
                rt2.localPosition = new Vector3(rt2.localPosition.x - 85, rt.localPosition.y, rt.localPosition.z);
                rt2.localScale = new Vector3(rt2.localScale.x * 0.85f, rt2.localScale.y * 0.85f, rt2.localScale.z * 0.85f);
                rt2.GetComponentInChildren<Image>().color = Color.black;
                Debug.Log("instantiate");
                VRInteractable interactable2 = emptyButton2.GetComponentInChildren<VRInteractable>();
                Debug.Log("vr interactable");
                text2 = emptyButton2.GetComponentInChildren<Text>();
                Debug.Log("text");
                text2.text = "Thrust";
                Debug.Log("Thrust Event");
                interactable2.OnInteract = new UnityEvent();
                Debug.Log("new UnityEvent()");
                interactable2.interactableName = "Toggle Ultra Thrust";
                Debug.Log("Create Thrust Listener");
                interactable2.OnInteract.AddListener(new UnityAction(() =>
                {
                    enableSuperThrust = !enableSuperThrust;
                    Main.MFDP.PlayInputSound();
                    Main.text2.color = enableSuperThrust ? Color.green : Color.red;
                }));
               Main.text2.color = enableSuperThrust ? Color.green : Color.red;
            
        }
        }

        //This method is called every frame by Unity. Here you'll probably put most of your code
        void Update()
        {
            try
            {
                if (plane == null)
                {
                    plane = VTOLAPI.GetPlayersVehicleGameObject();
                    return;
                }
                if (VTOLAPI.GetPlayersVehicleEnum().Equals(VTOLVehicles.F45A))
                {
                    UltraInstinct(plane);
                }
            }
            catch (Exception)
            {
            }
        }

        //This method is like update but it's framerate independent. This means it gets called at a set time interval instead of every frame. This is useful for physics calculations
        void FixedUpdate()
        {
            
        }

        //This function is called every time a scene is loaded. this behaviour is defined in Awake().
        private void SceneLoaded(VTOLScenes scene)
        {
            //If you want something to happen in only one (or more) scenes, this is where you define it.

            //For example, lets say you're making a mod which only does something in the ready room and the loading scene. This is how your code could look:
            switch (scene)
            {
                case VTOLScenes.ReadyRoom:
                    //Add your ready room code here
                    break;
                case VTOLScenes.LoadingScene:
                    //Add your loading scene code here
                    
                    break;
                case VTOLScenes.CustomMapBase:

                case VTOLScenes.Akutan:
                    StartCoroutine(main());
                    break;
            }
        }
        private void MissionReloaded()
        {
            StartCoroutine(main());

        }
        /**
         * Creates button without listener
         * 
         */
        public static VRInteractable createMFDButton()
        {



            return null;
        }
        public static void UltraInstinct(GameObject vehicle)
        {
            AeroController aeroController = vehicle.GetComponentInChildren<AeroController>();
            aeroController.flapSpeed = 100;
            OverGWarning overGWarning = vehicle.GetComponentInChildren<OverGWarning>();
            overGWarning.maxG = 9999999f;
            foreach (var item in aeroController.controlSurfaces)
            {
                item.actuatorSpeed = 999999f;
            }
            EngineMods(vehicle);
            FuelTankMods(vehicle);
            //FlightAssistMods(vehicle);
            GForceMods(vehicle);
            //CounterMeasureMods(vehicle);
            AirBrakeMods(vehicle);
            SpeedyAnimationMods(vehicle);
        }


        public static void EngineMods(GameObject vehicle)
        {
            engine = GameObject.Find("f45a-engine").GetComponent<ModuleEngine>();
            engine.startupTime = 0.1f;
            engine.fuelDrain = 0f;

            if (enableSuperThrust)
            {
                engine.maxThrust = 4000f;
                engine.idleThrottle = 0.01f;
                engine.abThrustMult = 50f;
            }
            else
            {
                engine.maxThrust = 208f;
                engine.idleThrottle = 0.03f;
                engine.abThrustMult = 1.12233f;
            }
            
        }
        public static void FlightAssistMods(GameObject vehicle)
        {
            flightAssist= vehicle.GetComponentInChildren<FlightAssist>();
            //flightAssist.gLimit = 99999999f;


            /*if (glimit)
            {
                Debug.Log("enabled GLIM");
                flightAssist.pitchGLimiter = true;
                flightAssist.gLimit = 10f;
                flightAssist.assistEnabled = true;

            }
            else
            {
                Debug.Log("disabled GLIM");

                flightAssist.pitchGLimiter = false;
                flightAssist.gLimit = 99999999f;
                flightAssist.assistEnabled = false ;
            }*/
            
            
        }
        public static void FuelTankMods(GameObject vehicle)
        {
            fuelTank = vehicle.GetComponentInChildren<FuelTank>();
            

        }
        public static void GForceMods(GameObject vehicle)
        {
            blackoutEffect = vehicle.GetComponentInChildren<BlackoutEffect>();
            blackoutEffect.gTolerance = 99999999f;
            blackoutEffect.maxGAccum = 99999999f;
            blackoutEffect.negGTolerance = 99999999f;
            blackoutEffect.instantaneousGDeath = 99999999f;
        }
        public static void CounterMeasureMods(GameObject vehicle)
        {
            counterMeasure = vehicle.GetComponentInChildren<Countermeasure>();
            counterMeasure.SetCount(120);


        }
        public static void AirBrakeMods(GameObject vehicle)
        {
            airBrakeController = vehicle.GetComponentInChildren<AirBrakeController>();
            airBrakeController.brakeDragRate = 0.1f;

        }
        public static void SpeedyAnimationMods(GameObject vehicle)
        {
            gearAnimator = vehicle.GetComponentInChildren<GearAnimator>();
            canopyAnimator = vehicle.GetComponentInChildren<CanopyAnimator>();
            gearAnimator.transitionTime = 0.5f;
            canopyAnimator.animTime = 0.5f;



        }
        public static void Mods(GameObject vehicle)
        {

        }
    }
}