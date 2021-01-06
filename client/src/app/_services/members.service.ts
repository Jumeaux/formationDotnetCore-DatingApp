import { map } from 'rxjs/operators';
import { BASE_URL } from './../back-end/url';
import { Member } from './../_models/member';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class MembersService {


  members: Member[]=[];
  constructor(private http: HttpClient) { }


  getMembers(){

    if(this.members.length > 0) return  of(this.members);

    return this.http.get<Member[]>(BASE_URL +'users').pipe(
      map(members=>{
        this.members=members;
        return members; 
      })
    );
  }

  getMember(username:string){
    const member= this.members.find(m=>m.username === username);

    if (member!== undefined) return of(member); 

    return this.http.get<Member>(BASE_URL+'users/'+username);
  }


  updateMember(member:Member){

    return this.http.put(BASE_URL+'users',member).pipe(
      map(()=>{
        const index= this.members.indexOf(member);
        this.members[index]=member;
      })
    );
  }
}
