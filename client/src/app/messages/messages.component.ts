
import { Pagination, PaginatedResult } from './../_models/pagination';
import { MessagesService } from './../_services/messages.service';
import {Message} from 'src/app/_models/message';

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {


  messages:Message[]=[];
  pageNumber:number=1;
  pageSize:number=5;
  container:string= "Unread";
  loading=false;
  pagination:Pagination
  constructor(private messsageService:MessagesService) { }

  ngOnInit(): void {
    this.loadMessage();
  }


  loadMessage(){
    this.loading=true;
    this.messsageService.getMessageForUSer(this.pageNumber,this.pageSize,this.container).subscribe(
      (response)=>{
        this.messages= response.result;
        this.pagination=response.pagination;
      }
    );
    this.loading=false;
  }

  deleteMessage(idMessage:number){
    this.messsageService.delete(idMessage).subscribe(()=>{
      this.messages.splice(this.messages.findIndex(x => x.id == idMessage),1);
    })
  }
  pageChanged(event:any){

    this.pageNumber=event.page;
    this.loadMessage();
  }

}
