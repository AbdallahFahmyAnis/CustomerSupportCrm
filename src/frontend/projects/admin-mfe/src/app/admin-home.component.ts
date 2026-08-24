import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** SDD CRM-035 — admin home. */
@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.scss'],
})
export class AdminHomeComponent {}
