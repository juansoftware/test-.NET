import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  private loadingMessagesSubject = new BehaviorSubject<string>('');
  public loadingMessage$ = this.loadingMessagesSubject.asObservable();

  show(message: string = 'Loading...'): void {
    this.loadingMessagesSubject.next(message);
    this.loadingSubject.next(true);
  }

  hide(): void {
    this.loadingSubject.next(false);
    this.loadingMessagesSubject.next('');
  }
}
