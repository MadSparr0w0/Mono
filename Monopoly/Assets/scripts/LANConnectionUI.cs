using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace MonopolyLAN
{
    public class LANConnectionUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipInput;
        [SerializeField] private ushort port = 7777;

        public void StartHost()
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData("0.0.0.0", port);
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            string ip = string.IsNullOrWhiteSpace(ipInput.text) ? "127.0.0.1" : ipInput.text.Trim();
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, port);
            NetworkManager.Singleton.StartClient();
        }
    }
}
