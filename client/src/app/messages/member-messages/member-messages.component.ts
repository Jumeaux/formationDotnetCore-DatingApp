import { NgForm } from '@angular/forms';
import { MessagesService } from './../../_services/messages.service';
import { Message } from './../../_models/message';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @Input() messages:Message[];
  @Input() username:string;
  @ViewChild('messageForm') messageForm:NgForm;
  contentMessage:string;

  constructor(private messageService:MessagesService) { }

  ngOnInit(): void {
  }

  sendMessage(){
    this.messageService.sendMessages(this.username,this.contentMessage).subscribe(message=>{
      this.messages.push(message);
      this.messageForm.reset();
    });
  }

}
