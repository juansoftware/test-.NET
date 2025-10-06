import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { LoadingService } from '../../services/loading.service';
import { Person } from '../../models/person.model';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  people: Person[] = [];
  stats = {
    totalPeople: 0,
    activeAstronauts: 0,
    retiredAstronauts: 0,
    unassignedPeople: 0,
    averageCareerLength: 0
  };

  constructor(
    private apiService: ApiService,
    private loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    this.loadReportsData();
  }

  loadReportsData(): void {
    this.loadingService.show('Loading reports data...');
    
    this.apiService.getPeople().subscribe({
      next: (people) => {
        this.people = people;
        this.calculateStats();
        this.loadingService.hide();
      },
      error: (error) => {
        console.error('Error loading reports data:', error);
        this.loadingService.hide();
      }
    });
  }

  private calculateStats(): void {
    this.stats.totalPeople = this.people.length;
    this.stats.activeAstronauts = 0;
    this.stats.retiredAstronauts = 0;
    this.stats.unassignedPeople = 0;
    
    let totalCareerDays = 0;
    let careerCount = 0;

    this.people.forEach(person => {
      if (person.currentDutyTitle) {
        if (person.currentDutyTitle === 'RETIRED') {
          this.stats.retiredAstronauts++;
        } else {
          this.stats.activeAstronauts++;
        }
        
        // Calculate career length
        if (person.careerStartDate) {
          const startDate = new Date(person.careerStartDate);
          const endDate = person.careerEndDate ? new Date(person.careerEndDate) : new Date();
          const careerDays = Math.floor((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24));
          totalCareerDays += careerDays;
          careerCount++;
        }
      } else {
        this.stats.unassignedPeople++;
      }
    });

    this.stats.averageCareerLength = careerCount > 0 ? Math.round(totalCareerDays / careerCount) : 0;
  }

  getPeopleByStatus(status: string): Person[] {
    return this.people.filter(person => {
      switch (status) {
        case 'active':
          return person.currentDutyTitle && person.currentDutyTitle !== 'RETIRED';
        case 'retired':
          return person.currentDutyTitle === 'RETIRED';
        case 'unassigned':
          return !person.currentDutyTitle;
        default:
          return true;
      }
    });
  }

  formatCareerLength(days: number): string {
    if (days < 365) {
      return `${Math.round(days / 30)} months`;
    } else {
      const years = Math.floor(days / 365);
      const months = Math.round((days % 365) / 30);
      return `${years} years${months > 0 ? ` ${months} months` : ''}`;
    }
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

  getUtilizationPercentage(): number {
    if (this.stats.totalPeople === 0) return 0;
    const assigned = this.stats.activeAstronauts + this.stats.retiredAstronauts;
    return Math.round((assigned / this.stats.totalPeople) * 100);
  }

  exportToCSV(): void {
    const csvContent = this.generateCSV();
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'stargate-acts-report.csv';
    link.click();
    window.URL.revokeObjectURL(url);
  }

  private generateCSV(): string {
    const headers = ['Name', 'Current Rank', 'Current Duty Title', 'Career Start Date', 'Career End Date', 'Status'];
    const rows = this.people.map(person => [
      person.name,
      person.currentRank || 'N/A',
      person.currentDutyTitle || 'N/A',
      person.careerStartDate || 'N/A',
      person.careerEndDate || 'N/A',
      this.getStatusText(person.currentDutyTitle)
    ]);
    
    return [headers, ...rows].map(row => 
      row.map(field => `"${field}"`).join(',')
    ).join('\n');
  }
}
