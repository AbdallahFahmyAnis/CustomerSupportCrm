import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/** SDD CRM-035 — admin home. */
@Component({
  selector: 'app-admin-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class AdminHomePage {}
