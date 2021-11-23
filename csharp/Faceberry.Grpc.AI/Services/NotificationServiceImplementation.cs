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

        /// <summary>
        /// Returns bool value.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="context">Context</param>
        /// <returns></returns>
        public override Task<BoolValue> ReceiveFrame(IdentificationResult request, ServerCallContext context)
        {
            try
            {
                var frameBytes = request.Frame.ToByteArray();
                var recognitionUnit = request.RecognitionUnitList;

                OnReceivedNotification?.Invoke(this, 
                    new NotificationEventsArgs(frameBytes, recognitionUnit));

                return Task.FromResult(new BoolValue {Value = true});
            }
            catch
            {
                return Task.FromResult(new BoolValue {Value = false});
            }
        }

        #endregion
    }
}