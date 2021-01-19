import { Router } from '@angular/router';
import { AccountService } from './../_services/account.service';
import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister=new EventEmitter();
  model:any={}
  maxDate: Date;
  validationsErrors: []=[];


  registerForm: FormGroup;
  constructor(private accountService:AccountService, private toastr:ToastrService, 
    private fb:FormBuilder, private route:Router) { }

  ngOnInit(): void {

    this.initForm();
    this.maxDate= new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initForm(){
    this.registerForm =this.fb.group({
        username: ['',Validators.required],
        gender: ['male'],
        knownAs: ['',Validators.required],
        dateOfBirth: ['',Validators.required],
        city: ['',Validators.required],
        country: ['',Validators.required],
        password: ['',[Validators.required, Validators.maxLength(8),Validators.minLength(4)]],
        passwordConfirm: ['',[Validators.required, this.matchValue('password')]]

    });
  }

  matchValue(matchTo:string): ValidatorFn{
      return (control:AbstractControl)=>{
        return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching:true}
      }
  }


  register(){
        
    this.accountService.register(this.registerForm.value).subscribe(

      response=>{this.route.navigateByUrl('/members')},
      error=>this.validationsErrors=error
    );  
  }

  cancel(){
    
   this.cancelRegister.emit(false);
  }

}
