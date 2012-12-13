using ZqlChartInterfaces;
namespace ZqlChart
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ZqlChartServiceClient : System.ServiceModel.DuplexClientBase<IZqlChartService>, IZqlChartService
    {

        public ZqlChartServiceClient(System.ServiceModel.InstanceContext callbackInstance) :
            base(callbackInstance)
        {
        }

        public ZqlChartServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) :
            base(callbackInstance, endpointConfigurationName)
        {
        }

        public ZqlChartServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ZqlChartServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ZqlChartServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(callbackInstance, binding, remoteAddress)
        {
        }

        public bool Join(ZqlChartObjects.ChatUser chatUser)
        {
            return base.Channel.Join(chatUser);
        }

        public void Leave(ZqlChartObjects.ChatUser chatUser)
        {
            base.Channel.Leave(chatUser);
        }

        public bool IsUserNameTaken(string strUserName)
        {
            return base.Channel.IsUserNameTaken(strUserName);
        }

        public void SendInkStrokes(System.IO.MemoryStream memoryStream)
        {
            base.Channel.SendInkStrokes(memoryStream);
        }

        public void SendBroadcastMessage(string strUserName, string message)
        {
            base.Channel.SendBroadcastMessage(strUserName, message);
        }


        public bool InitiateCall(string username)
        {
           return base.Channel.InitiateCall(username);
        }



        public void EndCall()
        {
             base.Channel.EndCall();
        }

    }

}

