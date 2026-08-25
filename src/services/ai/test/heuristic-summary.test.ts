import assert from 'node:assert/strict';
import { describe, it } from 'node:test';
import { summarizeTicket } from '../src/infrastructure/ai/heuristic.provider';

describe('CRM-023 summarizeTicket', () => {
  it('builds summary and highlights from ticket fields', () => {
    const result = summarizeTicket({
      id: '1',
      ticketNumber: 'TKT-1001',
      subject: 'Invoice mismatch',
      description: 'Line items do not match the PO. Please review.',
      category: 'Billing',
      priority: 'High',
      status: 'New',
      customerName: 'Acme',
    });
    assert.match(result.summary, /TKT-1001/);
    assert.match(result.summary, /Invoice mismatch/);
    assert.ok(result.highlights.some((h) => h.includes('Billing')));
    assert.ok(result.highlights.some((h) => h.includes('High')));
  });
});
