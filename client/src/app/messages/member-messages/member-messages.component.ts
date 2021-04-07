import { NgForm } from '@angular/forms';
import { MessagesService } from './../../_services/messages.service';
import { Message } from './../../_models/message';
import { Component, OnInit, Input, ViewChild, ChangeDetectionStrategy } from '@angular/core';
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @Input() messages:Message[];
  @Input() username:string;
  @ViewChild('messageForm') messageForm:NgForm;
  contentMessage:string;

  loading=false;

  constructor(public messageService:MessagesService) { }

  ngOnInit(): void {
  }

  sendMessage(){
    this.loading=true;
    this.messageService.sendMessages(this.username,this.contentMessage).then(()=>{
      this.messageForm.reset();
    }).finally(()=>this.loading=false);
  }

}
