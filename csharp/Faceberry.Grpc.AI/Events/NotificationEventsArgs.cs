using System;

namespace Faceberry.Grpc.AI.Events
{
    /// <summary>
    /// Defines notification event arguments.
    /// </summary>
    public class NotificationEventsArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes notification event arguments.
        /// </summary>
        /// <param name="frameBytes">Frame bytes</param>
        /// <param name="inferenceResult">Inference result</param>
        public NotificationEventsArgs(byte[] frameBytes, IdentificationUnitList identificationUnitList)
        {
            FrameBytes = frameBytes;
            IdentificationUnitList = identificationUnitList;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets frame bytes.
        /// </summary>
        public byte[] FrameBytes { get; private set; }

        /// <summary>
        /// Gets inference result.
        /// </summary>
        public IdentificationUnitList IdentificationUnitList { get; private set; }

        #endregion
    }

    /// <summary>
    /// Delegate.
    /// </summary>
    /// <param name="e">Event args</param>
    public delegate void NotificationEventHandler(object sender, NotificationEventsArgs e);
}
