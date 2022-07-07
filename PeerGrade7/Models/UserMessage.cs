using System.Runtime.Serialization;

namespace PeerGrade7.Models
{
    [DataContract]
    public class UserMessage
    {
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string SenderId { get; set; }
        [DataMember]
        public string ReceiverId { get; set; }

        public UserMessage(string subject, string message, string senderId, string receiverId)
        {
            Subject = subject;
            Message = message;
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}