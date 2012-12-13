using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LumiSoft.Media.Wave;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;

namespace ZqlChart
{

    public class CallManager
    {

        private WaveIn _waveIn;

        private WaveOut _waveOut;

        private IPEndPoint _serverEndPoint;

        private Thread _playSound;

        private UdpClient _socket;

        public CallManager(IPEndPoint serverEndpoint)
        {
            _serverEndPoint = serverEndpoint;
        }

        public void Start()
        {
            if (_waveIn != null || _waveOut != null)
            {
                throw new Exception("Call is allready started");
            }

            int waveInDevice = (Int32)Application.UserAppDataRegistry.GetValue("WaveIn", 0);
            int waveOutDevice = (Int32)Application.UserAppDataRegistry.GetValue("WaveOut", 0);

            _socket = new UdpClient(0); // opens a random available port on all interfaces

            _waveIn = new WaveIn(WaveIn.Devices[waveInDevice], 8000, 16, 1, 400);
            _waveIn.BufferFull += new BufferFullHandler(_waveIn_BufferFull);
            _waveIn.Start();

            _waveOut = new WaveOut(WaveOut.Devices[waveOutDevice], 8000, 16, 1);

            _playSound = new Thread(new ThreadStart(playSound));
            _playSound.IsBackground = true;
            _playSound.Start();

        }

        private void playSound()
        {
            try
            {
                while (true)
                {
                    lock (_socket)
                    {
                        if (_socket.Available != 0)
                        {
                            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                            byte[] received = _socket.Receive(ref endpoint);
                            // todo: add codec

                            _waveOut.Play(received, 0, received.Length);
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch
            {
                this.Stop();
            }
        }

        void _waveIn_BufferFull(byte[] buffer)
        {
            lock (_socket)
            {
                //todo: add codec
                _socket.Send(buffer, buffer.Length, _serverEndPoint);
            }
        }
        public void Stop()
        {
            if (_waveIn != null)
            {
                _waveIn.Dispose();
            }

            if (_waveOut != null)
            {
                _waveOut.Dispose();
            }

            if (_playSound.IsAlive)
            {
                _playSound.Abort();
            }

            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }
    }
}
