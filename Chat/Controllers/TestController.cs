using EchoServer.Application;

namespace Chat.Controllers
{
    [SocketControllerAttribute]
    public class TestController
    {
        [Action(0)]
        public void Test(string fff)
        {

        }
    }
}
