import { BASE_URL } from 'src/app/back-end/url';
import { User } from 'src/app/_models/user';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private http:HttpClient) { }

  getUsersWithRoles(){
    return this.http.get<Partial<User[]>>(BASE_URL +"admin/users-with-roles");
  }

  updateRole( username: string, roles:string[]){

    return this.http.post(BASE_URL+'admin/edit-roles/'+username+'?roles='+roles,{});
  }
}
