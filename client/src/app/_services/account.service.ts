import { BASE_URL } from './../back-end/url';
import { User } from './../_models/user';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {map} from 'rxjs/operators';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  

  currentUserSource= new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor( private http:HttpClient) { }


  login(model:any){

    return this.http.post(BASE_URL+'account/login',model).pipe(
      map((response:User)=>{
        const user= response;
        console.log(user);
        
        if(user){
          this.setCurrentUser(user);
        }
     })
    );
  }


  register(model:any){

    return this.http.post(BASE_URL+"account/register", model).pipe(
      map((user:User)=>{
        if (user) {
        this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user:User){
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logOut(){

    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
