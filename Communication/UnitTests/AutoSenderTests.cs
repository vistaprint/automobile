using System.Threading;
using NUnit.Framework;
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;
using Automobile.Communication.UnitTests.Helpers;

namespace Automobile.Communication.UnitTests
{
    [TestFixture]
    public class AutoSnderTests
    {
        private const string MSG = "hello world";
        public AutoSenderServer Auto { get; set; }

        [SetUp]
        public void Setup()
        {
            Auto = new AutoSenderServer(3001, MSG);
        }

        [TearDown]
        public void Cleanup()
        {
            Auto.Close();
        }

        [Test]
        public void ResponseTest()
        {
            var comm = new TcpClientCommunicator("127.0.0.1", 3001);
            comm.Initialize();
            var msg = comm.WaitForMessage<GenericMessage<string>>();
            Assert.AreEqual(MSG, msg.Contents);
            comm.SendResponse(msg.CreateResponse(true));
            Thread.Sleep(100);
            Assert.IsTrue(Auto.RecivedResponse);
        }
    }
}
