﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZucchettiCancGiorServiceReference
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://hfwe_fdeletereq.ws.localhost/", ConfigurationName="ZucchettiCancGiorServiceReference.hfwe_fdeletereqWS")]
    public interface hfwe_fdeletereqWS
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunResponse> hfwe_fdeletereq_RunAsync(ZucchettiCancGiorServiceReference.hfwe_fdeletereq_Run request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class hfwe_fdeletereq_Run
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="hfwe_fdeletereq_Run", Namespace="http://hfwe_fdeletereq.ws.localhost/", Order=0)]
        public ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunBody Body;
        
        public hfwe_fdeletereq_Run()
        {
        }
        
        public hfwe_fdeletereq_Run(ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://hfwe_fdeletereq.ws.localhost/")]
    public partial class hfwe_fdeletereq_RunBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string m_UserName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string m_Password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string m_Company;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public double pIdType;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public double pIdReq;
        
        public hfwe_fdeletereq_RunBody()
        {
        }
        
        public hfwe_fdeletereq_RunBody(string m_UserName, string m_Password, string m_Company, double pIdType, double pIdReq)
        {
            this.m_UserName = m_UserName;
            this.m_Password = m_Password;
            this.m_Company = m_Company;
            this.pIdType = pIdType;
            this.pIdReq = pIdReq;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class hfwe_fdeletereq_RunResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="hfwe_fdeletereq_RunResponse", Namespace="http://hfwe_fdeletereq.ws.localhost/", Order=0)]
        public ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunResponseBody Body;
        
        public hfwe_fdeletereq_RunResponse()
        {
        }
        
        public hfwe_fdeletereq_RunResponse(ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://hfwe_fdeletereq.ws.localhost/")]
    public partial class hfwe_fdeletereq_RunResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string @return;
        
        public hfwe_fdeletereq_RunResponseBody()
        {
        }
        
        public hfwe_fdeletereq_RunResponseBody(string @return)
        {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface hfwe_fdeletereqWSChannel : ZucchettiCancGiorServiceReference.hfwe_fdeletereqWS, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class hfwe_fdeletereqWSClient : System.ServiceModel.ClientBase<ZucchettiCancGiorServiceReference.hfwe_fdeletereqWS>, ZucchettiCancGiorServiceReference.hfwe_fdeletereqWS
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public hfwe_fdeletereqWSClient() : 
                base(hfwe_fdeletereqWSClient.GetDefaultBinding(), hfwe_fdeletereqWSClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.hfwe_fdeletereqWSPort.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public hfwe_fdeletereqWSClient(EndpointConfiguration endpointConfiguration) : 
                base(hfwe_fdeletereqWSClient.GetBindingForEndpoint(endpointConfiguration), hfwe_fdeletereqWSClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public hfwe_fdeletereqWSClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(hfwe_fdeletereqWSClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public hfwe_fdeletereqWSClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(hfwe_fdeletereqWSClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public hfwe_fdeletereqWSClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunResponse> ZucchettiCancGiorServiceReference.hfwe_fdeletereqWS.hfwe_fdeletereq_RunAsync(ZucchettiCancGiorServiceReference.hfwe_fdeletereq_Run request)
        {
            return base.Channel.hfwe_fdeletereq_RunAsync(request);
        }
        
        public System.Threading.Tasks.Task<ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunResponse> hfwe_fdeletereq_RunAsync(string m_UserName, string m_Password, string m_Company, double pIdType, double pIdReq)
        {
            ZucchettiCancGiorServiceReference.hfwe_fdeletereq_Run inValue = new ZucchettiCancGiorServiceReference.hfwe_fdeletereq_Run();
            inValue.Body = new ZucchettiCancGiorServiceReference.hfwe_fdeletereq_RunBody();
            inValue.Body.m_UserName = m_UserName;
            inValue.Body.m_Password = m_Password;
            inValue.Body.m_Company = m_Company;
            inValue.Body.pIdType = pIdType;
            inValue.Body.pIdReq = pIdReq;
            return ((ZucchettiCancGiorServiceReference.hfwe_fdeletereqWS)(this)).hfwe_fdeletereq_RunAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.hfwe_fdeletereqWSPort))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.hfwe_fdeletereqWSPort))
            {
                return new System.ServiceModel.EndpointAddress("http://10.16.187.59/workflow/servlet/hfwe_fdeletereq");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return hfwe_fdeletereqWSClient.GetBindingForEndpoint(EndpointConfiguration.hfwe_fdeletereqWSPort);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return hfwe_fdeletereqWSClient.GetEndpointAddress(EndpointConfiguration.hfwe_fdeletereqWSPort);
        }
        
        public enum EndpointConfiguration
        {
            
            hfwe_fdeletereqWSPort,
        }
    }
}