const assert = require('node:assert/strict');
const test = require('node:test');
const {
  validateWebFormInput,
} = require('../src/features/intake/submit-web-form/validate-web-form');

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
