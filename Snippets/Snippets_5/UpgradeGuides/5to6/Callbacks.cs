﻿namespace Snippets5.UpgradeGuides._5to6
{
    using NServiceBus;

    class Callbacks
    {

        void Simple()
        {
            IBus bus = null;
            #region 5to6-Callbacks

            RequestMessage requestMessage = new RequestMessage();
            bus.Send(requestMessage)
                .Register(ar =>
                {
                    CompletionResult localResult = (CompletionResult) ar.AsyncState;
                    ResponseMessage response = (ResponseMessage) localResult.Messages[0];
                }, null);

            #endregion
        }

        class RequestMessage : IMessage
        {
            
        }

        class ResponseMessage : IMessage
        {
             
        }
    }
}