import { PreventUnsavedChangesGuard } from './_guard/prevent-unsaved-changes.guard';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { AuthGuard } from './_guard/auth.guard';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';

const routes: Routes = [

  {path:'', component: HomeComponent},
  {
    path:'',
    runGuardsAndResolvers:'always',
    canActivate:[AuthGuard],
    children:[
      {path:'members', component:MemberListComponent},
      {path:'member/edit', component:MemberEditComponent, canDeactivate:[PreventUnsavedChangesGuard]},
      {path:'member/:username', component: MemberDetailComponent,resolve:{member:MemberDetailResolver}},
      {path:'lists', component: ListsComponent},
      {path:'messages', component: MessagesComponent}
    ]
  },
  {path:'server-error',component:ServerErrorComponent},
  {path:'Error', component:TestErrorComponent},
  {path:'not-found', component: NotFoundComponent},
  {path:'**', component:NotFoundComponent, pathMatch:'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
