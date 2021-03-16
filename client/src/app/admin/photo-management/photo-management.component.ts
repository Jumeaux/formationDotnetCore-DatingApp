import { AdminService } from './../../_services/admin.service';
import { Photo } from './../../_models/photo';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  photos:Photo[]
  constructor(private adminService:AdminService) { }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }


  getPhotosForApproval(){
    this.adminService.getPhotosForApproval().subscribe(res=>{
      this.photos=res;
    })
  }

  approvePhoto(idPhoto:number){
    this.adminService.approvePhoto(idPhoto).subscribe(()=>{

      this.photos.splice(this.photos.findIndex(x=>x.id===idPhoto),1);
    })
  }

  rejectPhoto(idPhoto:number){
    this.adminService.rejectPhoto(idPhoto).subscribe(()=>{

      this.photos.splice(this.photos.findIndex(x=>x.id===idPhoto),1);
    })
  }

}
