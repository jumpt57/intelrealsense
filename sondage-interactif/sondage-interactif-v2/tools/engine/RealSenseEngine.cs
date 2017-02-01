using SondageInteractifv2.controllers;
using SondageInteractifv2.tools.gesture;
using System;
using static PXCMHandData;

namespace SondageInteractifv2.tools
{
    /// <summary>
    /// Classe qui permet d'interagir directement 
    /// avec la caméra.
    /// </summary>
    class RealSenseEngine
    {
        private readonly PXCMSession session;
        private readonly PXCMSenseManager senseManager;
        private readonly PXCMHandModule handModule;
        private readonly PXCMHandConfiguration handConfig;
        private readonly AppController AppController;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="AppController">Controller principal</param>
        public RealSenseEngine(AppController AppController)
        {
            this.AppController = AppController;

            this.session = PXCMSession.CreateInstance();
            this.senseManager = session.CreateSenseManager();

            this.senseManager.EnableHand();
            this.handModule = senseManager.QueryHand();

            this.handConfig = handModule.CreateActiveConfiguration();

            this.handConfig.SubscribeGesture(OnFiredGesture);
            this.handConfig.SubscribeAlert(OnFiredAlert);
            
            this.handConfig.EnableGesture(HandDataType.GESTURE_HAND_THUMB_UP);
            this.handConfig.EnableGesture(HandDataType.GESTURE_HAND_THUMB_DOWN);

            this.handConfig.EnableStabilizer(true);

            this.handConfig.EnableAllAlerts();
            this.handConfig.ApplyChanges();

            this.senseManager.Init();

            this.senseManager.StreamFrames(false);            
        }

        /// <summary>
        /// Les événements de la main directement depuis
        /// la caméra, puis sont envoyé au controller
        /// </summary>
        /// <param name="data">Données de la caméra</param>
        void OnFiredGesture(PXCMHandData.GestureData data)
        {
            Console.WriteLine(data.state);
            if (data.state.Equals(GestureStateType.GESTURE_STATE_START))
            {             
                this.AppController.Action(data.name);
            }
                                       
        }

        /// <summary>
        /// Les événements d'information directement depuis
        /// la caméra, puis sont envoyé au controller
        /// </summary>
        /// <param name="data">Informations de la caméra</param>
        void OnFiredAlert(PXCMHandData.AlertData data)
        {
            this.AppController.Alert(data.label);
        }

        /// <summary>
        /// Permet d'arrêter la caméra
        /// </summary>
        public void Shutdown()
        {
            handModule.Dispose();
            handConfig.Dispose();
            senseManager.Close();
            session.Dispose();
        }

        /// <summary>
        /// Permet de mettre en pause la caméra
        /// </summary>
        public void Pause()
        {
            this.senseManager.PauseHand(true);
        }

        /// <summary>
        /// Permet de mettre en pause la caméra
        /// </summary>
        public void Pursue()
        {
            this.senseManager.PauseHand(false);
        }

    }
}
