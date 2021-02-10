import { Pagination } from 'src/app/_models/pagination';
import { MembersService } from 'src/app/_services/members.service';
import { Member } from './../_models/member';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Partial<Member[]>;
  predicat:string='liked';
  pageNumber:number=1;
  pageSize:number=5;

  pagination:Pagination;
  constructor(private memberService:MembersService) { }

  ngOnInit(): void {

    this.loadMemberLikes();
  }

  loadMemberLikes(){
    this.memberService.getLikes(this.predicat,this.pageNumber, this.pageSize).subscribe(res=>{
      this.members=res.result;
      this.pagination=res.pagination;
    });
  }

  pageChanged(event:any){
    this.pageNumber=event.page;
    this.loadMemberLikes();
  }
}
