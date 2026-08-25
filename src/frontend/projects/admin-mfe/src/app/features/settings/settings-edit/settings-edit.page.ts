import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SettingsStore } from '../settings.store';

/** SDD CRM-037 — system settings smart page. */
@Component({
  selector: 'app-settings-edit-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './settings-edit.html',
  styleUrls: ['./settings-edit.scss'],
})
export class SettingsEditPage implements OnInit {
  readonly store = inject(SettingsStore);

  organizationName = '';
  supportEmail = '';
  defaultCulture = 'en';
  maxFailedLoginAttempts = 5;
  lockoutMinutes = 15;

  constructor() {
    effect(() => {
      const row = this.store.settings();
      if (!row) {
        return;
      }
      this.organizationName = row.organizationName;
      this.supportEmail = row.supportEmail;
      this.defaultCulture = row.defaultCulture;
      this.maxFailedLoginAttempts = row.maxFailedLoginAttempts;
      this.lockoutMinutes = row.lockoutMinutes;
    });
  }

  ngOnInit(): void {
    this.store.load();
  }

  submit(): void {
    this.store.save({
      organizationName: this.organizationName.trim(),
      supportEmail: this.supportEmail.trim(),
      defaultCulture: this.defaultCulture,
      maxFailedLoginAttempts: Number(this.maxFailedLoginAttempts),
      lockoutMinutes: Number(this.lockoutMinutes),
    });
  }

  reset(): void {
    const row = this.store.settings();
    if (!row) {
      return;
    }
    this.organizationName = row.organizationName;
    this.supportEmail = row.supportEmail;
    this.defaultCulture = row.defaultCulture;
    this.maxFailedLoginAttempts = row.maxFailedLoginAttempts;
    this.lockoutMinutes = row.lockoutMinutes;
  }
}
