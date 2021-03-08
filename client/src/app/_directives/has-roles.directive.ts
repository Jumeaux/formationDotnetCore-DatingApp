import { take } from 'rxjs/operators';
import { User } from 'src/app/_models/user';
import { AccountService } from './../_services/account.service';
import { Directive, TemplateRef, ViewContainerRef, OnInit, Input } from '@angular/core';

@Directive({
  selector: '[appHasRoles]'
})
export class HasRolesDirective implements OnInit{

  @Input() appHasRoles:string [];
  user: User;
  constructor(private viewContainerRef:ViewContainerRef, 
    private templateRef:TemplateRef<any>,
    private accountService:AccountService) { 

      this.accountService.currentUser$.pipe(take(1)).subscribe(
        user=>{this.user=user;
        });
    }

    ngOnInit(){
      if(!this.user?.roles || this.user==null){
        this.viewContainerRef.clear();
      }


      if (this.user?.roles.some(r=>this.appHasRoles.includes(r))) {
        
        this.viewContainerRef.createEmbeddedView(this.templateRef)
      }else{ this.viewContainerRef.clear();}
    }

}
