using System.Threading;
using NUnit.Framework;
using Automobile.Communication.Messaging;
using Automobile.Communication.Tcp;
using Automobile.Communication.UnitTests.Helpers;

namespace Automobile.Communication.UnitTests
{
    [TestFixture]
    public class AutoResponderTests
    {
        public AutoResponderServer Auto { get; set; }

        [SetUp]
        public void Setup()
        {
            Auto = new AutoResponderServer(3001);
        }

        [TearDown]
        public void Cleanup()
        {
            Auto.Close();
        }

        [Test]
        public void ConnectionTest()
        {
            var comm = new TcpClientCommunicator("127.0.0.1", 3001);
            comm.Initialize();
            Assert.IsTrue(comm.Connected);
            comm.Close();
            Assert.IsFalse(comm.Connected);
        }

        [Test]
        public void MessageTest()
        {
            var comm = new TcpClientCommunicator("127.0.0.1", 3001);
            comm.Initialize();
            var resp = comm.SendMessage<Response>(new GenericMessage<string>("Hello World"));
            Assert.NotNull(resp);
            Assert.IsTrue(resp.Success);
        }

        [Test]
        public void CallbackTest()
        {
            var comm = new TcpClientCommunicator("127.0.0.1", 3001);
            comm.Initialize();
            comm.SendMessage<Response>(new GenericMessage<string>("Hello World"), resp =>
                {
                    Assert.NotNull(resp);
                    Assert.IsTrue(resp.Success);                                                                
                });
            Thread.Sleep(100);
        }
    }
}
