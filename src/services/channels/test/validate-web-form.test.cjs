const assert = require('node:assert/strict');
const test = require('node:test');
const {
  validateWebFormInput,
} = require('../src/features/intake/submit-web-form/schema');
const {
  validateEmailIngestInput,
} = require('../src/features/intake/ingest-email/schema');

/** SDD CRM-012 / CRM-027 — intake validation. */
test('rejects missing fields', () => {
  const err = validateWebFormInput({
    name: '',
    email: 'a@b.com',
    subject: 'Hi',
    message: 'Body',
  });
  assert.equal(err, 'name, email, subject, and message are required.');
});

test('rejects invalid email', () => {
  const err = validateWebFormInput({
    name: 'Ada',
    email: 'not-an-email',
    subject: 'Hi',
    message: 'Body',
  });
  assert.equal(err, 'email must be a valid address.');
});

test('accepts valid payload', () => {
  const err = validateWebFormInput({
    name: 'Ada',
    email: 'ada@example.com',
    subject: 'Need help',
    message: 'Printer is down',
  });
  assert.equal(err, null);
});

/** SDD CRM-008 — email ingest validation. */
test('email ingest rejects missing fields', () => {
  const err = validateEmailIngestInput({
    from: '',
    subject: 'Hi',
    body: 'Body',
  });
  assert.equal(err, 'from, subject, and body are required.');
});

test('email ingest rejects invalid from', () => {
  const err = validateEmailIngestInput({
    from: 'nope',
    subject: 'Hi',
    body: 'Body',
  });
  assert.equal(err, 'from must be a valid email address.');
});

test('email ingest accepts valid payload', () => {
  const err = validateEmailIngestInput({
    from: 'customer@example.com',
    subject: 'Billing question',
    body: 'Please explain my invoice.',
  });
  assert.equal(err, null);
});
