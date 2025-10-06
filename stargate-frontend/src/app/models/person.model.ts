export interface Person {
  personId: number;
  name: string;
  currentRank?: string;
  currentDutyTitle?: string;
  careerStartDate?: string;
  careerEndDate?: string;
}

export interface AstronautDuty {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate?: string;
}

export interface AstronautDetail {
  person: Person;
  astronautDuties: AstronautDuty[];
}

export interface CreatePersonRequest {
  name: string;
}

export interface UpdatePersonRequest {
  newName: string;
}

export interface CreateAstronautDutyRequest {
  name: string;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  responseCode: number;
  data?: T;
}

export interface PersonListResponse extends ApiResponse<Person[]> {
  people: Person[];
}

export interface PersonResponse extends ApiResponse<Person> {
  person?: Person;
}

export interface AstronautDetailResponse extends ApiResponse<AstronautDetail> {
  person: Person;
  astronautDuties: AstronautDuty[];
}

export interface CreatePersonResponse extends ApiResponse<any> {
  id: number;
}

export interface UpdatePersonResponse extends ApiResponse<any> {
  id: number;
}

export interface CreateAstronautDutyResponse extends ApiResponse<any> {
  id?: number;
}
