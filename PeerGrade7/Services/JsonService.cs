using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using PeerGrade7.Models;

namespace PeerGrade7.Services
{
    public class JsonService
    {
        public JsonService(IWebHostEnvironment webHostEnvironment) {
            WebHostEnvironment = webHostEnvironment; }
        public IWebHostEnvironment WebHostEnvironment { get; } 
        private string usersPath =>
            Path.Combine(WebHostEnvironment.WebRootPath, "users.json");
        private string messagesPath =>
            Path.Combine(WebHostEnvironment.WebRootPath, "messages.json");
        
        public IEnumerable<User> GetUsers() {
            using var fs = new FileStream(usersPath, FileMode.OpenOrCreate, FileAccess.Read);
            var formatter = new DataContractJsonSerializer(typeof(User[]));
            return (User[]) formatter.ReadObject(fs);
        }

        public void UpdateUsers(IEnumerable<User> users)
        {
            var sorted = from s in users orderby s.Email select s;
            try
            {
                using (var sw = new StreamWriter(usersPath))
                {
                    sw.Write(JsonSerializer.Serialize(sorted));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public IEnumerable<UserMessage> GetMessages() {
            using var fs = new FileStream(messagesPath, FileMode.OpenOrCreate, FileAccess.Read);
            var formatter = new DataContractJsonSerializer(typeof(UserMessage[]));
            return (UserMessage[]) formatter.ReadObject(fs);
        }
        
        public void UpdateMessages(IEnumerable<UserMessage> messages)
        {
            try
            {
                using (var sw = new StreamWriter(messagesPath))
                {
                    sw.Write(JsonSerializer.Serialize(messages));
                }
            }
            catch (Exception e)
            {
                
            }
        }
    }
}