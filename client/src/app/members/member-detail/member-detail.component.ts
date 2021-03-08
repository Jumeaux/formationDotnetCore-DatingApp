import { take } from 'rxjs/operators';
import { AccountService } from './../../_services/account.service';
import { User } from './../../_models/user';
import { PresenceService } from './../../_services/presence.service';
import { MessagesService } from './../../_services/messages.service';
import { Message } from './../../_models/message';
import { Member } from './../../_models/member';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MembersService } from 'src/app/_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryOptions, NgxGalleryImage } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';




@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {

  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent

  tabsActive: TabDirective;
  member: Member;
  messages: Message[] = []
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  user:User
  constructor(private messageService: MessagesService, 
    public presence :PresenceService,
    private activateRoute: ActivatedRoute, 
    private accountService:AccountService,
    private router:Router) { 
      this.accountService.currentUser$.pipe(take(1)).subscribe(user=>{
        this.user=user;
      })
      this.router.routeReuseStrategy.shouldReuseRoute = ()=> false;
  }
 

  ngOnInit(): void {
    this.activateRoute.data.subscribe(data => {
      this.member = data.member;
    });

    this.activateRoute.queryParams.subscribe(params => {

      params.tab ? this.selectTabs(params.tab) : this.selectTabs(0);
    });
    this.gallery();

  }

  gallery() {

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.getImages();

  }

  getImages(): NgxGalleryImage[] {

    const imagesUrl = [];

    for (const photo of this.member.photos) {

      imagesUrl.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });
    }
    return imagesUrl
  }


  onTabActived(data: TabDirective) {

    this.tabsActive = data;
    if (this.tabsActive.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.username);
    }else{
      this.messageService.stopHubConnection();
    }
  }


  loadMessagesThread() {
    this.messageService.getMessageThread(this.member.username).subscribe(response => {
      this.messages = response;
    });
  }

  selectTabs(tabId: number) {

    this.memberTabs.tabs[tabId].active = true;
  }

  ngOnDestroy(): void {
   this.messageService.stopHubConnection();
  }

}
