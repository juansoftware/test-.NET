import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ErrorHandlerService } from './error-handler.service';
import {
  Person,
  AstronautDetail,
  CreatePersonRequest,
  UpdatePersonRequest,
  CreateAstronautDutyRequest,
  PersonListResponse,
  PersonResponse,
  AstronautDetailResponse,
  CreatePersonResponse,
  UpdatePersonResponse,
  CreateAstronautDutyResponse
} from '../models/person.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private errorHandler: ErrorHandlerService
  ) {}

  // Person endpoints
  getPeople(): Observable<Person[]> {
    return this.http.get<PersonListResponse>(`${this.baseUrl}/Person`)
      .pipe(
        map(response => response.people || []),
        catchError(this.handleError.bind(this))
      );
  }

  getPersonByName(name: string): Observable<Person | null> {
    return this.http.get<PersonResponse>(`${this.baseUrl}/Person/${encodeURIComponent(name)}`)
      .pipe(
        map(response => response.person || null),
        catchError(this.handleError.bind(this))
      );
  }

  createPerson(request: CreatePersonRequest): Observable<CreatePersonResponse> {
    return this.http.post<CreatePersonResponse>(`${this.baseUrl}/Person`, JSON.stringify(request.name), {
      headers: {
        'Content-Type': 'application/json'
      }
    })
      .pipe(
        catchError(this.handleError.bind(this))
      );
  }

  updatePerson(name: string, request: UpdatePersonRequest): Observable<UpdatePersonResponse> {
    return this.http.put<UpdatePersonResponse>(`${this.baseUrl}/Person/${encodeURIComponent(name)}`, request)
      .pipe(
        catchError(this.handleError.bind(this))
      );
  }

  // Astronaut Duty endpoints
  getAstronautDutiesByName(name: string): Observable<AstronautDetail> {
    return this.http.get<AstronautDetailResponse>(`${this.baseUrl}/AstronautDuty/${encodeURIComponent(name)}`)
      .pipe(
        map(response => ({
          person: response.person,
          astronautDuties: response.astronautDuties || []
        })),
        catchError(this.handleError.bind(this))
      );
  }

  createAstronautDuty(request: CreateAstronautDutyRequest): Observable<CreateAstronautDutyResponse> {
    return this.http.post<CreateAstronautDutyResponse>(`${this.baseUrl}/AstronautDuty`, request)
      .pipe(
        catchError(this.handleError.bind(this))
      );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    const errorInfo = this.errorHandler.handleError(error);
    return throwError(() => new Error(errorInfo.message));
  }
}
