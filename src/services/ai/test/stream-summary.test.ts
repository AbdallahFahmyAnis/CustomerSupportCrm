import assert from 'node:assert/strict';
import { describe, it } from 'node:test';
import { streamSummaryChunks } from '../src/infrastructure/ai/heuristic.provider';

describe('CRM-023 streamSummaryChunks', () => {
  it('splits summary into ordered token chunks', () => {
    const chunks = streamSummaryChunks('TKT-1: Hello world from streaming summary test here now', 3);
    assert.ok(chunks.length >= 2);
    assert.equal(chunks.map((c) => c.trim()).join(' '), 'TKT-1: Hello world from streaming summary test here now');
  });

  it('returns empty for blank summary', () => {
    assert.deepEqual(streamSummaryChunks('   '), []);
  });
});
