import { map } from 'rxjs/operators';
import { RoleModalComponent } from './../../modals/role-modal/role-modal.component';
import { AdminService } from './../../_services/admin.service';
import { Component, OnInit, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  bsModalRef:BsModalRef
  users:Partial<User[]>;
  constructor(private adminService:AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {

    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.getUsersWithRoles().subscribe(users=>{
      this.users=users;
    });
  }

  openRoleModal(user :User){  
    const config:ModalOptions = {
      
      class:'modal-dialog-centered',
      initialState:{
        user,
        roles:this.getRoles(user)
      }
    };
    this.bsModalRef = this.modalService.show(RoleModalComponent,config); 
    this.bsModalRef.content.updateSelectedRoles.subscribe(values=>{
      
      const rolesToUpdate={
        roles:[... values.filter(eleme=>eleme.checked===true).map(el=>el.name)]
      }
      
      if(rolesToUpdate){
        this.adminService.updateRole(user.username, rolesToUpdate.roles).subscribe(()=>{
          user.roles=[... rolesToUpdate.roles];
        });
      }
    }); 
  }

  private getRoles(user:User){
    const roles =[];
    const userRoles = user.roles;
    const availableRoles: any[]=[
      {name:'Admin', value:'Admin'},
      {name:'Moderator', value:'Moderator'},
      {name:'Member', value:'Member'},
    ];

    availableRoles.forEach(role =>{
      let isMatch=false;  
      for(const userRole of userRoles){
        if(role.name=== userRole){

          isMatch=true;
          role.checked=true;
          roles.push(role);
          break;
        }
      }

      if(!isMatch)  {
        role.checked= false;
        roles.push(role);
      }

    });
    return roles;
  }

  

}
