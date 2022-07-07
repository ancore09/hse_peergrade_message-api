using System.Runtime.Serialization;

namespace PeerGrade7.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Email { get; set; }

        public User(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }
    }
}