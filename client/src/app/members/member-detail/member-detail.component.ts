import { MessagesService } from './../../_services/messages.service';
import { Message } from './../../_models/message';
import { Member } from './../../_models/member';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MembersService } from 'src/app/_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryOptions, NgxGalleryImage } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';




@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent

  tabsActive: TabDirective;
  member: Member;
  messages: Message[] = []
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private messageService: MessagesService, private activateRoute: ActivatedRoute) { }

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

      this.loadMessagesThread();
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

}
