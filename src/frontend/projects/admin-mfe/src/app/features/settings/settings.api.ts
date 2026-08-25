import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ErpDelivery, SystemSettings } from './settings.models';

/** SDD CRM-037 — identity system settings via gateway. */
@Injectable({ providedIn: 'root' })
export class SettingsApi {
  private readonly http = inject(HttpClient);

  get(): Observable<SystemSettings> {
    return this.http.get<SystemSettings>('/api/identity/settings');
  }

  update(body: Omit<SystemSettings, 'updatedAt'>): Observable<SystemSettings> {
    return this.http.put<SystemSettings>('/api/identity/settings', body);
  }

  /** SDD CRM-039 polish / 044 */
  erpDeliveries(take = 10): Observable<ErpDelivery[]> {
    return this.http.get<ErpDelivery[]>(`/api/tickets/integrations/erp-deliveries?take=${take}`);
  }
}
