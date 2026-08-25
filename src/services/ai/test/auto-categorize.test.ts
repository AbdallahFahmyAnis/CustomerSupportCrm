import assert from 'node:assert/strict';
import { describe, it } from 'node:test';
import { categorizeTicket } from '../src/infrastructure/ai/heuristic.provider';

describe('CRM-025 categorizeTicket', () => {
  it('maps invoice language to Billing and Urgent when needed', () => {
    const row = categorizeTicket({
      id: '1',
      ticketNumber: 'TKT-3',
      subject: 'Urgent invoice refund',
      description: 'Wrong payment charged asap',
      category: 'General',
      priority: 'Medium',
      status: 'New',
      customerName: 'Gamma',
    });
    assert.equal(row.category, 'Billing');
    assert.equal(row.priority, 'Urgent');
    assert.ok(row.confidence >= 0.8);
  });
});
