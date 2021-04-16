using System.Net.Sockets;
using System.Text;

namespace CsgoTranslator.MinimalisticTelnet
{
    internal class TelnetConnection
    {
        private TcpClient _tcpSocket;

        private const int TimeOutMs = 100;
        private const string Hostname = "localhost";
        private const int Port = 2121;

        public bool Connected => _tcpSocket != null && _tcpSocket.Connected;

        public TelnetConnection()
        {
            Connect();
        }

        private bool Connect()
        {
            try
            {
                _tcpSocket = new TcpClient(Hostname, Port);
                return _tcpSocket.Connected;
            }
            catch
            {
                return false;
            }
        }

        public void WriteLine(string cmd)
        {
            var utf8 = Encoding.UTF8;
            var buf = utf8.GetBytes((cmd + "\n").Replace("\0xFF", "\0xFF\0xFF"));

            _tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            if (!_tcpSocket.Connected) return null;
            var sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(TimeOutMs);
            } while (_tcpSocket.Available > 0);
            return sb.ToString();
        }

        private void ParseTelnet(StringBuilder sb)
        {
            while (_tcpSocket.Available > 0)
            {
                var input = _tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1 :
                        break;
                    case (int)Verbs.Iac:
                        // interpret as command
                        var inputVerb = _tcpSocket.GetStream().ReadByte();
                        if (inputVerb == -1) break;
                        switch (inputVerb)
                        {
                            case (int)Verbs.Iac: 
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputVerb);
                                break;
                            case (int)Verbs.Do: 
                            case (int)Verbs.Dont:
                            case (int)Verbs.Will:
                            case (int)Verbs.Wont:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                var inputOption = _tcpSocket.GetStream().ReadByte();
                                if (inputOption == -1) break;
                                
                                _tcpSocket.GetStream().WriteByte((byte)Verbs.Iac);
                                if (inputOption == (int)Options.Sga )
                                    _tcpSocket.GetStream().WriteByte(inputVerb == (int)Verbs.Do ? (byte)Verbs.Will:(byte)Verbs.Do); 
                                else
                                    _tcpSocket.GetStream().WriteByte(inputVerb == (int)Verbs.Do ? (byte)Verbs.Wont : (byte)Verbs.Dont); 
                                _tcpSocket.GetStream().WriteByte((byte)inputOption);
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