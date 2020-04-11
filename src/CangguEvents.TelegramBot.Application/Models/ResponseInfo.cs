using System;

namespace CangguEvents.TelegramBot.Application.Models
{
    public readonly struct ResponseInfo : IEquatable<ResponseInfo>
    {
        public readonly long UserId;
        public readonly int MessageId;
        public readonly string? CallbackQueryId;

        public ResponseInfo(long userId, int messageId, string? callbackQueryId = null)
        {
            UserId = userId;
            MessageId = messageId;
            CallbackQueryId = callbackQueryId;
        }

        public void Deconstruct(out long userId, out int messageId, out string? callbackQueryId)
        {
            userId = UserId;
            messageId = MessageId;
            callbackQueryId = CallbackQueryId;
        }

        public void Deconstruct(out long userId, out int messageId)
        {
            userId = UserId;
            messageId = MessageId;
        }

        public bool Equals(ResponseInfo other) =>
            UserId == other.UserId && MessageId == other.MessageId && CallbackQueryId == other.CallbackQueryId;

        public override bool Equals(object? obj) => obj is ResponseInfo other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(UserId, MessageId, CallbackQueryId);
    }
}