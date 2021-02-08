import { User } from './user';
export class UserParams{
    minAge=18;
    maxAge=99;
    gender:string;
    pageNumber=1;
    pageSize=5; 
    orderBy="lastActive";

    constructor(user:User){
        this.gender= user.gender ==='female'? 'male':'female';

       // userParams.Gender = user.Gender == "male" ? "female" : "male";
    }
}