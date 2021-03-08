import { BASE_URL } from './../back-end/url';
import { User } from './../_models/user';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {map} from 'rxjs/operators';
import { ReplaySubject } from 'rxjs';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {


  currentUserSource= new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor( private http:HttpClient,private presenceService:PresenceService) { }


  login(model:any){

    return this.http.post(BASE_URL+'account/login',model).pipe(
      map((response:User)=>{
        const user= response;
        if(user){
          this.setCurrentUser(user);
          this.presenceService.createHubConnection(user);
        }
     })
    );
  }


  register(model:any){

    return this.http.post(BASE_URL+"account/register", model).pipe(
      map((user:User)=>{
        if (user) {
        this.setCurrentUser(user);
        this.presenceService.createHubConnection(user);
        }
      })
    );
  }

  setCurrentUser(user:User){
    user.roles=[];
    const roles= this.decodeToken(user.token).role;
    Array.isArray(roles) ? user.roles=roles : user.roles.push(roles);
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);
  }


  logOut(){

    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  decodeToken(token){
    return JSON.parse(atob(token.split(".")[1]));
  }
}
