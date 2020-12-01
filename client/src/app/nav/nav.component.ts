import { AccountService } from './../_services/account.service';
import { Component, OnInit } from '@angular/core';



@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {


  model:any={};
  loggedIn:boolean;
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {

  }


  // login the user
  login():void{

    this.accountService.login(this.model).subscribe(

      res=> {console.log(res); this.loggedIn=true},
      error=>console.log(error)
    ); 
  }


  // logOut the user
  logOut(){
    this.accountService.logOut()
    this.loggedIn=false;
  }

  
}
