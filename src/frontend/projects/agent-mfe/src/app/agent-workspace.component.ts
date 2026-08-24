import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-agent-workspace',
  standalone: true,
  template: `
    <section class="workspace">
      <h1>{{ title() }}</h1>
      <p>{{ hint() }}</p>
      @if (bootstrap(); as status) {
        <pre data-testid="customers-bootstrap">{{ status }}</pre>
      }
    </section>
  `,
  styles: `
    .workspace { padding: 1.5rem; max-width: 48rem; }
    pre { background: #0f172a; color: #e2e8f0; padding: 1rem; border-radius: 0.5rem; overflow: auto; }
  `,
})
export class AgentWorkspaceComponent implements OnInit {
  private readonly http = inject(HttpClient);
  readonly bootstrap = signal('');
  readonly title = signal(document.documentElement.lang === 'ar' ? 'مساحة عمل الوكيل جاهزة' : 'Agent workspace is ready');
  readonly hint = signal(
    document.documentElement.lang === 'ar'
      ? 'العملاء والتذاكر تصل إلى هنا في الشرائح التالية.'
      : 'Customers and tickets land here in later SDD slices.',
  );

  ngOnInit(): void {
    this.http.get('/api/customers/bootstrap').subscribe({
      next: (value) => this.bootstrap.set(JSON.stringify(value, null, 2)),
      error: () => this.bootstrap.set('{"status":"unavailable"}'),
    });
  }
}
