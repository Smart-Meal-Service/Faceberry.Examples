using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Net;
using System.Net.Sockets;

namespace Faceberry.Grpc.AI.Extensions
{
    /// <summary>
    /// Static class rovides extension methods for the GRPC services.
    /// </summary>
    public static class GrpcExtensions
    {
        /// <summary>
        /// Creates gRPC server.
        /// </summary>
        /// <param name="grpc">Notification service implementation</param>
        /// <param name="port">Port</param>
        /// <returns>Server</returns>
        public static Server CreateServer(NotificationServiceImplementation grpc, int port = 5080)
        {
            var credentials = ServerCredentials.Insecure;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var host = GetDefaultHostAddress();
            var serverPort = new ServerPort(host, port, credentials);

            var _grpcServer = new Server()
            {
                Ports = { serverPort },
                Services = {
                    NotificationService.BindService(grpc)
                },
            };

            return _grpcServer;
        }

        /// <summary>
        /// Creates gRRC client channel.
        /// </summary>
        /// <param name="port">Port</param>
        /// <returns>GrpcChannel</returns>
        public static GrpcChannel CreateClientChannel(int port = 5070)
        {
            var host = GetDefaultHostAddress();
            var clientPort = port;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var address = $"http://{host}:{clientPort}";

            var grpcChannelOptions = new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure
            };

            return GrpcChannel.ForAddress(address, grpcChannelOptions);
        }

        /// <summary>
        /// Return host address.
        /// </summary>
        /// <returns>Host address</returns>
        public static string GetDefaultHostAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system.");
        }
    }
}
