import { AccountService } from './../_services/account.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {


  model:any={};
  loggedIn:boolean;
  constructor(public accountService: AccountService, private route:Router, private toastr:ToastrService) { }

  ngOnInit(): void {

  }


  // login the user
  login():void{

    this.accountService.login(this.model).subscribe(
      res=> { this.route.navigateByUrl('/members')}
    ); 
  }


  // logOut the user
  logOut(){
    this.accountService.logOut();
    this.route.navigateByUrl('/');
  }

  
}
