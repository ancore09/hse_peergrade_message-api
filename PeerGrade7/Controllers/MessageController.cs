using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PeerGrade7.Models;
using PeerGrade7.Services;

namespace PeerGrade7.Controllers
{
    [Route("/api/[controller]")]
    public class MessageController : Controller
    {
        private static List<UserMessage> _messages = new List<UserMessage>();
        private static List<User> _users = new List<User>();
        private static Random _random = new Random();

        public MessageController(JsonService service)
        {
            JsonService = service;
            //try
            //{
                _messages = service.GetMessages().ToList();
                _users = service.GetUsers().ToList();
            //}
            //catch (Exception e)
            //{
                //Console.WriteLine("Произошла ошибка чтения из файлов");
                //.WriteLine(e.Message);
            //}
        }
        
        public JsonService JsonService { get; set; }

        /// <summary>
        /// Добавить новое сообщение
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns>400, если отправителя или получателя не существует</returns>
        [HttpPost("AddMessage")]
        public IActionResult AddMessage([FromBody] UserMessage message)
        {
            if (_users.Find(user => user.Email == message.SenderId) == null ||
                _users.Find(user => user.Email == message.ReceiverId) == null)
                return BadRequest();
            
            _messages.Add(message);
            JsonService.UpdateMessages(_messages);
            
            return Ok(_messages.Where(m => m.SenderId == message.SenderId).ToList());
        }
        
        /// <summary>
        /// Добавить пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>400, если пользователь уже зарегистриирован</returns>
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            if (_users.Find(u => u.Email == user.Email) != null)
                return BadRequest();
            
            _users.Add(user);
            JsonService.UpdateUsers(_users);
            
            return Ok(user);
        }
        
        /// <summary>
        /// Генерация пользовотелей и сообщений
        /// </summary>
        /// <returns>Список сгенерированных сообщений</returns>
        [HttpPost("Generate")]
        public IActionResult Generate()
        {
            _users.Clear();
            _messages.Clear();
            for (int i = 0; i < _random.Next(1, 11); i++)
            {
                _users.Add(new User(GetRandomString(_random.Next(3, 5)),
                    GetRandomString(_random.Next(5, 8)) + "@" + 
                    GetRandomString(_random.Next(4, 6)) + "." + 
                    GetRandomString(_random.Next(2, 4))));
            }

            for (int i = 0; i < _random.Next(1, 15); i++)
            {
                _messages.Add(new UserMessage(GetRandomString(_random.Next(3, 5)), 
                    GetRandomString(_random.Next(3, 5)), 
                    _users[_random.Next(_users.Count)].Email, 
                    _users[_random.Next(_users.Count)].Email));
            }

            _users = new List<User>(from s in _users orderby s.Email select s);
            
            JsonService.UpdateUsers(_users);
            JsonService.UpdateMessages(_messages);
            
            return Ok(_messages);
        }

        /// <summary>
        /// Получить сообщения по отправителю
        /// </summary>
        /// <param name="senderId">почта отправителя</param>
        /// <returns>404, если собщения не были найдены</returns>
        [HttpGet]
        [Route("GetMessagesBySender/{senderId:required}")]
        public IActionResult GetMessagesBySender(string senderId)
        {
            var match = _messages.FindAll(message => message.SenderId.Equals(senderId));
            if (match.Count == 0) return NotFound();
            return Ok(match);
        }

        /// <summary>
        /// Получить сообщения по получателю
        /// </summary>
        /// <param name="receiverId">почта получателя</param>
        /// <returns>404, если собщения не были найдены</returns>
        [HttpGet]
        [Route("GetMessagesByReceiver/{receiverId:required}")]
        public IActionResult GetMessagesByReceiver(string receiverId)
        {
            var match = _messages.FindAll(message => message.ReceiverId.Equals(receiverId));
            if (match.Count == 0) return NotFound();
            return Ok(match);
        }
        
        /// <summary>
        /// Получить сообщения по получателю и отправителю
        /// </summary>
        /// <param name="senderId">почта отправителя></param>
        /// <param name="receiverId">почта получателя</param>
        /// <returns>404, если собщения не были найдены</returns>
        [HttpGet]
        [Route("GetMessagesBySenderAndReceiver/{senderId:required}/{receiverId:required}")]
        public IActionResult GetMessagesBySenderAndReceiver(string senderId, string receiverId)
        {
            var match = _messages.FindAll(message =>
                message.SenderId.Equals(senderId) && message.ReceiverId.Equals(receiverId));
            if (match.Count == 0) return NotFound();
            return Ok(match);
        }
        
        /// <summary>
        /// Получить пользователя по почте
        /// </summary>
        /// <param name="email">почта</param>
        /// <returns>404, если пользователь не был найден</returns>
        [HttpGet]
        [Route("GetUserByEmail/{email:required}")]
        public IActionResult GetUserByEmail(string email)
        {
            var match = _users.FindAll(user => user.Email.Equals(email));
            if (match.Count == 0) return NotFound();
            return Ok(match);
        }
        
        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <param name="limit">Максимальное количество в выборке</param>
        /// <param name="offset">Сдвиг относительно начала списка пользовотелей</param>
        /// <returns>400, если данные некорректны</returns>
        [HttpGet]
        [Route("GetAllUsers/{limit}/{offset}")]
        public IActionResult GetAllUsers(int limit = 0, int offset = 0)
        {
            if (limit <= 0 || offset < 0 || offset >= _users.Count) return BadRequest();
            int endIndex = limit <= _users.Count - offset ? offset+limit : _users.Count;
            return Ok(_users.ToArray()[offset..endIndex]);
        }
        
        /// <summary>
        /// Получить все сообщения
        /// </summary>
        /// <returns>200 и список сообщений</returns>
        [HttpGet]
        [Route("GetAllMessages")]
        public IActionResult GetAllMessages()
        {
            return Ok(_messages);
        }
        
        /// <summary>
        /// Генерация случайной строки
        /// </summary>
        /// <param name="length">длина строки</param>
        /// <returns>Модель машины или моторной лодки)</returns>
        static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        
    }
}