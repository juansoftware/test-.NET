import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { LoadingService } from '../../services/loading.service';
import { Person, CreatePersonRequest, UpdatePersonRequest } from '../../models/person.model';

interface NotificationMessage {
  type: 'success' | 'error' | 'info';
  message: string;
  timestamp: Date;
}

@Component({
  selector: 'app-people',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './people.component.html',
  styleUrls: ['./people.component.scss']
})
export class PeopleComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  // Data
  people: Person[] = [];
  filteredPeople: Person[] = [];
  searchTerm = '';
  
  // Form states
  showCreateForm = false;
  showEditForm = false;
  editingPerson: Person | null = null;
  
  // Form data with validation
  createForm = {
    name: '',
    isValid: false
  };
  
  editForm = {
    newName: '',
    isValid: false
  };

  // UI state
  isLoading = false;
  notifications: NotificationMessage[] = [];
  formErrors: { [key: string]: string } = {};

  constructor(
    private apiService: ApiService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.loadPeople();
    this.setupLoadingSubscription();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupLoadingSubscription(): void {
    this.loadingService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe((isLoading: boolean) => {
        this.isLoading = isLoading;
      });
  }

  loadPeople(): void {
    this.loadingService.show('Loading people...');
    
    this.apiService.getPeople()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (people) => {
          this.people = people;
          this.filteredPeople = people;
          this.loadingService.hide();
          this.showNotification('success', 'People loaded successfully');
        },
        error: (error) => {
          console.error('Error loading people:', error);
          this.loadingService.hide();
          this.showNotification('error', 'Failed to load people. Please try again.');
        }
      });
  }

  searchPeople(): void {
    if (!this.searchTerm.trim()) {
      this.filteredPeople = this.people;
      return;
    }
    
    const term = this.searchTerm.toLowerCase();
    this.filteredPeople = this.people.filter(person =>
      person.name.toLowerCase().includes(term) ||
      (person.currentDutyTitle && person.currentDutyTitle.toLowerCase().includes(term)) ||
      (person.currentRank && person.currentRank.toLowerCase().includes(term))
    );
  }

  validateCreateForm(): boolean {
    this.formErrors = {};
    
    if (!this.createForm.name.trim()) {
      this.formErrors['name'] = 'Name is required';
      return false;
    }
    
    if (this.createForm.name.trim().length < 2) {
      this.formErrors['name'] = 'Name must be at least 2 characters';
      return false;
    }
    
    if (this.createForm.name.trim().length > 100) {
      this.formErrors['name'] = 'Name must be less than 100 characters';
      return false;
    }
    
    // Check for duplicate names
    const existingPerson = this.people.find(p => 
      p.name.toLowerCase() === this.createForm.name.trim().toLowerCase()
    );
    
    if (existingPerson) {
      this.formErrors['name'] = 'A person with this name already exists';
      return false;
    }
    
    return true;
  }

  validateEditForm(): boolean {
    this.formErrors = {};
    
    if (!this.editForm.newName.trim()) {
      this.formErrors['newName'] = 'Name is required';
      return false;
    }
    
    if (this.editForm.newName.trim().length < 2) {
      this.formErrors['newName'] = 'Name must be at least 2 characters';
      return false;
    }
    
    if (this.editForm.newName.trim().length > 100) {
      this.formErrors['newName'] = 'Name must be less than 100 characters';
      return false;
    }
    
    // Check for duplicate names (excluding current person)
    const existingPerson = this.people.find(p => 
      p.name.toLowerCase() === this.editForm.newName.trim().toLowerCase() &&
      p.personId !== this.editingPerson?.personId
    );
    
    if (existingPerson) {
      this.formErrors['newName'] = 'A person with this name already exists';
      return false;
    }
    
    return true;
  }

  createPerson(): void {
    if (!this.validateCreateForm()) {
      return;
    }
    
    this.loadingService.show('Creating person...');
    
    const request: CreatePersonRequest = {
      name: this.createForm.name.trim()
    };
    
    this.apiService.createPerson(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.loadPeople();
          this.showCreateForm = false;
          this.createForm.name = '';
          this.loadingService.hide();
          this.showNotification('success', `Person "${request.name}" created successfully`);
        },
        error: (error) => {
          console.error('Error creating person:', error);
          this.loadingService.hide();
          this.showNotification('error', 'Failed to create person. Please try again.');
        }
      });
  }

  startEdit(person: Person): void {
    this.editingPerson = person;
    this.editForm.newName = person.name;
    this.showEditForm = true;
  }

  updatePerson(): void {
    if (!this.editingPerson || !this.validateEditForm()) {
      return;
    }
    
    this.loadingService.show('Updating person...');
    
    const request: UpdatePersonRequest = {
      newName: this.editForm.newName.trim()
    };
    
    this.apiService.updatePerson(this.editingPerson.name, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.loadPeople();
          this.cancelEdit();
          this.loadingService.hide();
          this.showNotification('success', `Person updated to "${request.newName}" successfully`);
        },
        error: (error) => {
          console.error('Error updating person:', error);
          this.loadingService.hide();
          this.showNotification('error', 'Failed to update person. Please try again.');
        }
      });
  }

  cancelCreate(): void {
    this.showCreateForm = false;
    this.createForm.name = '';
    this.formErrors = {};
  }

  cancelEdit(): void {
    this.showEditForm = false;
    this.editingPerson = null;
    this.editForm.newName = '';
    this.formErrors = {};
  }

  dismissNotification(index: number): void {
    this.notifications.splice(index, 1);
  }

  private showNotification(type: 'success' | 'error' | 'info', message: string): void {
    const notification: NotificationMessage = {
      type,
      message,
      timestamp: new Date()
    };
    
    this.notifications.push(notification);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
      const index = this.notifications.indexOf(notification);
      if (index > -1) {
        this.notifications.splice(index, 1);
      }
    }, 5000);
  }

  getStatusBadgeClass(status: string | undefined): string {
    if (!status) return 'status-none';
    if (status === 'RETIRED') return 'status-retired';
    return 'status-active';
  }

  getStatusText(status: string | undefined): string {
    if (!status) return 'Not Assigned';
    if (status === 'RETIRED') return 'Retired';
    return 'Active';
  }

  getFormError(field: string): string {
    return this.formErrors[field] || '';
  }

  hasFormError(field: string): boolean {
    return !!this.formErrors[field];
  }
}
