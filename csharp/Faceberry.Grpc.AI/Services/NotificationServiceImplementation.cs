using Faceberry.Grpc.AI.Events;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Threading.Tasks;

namespace Faceberry.Grpc.AI
{
    /// <summary>
    /// Implements <see cref="NotificationService"/> service.
    /// </summary>
    public class NotificationServiceImplementation : NotificationService.NotificationServiceBase
    {
        #region Events

        /// <summary>
        /// Received notification event.
        /// </summary>
        public event NotificationEventHandler OnReceivedNotification;

        #endregion

        #region Methods

        public override Task<Empty> ReceiveFrame(RecognitionResult request, ServerCallContext context)
        {
            try
            {
                var frameBytes = request.Frame.ToByteArray();
                var recognitionUnit = request.RecognitionUnitList;

                OnReceivedNotification?.Invoke(this,
                    new NotificationEventsArgs(frameBytes, recognitionUnit));

                return Task.FromResult(new Empty());
            }
            catch
            {
                // log here
                return Task.FromResult(new Empty());
            }
        }

        #endregion
    }
}