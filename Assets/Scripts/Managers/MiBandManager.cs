using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        [SerializeField] private bool hideWindow = true;

        public static event Action<HeartRateResponse> OnHeartRateChange;
        public static event Action<bool> OnDeviceConnectionChange;


        private TcpClient _client;
        private bool _serverResponseReceived = true;

        private const float CONNECTION_RETRY_INTERVAL = 5f;

        private ServerWriter _writer;
        private ServerReader _reader;
        
        private readonly List<MiBand> _miBands = new List<MiBand>();

        private void OnEnable() => StartCoroutine(Initialize());

        private void OnDisable()
        {
            foreach (MiBand band in _miBands.Where(band => band.IsMeasuring))
                SendCommand(_miBands.IndexOf(band), Consts.Command.StopMeasurement);
            
            BackgroundServer.StopServer();
            _client?.Dispose();
        }

        public IEnumerator AddMiBand()
        {
            yield return ConnectToBand(_miBands.Count);
            yield return StartMeasurement(_miBands.Count);
            _miBands.Add(new MiBand{IsMeasuring = true});
        }

        private IEnumerator Initialize()
        {
            //BackgroundServer.StartServer(hideWindow);
            // Short delay for server to start.
            yield return new WaitForSeconds(2);
            yield return ConnectToSever();
        }

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

        private IEnumerator ConnectToServerAfterDelay()
        {
            yield return new WaitForSeconds(CONNECTION_RETRY_INTERVAL);
            yield return ConnectToSever();
        }

        private IEnumerator ConnectToBand(int deviceIndex)
        {
            SendCommand(deviceIndex, Consts.Command.ConnectBand);
            yield return new WaitUntil(() => _serverResponseReceived);
            SendCommand(deviceIndex, Consts.Command.AuthenticateBand);
            yield return new WaitUntil(() => _serverResponseReceived);
        }

        private IEnumerator StartMeasurement(int deviceIndex)
        {
            SendCommand(deviceIndex, Consts.Command.SubscribeToHeartRateChange);
            yield return new WaitUntil(() => _serverResponseReceived);
            SendCommand(deviceIndex, Consts.Command.StartMeasurement);
            yield return new WaitUntil(() => _serverResponseReceived);
        }

        private void SendCommand(int deviceIndex, Consts.Command command)
        {
            if (!_serverResponseReceived)
                return;
            _serverResponseReceived = false;
            _writer.WriteServerCommand(new ServerCommand(deviceIndex, command));
        }

        private IEnumerator ListenForResponse()
        {
            _reader.StartReadTaskAsync();
            yield return new WaitUntil(() => _reader.IsReadTaskCompleted());
            ServerResponse response = ServerResponse.FromJson(_reader.FinishReadTaskAsync());

            switch (response.Data)
            {
                case Exception exception:
                    throw exception;
                case HeartRateResponse heartRateResponse:
                    MiBand band = _miBands[heartRateResponse.DeviceIndex];
                    // If last two measurements were zero, restart.
                    if (heartRateResponse.HeartRate == 0)
                    {
                        if (band.LastHeartRateWasZero)
                            StartCoroutine(RestartMeasurement(heartRateResponse.DeviceIndex));
                        band.LastHeartRateWasZero = true;
                    }
                    OnHeartRateChange?.Invoke(heartRateResponse);
                    break;
                case DeviceConnectionResponse connectionResponse:
                    OnDeviceConnectionChange?.Invoke(connectionResponse.IsConnected);
                    break;
            }

            _serverResponseReceived = true;
            yield return ListenForResponse();
        }

        private IEnumerator RestartMeasurement(int deviceIndex)
        {
            SendCommand(deviceIndex, Consts.Command.StopMeasurement);
            yield return new WaitUntil(() => _serverResponseReceived);
            SendCommand(deviceIndex, Consts.Command.StartMeasurement);
            yield return new WaitUntil(() => _serverResponseReceived);
        }
        
        private struct MiBand
        {
            public bool IsMeasuring;
            public bool LastHeartRateWasZero;
        }
    }
}