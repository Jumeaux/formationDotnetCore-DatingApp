import { User } from 'src/app/_models/user';
import { AccountService } from './account.service';
import { UserParams } from './../_models/userParams';
import { map, take } from 'rxjs/operators';
import { BASE_URL } from './../back-end/url';
import { Member } from './../_models/member';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';


@Injectable({
  providedIn: 'root'
})
export class MembersService {


  members: Member[]=[];
  memberCache= new Map();
  userParams:UserParams;
  user:User;


  
  constructor(private http: HttpClient, private accountService:AccountService) {

    this.accountService.currentUser$.pipe(take(1)).subscribe(user=>{
      this.user= user;
      this.userParams= new UserParams(user);
    });
   }


  getMembers(userParams:UserParams){
    
    var response= this.memberCache.get(Object.values(userParams).join('-'));
      console.log(response);
      
    if (response) return of(response);

   let param=this.getPaginationHeaders(userParams.pageNumber,userParams.pageSize)
    param=param.append('minAge',userParams.minAge.toString());
    param=param.append('maxAge',userParams.maxAge.toString());
    param=param.append('gender',userParams.gender);      
    param=param.append('orderBy',userParams.orderBy);  
    
    return this.getPaginatedResult<Member[]>(BASE_URL+'users', param)
      .pipe(map(response=>{
        this.memberCache.set(Object.values(userParams).join('-'), response);        
        return response;
      }))
  }


  getPaginationHeaders(pageNumber:number,pageSize:number):HttpParams{

    let params= new HttpParams();
      params= params.append('pageNumber',pageNumber.toString());
      params= params.append('pageSize',pageSize.toString());
      return params; 
  }

  getMember(username:string){

    const member= [...this.memberCache.values()]
    .reduce((preValues,element)=> preValues.concat(element), [])
    .find((member:Member)=> member.username === username);
    
    
    if(member) return of(member);

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

  setMainPhoto(idPhoto:number){

    return this.http.put(BASE_URL+'users/set-main-photo/'+idPhoto,{})
  }

  deletePhot(photoId:number){

    return this.http.delete(BASE_URL+'users/delete-photo/'+photoId);
  }

  private getPaginatedResult<T>(url,params) {
    
    const paginatedResult: PaginatedResult<T>= new PaginatedResult<T>();
    
    return this.http.get<T>(url, { observe: 'response', params }).pipe(

      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })

    );
  }

  setUserParams(userParams:UserParams){

    return this.userParams=userParams;
  }

  getUserParams(){

    return this.userParams;
  }
  resetUSerParams(){
    this.userParams= new UserParams(this.user);
    return this.userParams;
  }

}
