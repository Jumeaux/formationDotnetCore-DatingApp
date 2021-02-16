import {Message}  from 'src/app/_models/message';
import { BASE_URL } from './../back-end/url';
import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelpers';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {

  constructor(private http:HttpClient) { }


  getMessageForUSer(pageNumber:number, pageSize:number, container:string){

    let param= getPaginationHeaders(pageNumber,pageSize);
    param=param.append("Container", container);

    return getPaginatedResult<Message[]>(BASE_URL+'messages', param, this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(BASE_URL+"messages/thread/"+username);
  }

  sendMessages(usernamne:string, content:string){
    return this.http.post<Message>(BASE_URL +'messages',{recipientUsername:usernamne,content})
  }

  delete(id:number){

    return this.http.delete(BASE_URL+'messages/'+id);
  }
}
