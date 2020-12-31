import { Photo } from "./photo";

export interface Member {
  id: number;
  username: string;
  photoUrl: string;
  age: number;
  gender: string;
  knowAs?: any;
  lastActive: Date;
  created: Date;
  introduction: string;
  lookingFor: string;
  interests: string;
  city: string;
  country: string;
  photos: Photo[];
}

