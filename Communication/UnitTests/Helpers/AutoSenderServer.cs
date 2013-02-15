using System.Threading;
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;

namespace Automobile.Communication.UnitTests.Helpers
{
    public class AutoSenderServer: TcpServerCommunicator
    {
        private readonly Thread _sendThread;
        private readonly string _msg;

        public AutoSenderServer(int port, string msg) : base(port)
        {
            _msg = msg;
            _sendThread = new Thread(AutoSend);
            _sendThread.Start();
            while (!_sendThread.IsAlive);
        }

        public void AutoSend()
        {
            try
            {
                Initialize();
                var resp = SendMessage<IResponse>(new GenericMessage<string>(_msg));
                RecivedResponse = resp != null;
            }
            catch (ThreadAbortException)
            {
                // clear the abort so it doesn't get triggered again
                Thread.ResetAbort();
                return;
            }

        }

        public override void Close()
        {
            _sendThread.Abort();
            _sendThread.Join();
            base.Close();
        }

        public bool RecivedResponse { get; private set; }
    }
}