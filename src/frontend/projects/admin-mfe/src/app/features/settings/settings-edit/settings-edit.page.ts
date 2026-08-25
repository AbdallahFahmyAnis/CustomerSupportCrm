import { DatePipe } from '@angular/common';
import { Component, OnInit, effect, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SettingsStore } from '../settings.store';

/** SDD CRM-037 — system settings smart page. */
@Component({
  selector: 'app-settings-edit-page',
  standalone: true,
  imports: [FormsModule, DatePipe],
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
  productTitle = 'Customer Support CRM';
  primaryColor = '#2563eb';
  logoUrl = '/brand/azm-squad.png';
  erpWebhookUrl = '';
  erpWebhookAuthHeader = '';

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
      this.productTitle = row.productTitle || 'Customer Support CRM';
      this.primaryColor = row.primaryColor || '#2563eb';
      this.logoUrl = row.logoUrl || '/brand/azm-squad.png';
      this.erpWebhookUrl = row.erpWebhookUrl || '';
      this.erpWebhookAuthHeader = row.erpWebhookAuthHeader || '';
    });
  }

  ngOnInit(): void {
    this.store.load();
  }

  refreshDeliveries(): void {
    this.store.loadErpDeliveries();
  }

  submit(): void {
    this.store.save({
      organizationName: this.organizationName.trim(),
      supportEmail: this.supportEmail.trim(),
      defaultCulture: this.defaultCulture,
      maxFailedLoginAttempts: Number(this.maxFailedLoginAttempts),
      lockoutMinutes: Number(this.lockoutMinutes),
      productTitle: this.productTitle.trim(),
      primaryColor: this.primaryColor.trim(),
      logoUrl: this.logoUrl.trim(),
      erpWebhookUrl: this.erpWebhookUrl.trim(),
      erpWebhookAuthHeader: this.erpWebhookAuthHeader.trim(),
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
    this.productTitle = row.productTitle || 'Customer Support CRM';
    this.primaryColor = row.primaryColor || '#2563eb';
    this.logoUrl = row.logoUrl || '/brand/azm-squad.png';
    this.erpWebhookUrl = row.erpWebhookUrl || '';
    this.erpWebhookAuthHeader = row.erpWebhookAuthHeader || '';
  }
}
