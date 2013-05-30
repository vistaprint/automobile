using System.Threading;
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;

namespace Automobile.Communication.UnitTests.Helpers
{
    public class AutoResponderServer : TcpServerCommunicator
    {
        private readonly Thread _respondThread;

        public AutoResponderServer(int port) : base(port)
        {
            _respondThread = new Thread(AutoRespond);
            _respondThread.Start();
            while (!_respondThread.IsAlive);
        }

        public void AutoRespond()
        {
            try
            {
                Initialize();
                while(true)
                {
                    var msg = WaitForMessage<Message>();
                    SendResponse(msg.CreateResponse(true));
                }
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
            _respondThread.Abort();
            _respondThread.Join();
            base.Close();
        }

    }
}