import { BASE_URL } from './../back-end/url';
import { Member } from './../_models/member';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  constructor(private http: HttpClient) { }


  getMembers(){

    return this.http.get<Member[]>(BASE_URL +'users');
  }

  getMember(username:string){

    return this.http.get<Member>(BASE_URL+'users/'+username)
  }
}
