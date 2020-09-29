using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Data;
using Data.ResponseTypes;
using Data.ServerCommunication;
using LocalServer;
using Managers.Abstract;
using UnityEngine;

namespace Managers
{
    public class MiBandManager : Singleton<MiBandManager>
    {
        [Tooltip("Should the Server-Window be hidden?")] [SerializeField]
        private bool hideWindow = true;

        /// <summary>
        /// Invoked whenever a heart rate is received from the server.
        /// Will be invoked for all devices -> Filtering needed.
        /// </summary>
        public static event Action<HeartRateResponse> OnHeartRateChange;

        /// <summary>
        /// Invoked whenever a device is disconnected. TODO: Not tested or implemented yet.
        /// </summary>
        public static event Action<bool> OnDeviceConnectionChange;

        /// <summary>
        /// The number of currently connected bands.
        /// </summary>
        public int ConnectedBands => _miBands.Count;


        /// <summary>
        /// The Client-Reference of the Tcp-connection between the local server and this object.
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// Used for sending data to the server.
        /// </summary>
        private ServerWriter _writer;

        /// <summary>
        /// Used for reading data sent from the server.
        /// </summary>
        private ServerReader _reader;

        /// <summary>
        /// A list of connected MiBands. Holds additional data such as <see cref="MiBand.ServerResponseReceived"/>. 
        /// </summary>
        private readonly List<MiBand> _miBands = new List<MiBand>();

        /// <summary>
        /// A time delay after which another attempt to connect to the server will be started.
        /// </summary>
        private const float CONNECTION_RETRY_INTERVAL = 5f;

        /// <summary>
        /// Initialises some needed functionality.
        /// </summary>
        private void OnEnable() => StartCoroutine(Initialize());

        /// <summary>
        /// Stops all measurements (Allows faster reconnecting) and closes the server and sockets.
        /// </summary>
        private void OnDisable()
        {
            foreach (MiBand band in _miBands.Where(band => band.IsMeasuring))
                SendCommand(_miBands.IndexOf(band), Consts.Command.StopMeasurement);

            BackgroundServer.StopServer();
            _client?.Dispose();
        }

        /// <summary>
        /// Connects to a devices with the given device index.
        /// </summary>
        /// <param name="deviceIndex">The device index of the device which to connect to.</param>
        public IEnumerator ConnectToBand(int deviceIndex)
        {
            _miBands.Add(new MiBand {ServerResponseReceived = true});
            MiBand band = _miBands[deviceIndex];
            SendCommand(deviceIndex, Consts.Command.ConnectBand);
            yield return new WaitUntil(() => band.ServerResponseReceived);
            SendCommand(deviceIndex, Consts.Command.AuthenticateBand);
            yield return new WaitUntil(() => band.ServerResponseReceived);
            HeartRateManager.Instance.AddPlayer();
        }

        /// <summary>
        /// Starts the measurement function for the specified device.
        /// </summary>
        /// <param name="deviceIndex">The devices index of the device.</param>
        public IEnumerator StartMeasurement(int deviceIndex)
        {
            MiBand band = _miBands[deviceIndex];
            SendCommand(deviceIndex, Consts.Command.SubscribeToHeartRateChange);
            yield return new WaitUntil(() => band.ServerResponseReceived);
            SendCommand(deviceIndex, Consts.Command.StartMeasurement);
            yield return new WaitUntil(() => band.ServerResponseReceived);
            band.IsMeasuring = true;
        }

        /// <summary>
        /// Starts the server and connects to it.
        /// </summary>
        private IEnumerator Initialize()
        {
            BackgroundServer.StartServer(hideWindow);
            // Short delay for server to start.
            yield return new WaitForSeconds(2);
            yield return ConnectToSever();
        }

        /// <summary>
        /// Tries to connect to the server. After fail, will try after a delay using <see cref="ConnectToServerAfterDelay"/>.
        /// </summary>
        private IEnumerator ConnectToSever()
        {
            try
            {
                _client = new TcpClient("localhost", Consts.ServerData.PORT);
                _writer = new ServerWriter(_client.GetStream());
                _reader = new ServerReader(_client.GetStream());
            }
            catch (SocketException)
            {
                StartCoroutine(ConnectToServerAfterDelay());
            }

            yield return StartCoroutine(ListenForResponse());
        }

        /// <summary>
        /// Tries to connect to the server after a delay. Used after a failed attempt.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ConnectToServerAfterDelay()
        {
            yield return new WaitForSeconds(CONNECTION_RETRY_INTERVAL);
            yield return ConnectToSever();
        }

        /// <summary>
        /// Sends a specified command for the specified device to the server.
        /// </summary>
        /// <param name="deviceIndex">The device index of the device the command is for.</param>
        /// <param name="command">The command for the server.</param>
        private void SendCommand(int deviceIndex, Consts.Command command)
        {
            MiBand band = _miBands[deviceIndex];
            if (!band.ServerResponseReceived)
                return;
            band.ServerResponseReceived = false;
            _writer.WriteServerCommand(new ServerCommand(deviceIndex, command));
        }

        /// <summary>
        /// Listens for responses from the server and invokes events depending on those responses.
        /// </summary>
        /// <exception cref="Exception">Throws exceptions that occured on the server-side.</exception>
        private IEnumerator ListenForResponse()
        {
            Task<string> task = _reader.ReadStringAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            ServerResponse response = ServerResponse.FromJson(task.Result);
            switch (response.Data)
            {
                case SuccessResponse successResponse:
                    MiBand band = _miBands[successResponse.DeviceIndex];
                    band.ServerResponseReceived = true;
                    break;
                case Exception exception:
                    throw exception;
                case HeartRateResponse heartRateResponse:
                    band = _miBands[heartRateResponse.DeviceIndex];
                    // If last two measurements were zero, restart.
                    if (heartRateResponse.IsRepeating)
                    {
                        if (!band.IsRestarting)
                        {
                            band.IsRestarting = true;
                            StartCoroutine(RestartMeasurement(heartRateResponse.DeviceIndex));
                        }
                    }

                    OnHeartRateChange?.Invoke(heartRateResponse);
                    break;
                case DeviceConnectionResponse connectionResponse:
                    OnDeviceConnectionChange?.Invoke(connectionResponse.IsConnected);
                    break;
            }

            yield return ListenForResponse();
        }

        /// <summary>
        /// Sends all necessary commands for restarting the measurement.
        /// </summary>
        /// <param name="deviceIndex">The device index of the device which should restart the measurement.</param>
        private IEnumerator RestartMeasurement(int deviceIndex)
        {
            MiBand band = _miBands[deviceIndex];
            SendCommand(deviceIndex, Consts.Command.StopMeasurement);
            yield return new WaitUntil(() => band.ServerResponseReceived);
            SendCommand(deviceIndex, Consts.Command.StartMeasurement);
            yield return new WaitUntil(() => band.ServerResponseReceived);
            band.IsRestarting = false;
        }

        /// <summary>
        /// Small struct for keeping track of data related to the miband device and it's communication.
        /// </summary>
        private class MiBand
        {
            /// <summary>
            /// Whether a response was received after the last command for this device was sent.
            /// </summary>
            public bool ServerResponseReceived;

            /// <summary>
            /// Whether this device is currently measuring the heart rate.
            /// </summary>
            public bool IsMeasuring;

            /// <summary>
            /// Whether we requested a restart for the measurement.
            /// </summary>
            public bool IsRestarting;
        }
    }
}