// minimalistic telnet implementation
// conceived by Tom Janssens on 2007/06/06  for codeproject
//
// http://www.corebvba.be



using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace MinimalisticTelnet
{
    enum Verbs {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    enum Options
    {
        SGA = 3
    }

    class TelnetConnection
    {
        private TcpClient _tcpSocket;

        private readonly int _timeOutMs = 100;
        private readonly string _hostname = "localhost";
        private readonly int _port = 2121;

        public bool Connected
        {
            get {
                if (this._tcpSocket == null) return false;
                return this._tcpSocket.Connected;
                }
        }

        public TelnetConnection()
        {
            this.Connect();
        }

        private bool Connect()
        {
            try
            {
                this._tcpSocket = new TcpClient(_hostname, _port);
                return this._tcpSocket.Connected;
            }
            catch
            {
                return false;
            }
        }

        public void WriteLine(string cmd)
        {
            var utf8 = Encoding.UTF8;
            byte[] buf = utf8.GetBytes((cmd + "\n").Replace("\0xFF", "\0xFF\0xFF"));

            _tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            if (!_tcpSocket.Connected) return null;
            StringBuilder sb=new StringBuilder();
            do
            {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(_timeOutMs);
            } while (_tcpSocket.Available > 0);
            return sb.ToString();
        }

        private void ParseTelnet(StringBuilder sb)
        {
            while (_tcpSocket.Available > 0)
            {
                int input = _tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1 :
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = _tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC: 
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO: 
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = _tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                _tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA )
                                    _tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL:(byte)Verbs.DO); 
                                else
                                    _tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT); 
                                _tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append( (char)input );
                        break;
                }
            }
        }
    }
}
