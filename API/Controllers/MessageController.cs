using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController:BaseApicontroller
    {
        private readonly IUserRepository _userRepo;
        private readonly IMessageRepository _messageRepo;
        private IMapper _mapper;

        public MessagesController(IUserRepository userRepo, IMessageRepository messageRepo, IMapper mapper){

            _userRepo=userRepo;
            _messageRepo=messageRepo;
            _mapper=mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> AddMessage(CreateMessageDto createMessage)
        {
            var senderUsername= User.GetUsername();

            if (senderUsername == createMessage.RecipientUsername.ToLower()) return BadRequest("you can't send message to yourself");

            var sender= await _userRepo.GetUserByUsernameAsync(senderUsername);
            var recipient= await _userRepo.GetUserByUsernameAsync(createMessage.RecipientUsername);

            if (recipient==null) return NotFound();

            var message= new Message
            {
               Sender=sender,
               Recipient=recipient,
               SenderUsername=senderUsername,
               RecipientUsername=recipient.UserName,
               Content=createMessage.Content 
            };

            _messageRepo.AddMessage(message);
            if( await _messageRepo.SaveAllAsync())  return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");

           
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageFoUser([FromQuery] MessageParams messsageParams)
        {
                messsageParams.Username= User.GetUsername();
                var messages = await _messageRepo.GetMessageForUser(messsageParams);

                Response.AddPageHeader(messages.CurrentPage,messages.PageSize, messages.TotalCount, messages.TotalPages);

                return messages;
        }


        [HttpGet("thread/{username}")]
        public async Task <ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {   
            var currentUSer= User.GetUsername(); 

            return Ok(await _messageRepo.GetMessageThread(currentUSer,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteMessage(int id){

            var username= User.GetUsername();
            var message= await _messageRepo.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername!= username) 
                return Unauthorized("you're not authorize to delete this message");

            if(message.SenderUsername == username) message.SenderDelete=true;

            if(message.RecipientUsername == username) message.RecipientDelete=true;

            if( message.SenderDelete && message.RecipientDelete) _messageRepo.DeleteMessage(message);

            if(await _messageRepo.SaveAllAsync()) return Ok();

            return BadRequest("failed to delete message");


        }
    }
}