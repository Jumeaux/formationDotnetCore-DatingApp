import { ToastrService } from 'ngx-toastr';
import { MembersService } from './../../_services/members.service';
import { Member } from './../../_models/member';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {


  @Input() member:Member
  constructor(private membersService:MembersService, private toastrService:ToastrService) { }

  ngOnInit(): void {
  }


  addLike(member:Member){
    this.membersService.addLike(member.username).subscribe(
     () =>{this.toastrService.success(`you have liked ${member.username}`)},
    );
  }

}
