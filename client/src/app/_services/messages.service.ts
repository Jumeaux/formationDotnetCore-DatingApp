import { BusyService } from './busy.service';
import { take } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { User } from './../_models/user';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import {Message}  from 'src/app/_models/message';
import { BASE_URL, HUBS_URL } from './../back-end/url';
import { HttpClient } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelpers';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {

  hubConnection : HubConnection;
  private messageThreadSource= new  BehaviorSubject<Message[]>([]);
  public messageThread$= this.messageThreadSource.asObservable();
  constructor(private http:HttpClient, private busyService:BusyService) { }


  createHubConnection(user:User, otherUser:string)
  { 
    this.busyService.busy();
      this.hubConnection= new HubConnectionBuilder()
        .withUrl(HUBS_URL+'messages?user='+otherUser, {
          accessTokenFactory:()=>user.token
        }).withAutomaticReconnect().build();
        
        
      this.hubConnection.start()
      .catch(error=> console.log(error))
      .finally(()=>this.busyService.idle());
    
      this.hubConnection.on('ReceivedMessagesThread',messages=>{
          this.messageThreadSource.next(messages);
        });
      this.hubConnection.on("NewMessage",message=>{
        this.messageThread$.pipe(take(1)).subscribe(messages=>{
          this.messageThreadSource.next([... messages,message])
        })
      })

      this.hubConnection.on("UpdateGroupe",(group:Group)=>{

        if (group.connections.some(x =>x.username===otherUser)) {
          this.messageThread$.pipe(take(1)).subscribe(messages=>{
            messages.forEach(msg=>{

              if(!msg.dateRead){
                msg.dateRead= new Date(Date.now());
              }
            })
            this.messageThreadSource.next([... messages])
          })
        }
      })
  }

  public stopHubConnection()
  {
    if(this.hubConnection){

      this.messageThreadSource.next([]);

      this.hubConnection.stop();
    } 
    
  }

  getMessageForUSer(pageNumber:number, pageSize:number, container:string){

    let param= getPaginationHeaders(pageNumber,pageSize);
    param=param.append("Container", container);

    return getPaginatedResult<Message[]>(BASE_URL+'messages', param, this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(BASE_URL+"messages/thread/"+username);
  }

  async sendMessages(usernamne:string, content:string){
   
    return this.hubConnection.invoke("SendMessage",{recipientUsername: usernamne,content})
      .catch(error=>console.log(error));
  }

  delete(id:number){

    return this.http.delete(BASE_URL+'messages/'+id);
  }
}
