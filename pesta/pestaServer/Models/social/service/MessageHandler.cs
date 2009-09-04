using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Jayrock.Json;
using pesta.Data;
using Pesta.Engine.protocol;
using Pesta.Engine.social.spi;

namespace pestaServer.Models.social.service
{
    public class MessageHandler : DataRequestHandler
    {
        private readonly IMessagesService service;

        private const string MESSAGE_PATH = "/messages/{userId}+/{msgCollId}/{messageIds}+";

        public MessageHandler()
        {
            Type serviceType = Type.GetType(Pesta.Utilities.PestaSettings.DbServiceName);
            service = serviceType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null) as IMessagesService;
        }


        protected override object handleDelete(RequestItem request)
        {
            request.applyUrlTemplate(MESSAGE_PATH);

            HashSet<UserId> userIds = request.getUsers();
            String msgCollId = request.getParameter("msgCollId");
            HashSet<String> messageIds = request.getListParameter("messageIds");

            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");

            if (msgCollId == null)
            {
                throw new ProtocolException((int)HttpStatusCode.BadRequest,
                    "A message collection is required");
            }

            Preconditions<String>.requireNotEmpty(messageIds, "No message IDs specified");
            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            UserId user = iuserid.Current;
            service.deleteMessages(user, msgCollId, messageIds, request.getToken());
            return new JsonObject();
        }

        protected override object handlePut(RequestItem request)
        {
            request.applyUrlTemplate(MESSAGE_PATH);

            HashSet<UserId> userIds = request.getUsers();
            String msgCollId = request.getParameter("msgCollId");
            HashSet<String> messageIds = request.getListParameter("messageIds");

            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");

            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            UserId user = iuserid.Current;

            if (msgCollId == null) 
            {
                throw new ProtocolException((int)HttpStatusCode.BadRequest,
                        "A message collection is required");
            }

            if (messageIds.Count == 0) 
            {
                // No message IDs specified, this is a PUT to a message collection
                MessageCollection msgCollection = request.getTypedParameter<MessageCollection>("entity");
                if (msgCollection == null) 
                {
                    throw new ProtocolException((int)HttpStatusCode.BadRequest,
                                    "cannot parse message collection");
                }

                // TODO, do more validation.
                service.modifyMessageCollection(user, msgCollection, request.getToken());
                return new JsonObject();
            }

            Preconditions<string>.requireSingular(messageIds, "Only one messageId at a time");

            Message message = request.getTypedParameter<Message>("entity");
            // TODO, do more validation.

            if (message == null || message.id == null) 
            {
                throw new ProtocolException((int)HttpStatusCode.BadRequest,
                    "cannot parse message or missing ID");
            }
            IEnumerator<String> imsgid = messageIds.GetEnumerator();
            imsgid.MoveNext();
            service.modifyMessage(user, msgCollId, imsgid.Current, message, request.getToken());
            return new JsonObject();
        }

        protected override object handlePost(RequestItem request)
        {
            request.applyUrlTemplate(MESSAGE_PATH);

            HashSet<UserId> userIds = request.getUsers();
            String msgCollId = request.getParameter("msgCollId");
            HashSet<String> messageIds = request.getListParameter("messageIds");

            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");

            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            UserId user = iuserid.Current;

            if (msgCollId == null) 
            {
                // Request to create a new message collection
                MessageCollection msgCollection = request.getTypedParameter<MessageCollection>("entity");

                return service.createMessageCollection(user, msgCollection, request.getToken());
            }

            // A message collection has been specified, allow for posting

            Preconditions<string>.requireEmpty(messageIds,"Message IDs not allowed here, use PUT instead");

            Message message = request.getTypedParameter<Message>("entity");
            Preconditions<String>.requireNotEmpty(message.recipients, "No recipients specified");
            service.createMessage(user, request.getAppId(), msgCollId, message,
                                request.getToken());
            return new JsonObject();
        }

        protected override object handleGet(RequestItem request)
        {
            request.applyUrlTemplate(MESSAGE_PATH);

            HashSet<UserId> userIds = request.getUsers();
            String msgCollId = request.getParameter("msgCollId");
            HashSet<String> messageIds = request.getListParameter("messageIds");

            CollectionOptions options = new CollectionOptions(request);

            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");
            
            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            UserId user = iuserid.Current;

            if (msgCollId == null)
            {
                // No message collection specified, return list of message collections
                return service.getMessageCollections(user, MessageCollection.ALL_FIELDS,
                    options, request.getToken());
            }
            // If messageIds are specified return them, otherwise return entries in the given collection.
            return service.getMessages(user, msgCollId,
                Message.ALL_FIELDS, messageIds, options, request.getToken());
        }
    }
}
