import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  title = 'Stargate ACTS';
  navigationItems = [
    { label: 'Dashboard', route: '/dashboard', icon: 'dashboard' },
    { label: 'People', route: '/people', icon: 'people' },
    { label: 'Astronaut Duties', route: '/astronaut-duties', icon: 'assignment' },
    { label: 'Reports', route: '/reports', icon: 'assessment' }
  ];

  getIcon(iconName: string): string {
    const icons: { [key: string]: string } = {
      dashboard: '📊',
      people: '👥',
      assignment: '📋',
      assessment: '📈'
    };
    return icons[iconName] || '📄';
  }
}
