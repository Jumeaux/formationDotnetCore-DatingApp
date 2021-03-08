import { Router } from '@angular/router';
import { User } from 'src/app/_models/user';
import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HUBS_URL } from '../back-end/url';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  private hubConnection:HubConnection;

  private onelineUsersSource= new BehaviorSubject<string[]>([]);
  onlineUsers$= this.onelineUsersSource.asObservable();
  
  constructor(private toastService:ToastrService, private route:Router) { }

  createHubConnection(user:User){
    this.hubConnection= new HubConnectionBuilder()
      .withUrl(HUBS_URL+'presence',{
        accessTokenFactory:()=>user.token
      })
      .withAutomaticReconnect().build();

    this.hubConnection.start().catch(error=>this.toastService.error(error));

    this.hubConnection.on("UserIsOnLine", username=>{
      this.onlineUsers$.pipe(take(1)).subscribe(usernames=>{
        this.onelineUsersSource.next([... usernames,username]);
      })
    })

    this.hubConnection.on("UserIsOffLine", username=>{
      this.onlineUsers$.pipe(take(1)).subscribe(usernames=>{
        this.onelineUsersSource.next([...usernames.filter(x =>x !==username)])
      })
    });

    this.hubConnection.on("OnlineUsers",(usernames:string[])=> {
      this.onelineUsersSource.next(usernames);
    })

    // notify user 
    this.hubConnection.on("NewMessageReceived",({username,knownAs})=>{
      this.toastService.info("You received a new message from "+knownAs)
       .onTap.pipe(take(1)).subscribe(()=>this.route.navigateByUrl("member/"+username+ "?tab=3"));
    })
  }

  stopHubConnection(){
    this.hubConnection.stop().catch(error=>{console.log(error);
    })
  }
}
