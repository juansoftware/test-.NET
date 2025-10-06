import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { LoadingService } from '../../services/loading.service';
import { Person, AstronautDetail, CreateAstronautDutyRequest } from '../../models/person.model';

@Component({
  selector: 'app-astronaut-duties',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './astronaut-duties.component.html',
  styleUrls: ['./astronaut-duties.component.scss']
})
export class AstronautDutiesComponent implements OnInit {
  people: Person[] = [];
  selectedPerson: Person | null = null;
  astronautDetails: AstronautDetail | null = null;
  
  // Form states
  showCreateForm = false;
  showDetailsModal = false;
  
  // Form data
  createForm = {
    name: '',
    rank: '',
    dutyTitle: '',
    dutyStartDate: ''
  };

  constructor(
    private apiService: ApiService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.loadPeople();
  }

  loadPeople(): void {
    this.loadingService.show('Loading people...');
    
    this.apiService.getPeople().subscribe({
      next: (people) => {
        this.people = people;
        this.loadingService.hide();
      },
      error: (error) => {
        console.error('Error loading people:', error);
        this.loadingService.hide();
      }
    });
  }

  viewAstronautDetails(person: Person): void {
    this.selectedPerson = person;
    this.loadingService.show('Loading astronaut details...');
    
    this.apiService.getAstronautDutiesByName(person.name).subscribe({
      next: (details) => {
        this.astronautDetails = details;
        this.showDetailsModal = true;
        this.loadingService.hide();
      },
      error: (error) => {
        console.error('Error loading astronaut details:', error);
        this.loadingService.hide();
      }
    });
  }

  createDuty(): void {
    if (!this.createForm.name || !this.createForm.rank || 
        !this.createForm.dutyTitle || !this.createForm.dutyStartDate) {
      return;
    }
    
    this.loadingService.show('Creating astronaut duty...');
    
    const request: CreateAstronautDutyRequest = {
      name: this.createForm.name,
      rank: this.createForm.rank,
      dutyTitle: this.createForm.dutyTitle,
      dutyStartDate: this.createForm.dutyStartDate
    };
    
    this.apiService.createAstronautDuty(request).subscribe({
      next: (response) => {
        this.loadPeople();
        this.showCreateForm = false;
        this.resetCreateForm();
        this.loadingService.hide();
      },
      error: (error) => {
        console.error('Error creating astronaut duty:', error);
        this.loadingService.hide();
      }
    });
  }

  resetCreateForm(): void {
    this.createForm = {
      name: '',
      rank: '',
      dutyTitle: '',
      dutyStartDate: ''
    };
  }

  cancelCreate(): void {
    this.showCreateForm = false;
    this.resetCreateForm();
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedPerson = null;
    this.astronautDetails = null;
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

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  getDuration(startDate: string, endDate?: string): string {
    const start = new Date(startDate);
    const end = endDate ? new Date(endDate) : new Date();
    const diffTime = Math.abs(end.getTime() - start.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (diffDays < 30) {
      return `${diffDays} days`;
    } else if (diffDays < 365) {
      const months = Math.floor(diffDays / 30);
      return `${months} month${months > 1 ? 's' : ''}`;
    } else {
      const years = Math.floor(diffDays / 365);
      const months = Math.floor((diffDays % 365) / 30);
      let result = `${years} year${years > 1 ? 's' : ''}`;
      if (months > 0) {
        result += ` ${months} month${months > 1 ? 's' : ''}`;
      }
      return result;
    }
  }
}
