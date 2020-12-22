

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { BASE_URL } from './../../back-end/url';
@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {

  validationErrors:string[]=[]
  constructor(private http:HttpClient) { }

  ngOnInit(): void {
  }

  get404Error():void{
    this.http.get(BASE_URL+"bugg/not-found").subscribe(
      
      res=>console.log(res),
      error=>console.log(error)
    );
  }


  get400Error(){
    this.http.get(BASE_URL+"bugg/bad-request").subscribe(

      res=>{console.log(res)},
      error=>{console.log(error)}
    );
  }


  get401Error():void{
    this.http.get(BASE_URL+"bugg/auth").subscribe(

      res=>console.log(res),
      error=>console.log(error)
   
    );
  }


  get500Error():void{
    this.http.get(BASE_URL+"bugg/server-error").subscribe(

      res=>console.log(res),
      error=>console.log(error)
   
    );
  }

  get400ValidationError():void{
    this.http.post(BASE_URL+"account/register",{}).subscribe(

      res=>console.log(res),
      error=>{this.validationErrors=error }
   
    );
  }
  
 

}
