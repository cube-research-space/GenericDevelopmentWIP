﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace CrossPlatformVR
{
    public class RoomMgr : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        #region Photon Callbacks
        /// <summary>
        /// Photon callback advises that local player has left room
        /// Re-load launcher scene
        /// </summary>
        public override void OnLeftRoom()
        {
            // Load 'first' scene (Launcher.unity)
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);        // seen when another player disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);       // seen when another player connects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }

        #endregion
        private void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Missing playerPrefab reference...please set it up in GameObject 'Room Manager", this);
            }
            else
            {
                if (PlayerMgr.LocalPlayerInstance == null)
                {
                    //Debug.LogFormat("Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    Debug.LogFormat("Instantiating {1} from {0}", SceneManagerHelper.ActiveSceneName, PhotonNetwork.LocalPlayer.NickName);

                    // Spawn a character for the local player
                    // This gets synced by using PhotonNetwork.Instantiate
                    //PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);

                    /* PH: Added to test where players instantiate according to their type) */
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("Master Client instantiated by RoomMgr...");
                        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
                    }
                    else
                    {
                        Debug.Log("Remote Client instantiated by RoomMgr...");
                        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(1f, 1f, 0f), Quaternion.identity, 0);
                    }
                }
                else
                {
                    Debug.LogFormat("ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        #region Public and Private Methods
        /// <summary>
        /// Make local player leave the room on Photon server
        /// </summary>
        public static void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// Loads the Hydac Scene after a connection is made to the Photon Server (via the Launcher)
        /// </summary>
        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork: Trying to load a level but we are not the master client");
            }

            Debug.LogFormat("PhotonNetwork: Loading Level {0}", PhotonNetwork.CurrentRoom);
            PhotonNetwork.LoadLevel("QuestVRUIScene");
        }
        #endregion
    }
}
