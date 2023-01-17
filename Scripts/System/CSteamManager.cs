using System;
using System.Collections.Generic;
#if STEAMWORKS_WIN
using Steamworks;
#endif
using UnityEngine;

namespace CDK {
    public class CSteamManager
    #if STEAMWORKS_WIN
    : SteamManager 
    #endif
    {
        #if STEAMWORKS_WIN
        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
        #endif
        
        public static event Action OnSteamOverlayOpen {
            add {
                _onSteamOverlayOpen -= value;
                _onSteamOverlayOpen += value;
            }
            remove {
                _onSteamOverlayOpen -= value;
            }
        }
        private static Action _onSteamOverlayOpen;
        
        public static event Action OnSteamOverlayClosed {
            add {
                _onSteamOverlayClosed -= value;
                _onSteamOverlayClosed += value;
            }
            remove {
                _onSteamOverlayClosed -= value;
            }
        }
        private static Action _onSteamOverlayClosed;

        #if STEAMWORKS_WIN
        protected override void Awake() {
            base.Awake();

            if (!Initialized) return;
            
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            m_GameOverlayActivated.Dispose();
        }
        #endif

        public static void DoIfInitialized(Action action) {
            #if !STEAMWORKS_WIN
            return;
            #else
            if (action == null) return;
            if (!Initialized) {
                return;
            }
            action();
            #endif
        }
        
        #if STEAMWORKS_WIN
        private void OnGameOverlayActivated(GameOverlayActivated_t pCallback) {
            if(pCallback.m_bActive != 0) {
                Debug.Log("Steam Overlay has been activated");
                _onSteamOverlayOpen?.Invoke();
            }
            else {
                Debug.Log("Steam Overlay has been closed");
                _onSteamOverlayClosed?.Invoke();
            }
        }
        #endif
        
    }
}