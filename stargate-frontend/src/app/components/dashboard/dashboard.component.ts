import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { LoadingService } from '../../services/loading.service';
import { Person, AstronautDetail } from '../../models/person.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats = {
    totalPeople: 0,
    activeAstronauts: 0,
    retiredAstronauts: 0,
    totalDuties: 0
  };
  
  recentPeople: Person[] = [];
  recentDuties: any[] = [];

  constructor(
    private apiService: ApiService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    this.loadingService.show('Loading dashboard data...');
    
    this.apiService.getPeople().subscribe({
      next: (people) => {
        this.recentPeople = people.slice(0, 5); // Show only first 5
        this.calculateStats(people);
        this.loadingService.hide();
      },
      error: (error) => {
        console.error('Error loading dashboard data:', error);
        this.loadingService.hide();
      }
    });
  }

  private calculateStats(people: Person[]): void {
    this.stats.totalPeople = people.length;
    
    people.forEach(person => {
      if (person.currentDutyTitle) {
        if (person.currentDutyTitle === 'RETIRED') {
          this.stats.retiredAstronauts++;
        } else {
          this.stats.activeAstronauts++;
        }
      }
    });
    
    // For now, we'll estimate total duties (in a real app, you'd get this from an API)
    this.stats.totalDuties = this.stats.activeAstronauts + this.stats.retiredAstronauts;
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
}
