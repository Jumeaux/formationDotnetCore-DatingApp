import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';
import { AccountService } from './../_services/account.service';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private accountService:AccountService, private toaster: ToastrService){

  }

  canActivate(): Observable<boolean > {
   return this.accountService.currentUser$.pipe(
      map(user=>{
        if(user.roles.includes("Admin") || user.roles.includes("Moderator")) {
          return true;
        } 
        this.toaster.error("you're not allowd");
      })
    )

  }
  
}
