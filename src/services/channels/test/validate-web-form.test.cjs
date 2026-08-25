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

const {
  validateReplyEmailInput,
} = require('../src/features/messages/reply-email/schema');

/** SDD CRM-040 — outbound reply validation. */
test('email reply rejects empty body', () => {
  const err = validateReplyEmailInput('ticket-1', { body: '  ' });
  assert.equal(err, 'body is required.');
});

test('email reply rejects bad to', () => {
  const err = validateReplyEmailInput('ticket-1', {
    body: 'Hello',
    to: 'not-email',
  });
  assert.equal(err, 'to must be a valid email address.');
});

test('email reply accepts valid body', () => {
  const err = validateReplyEmailInput('ticket-1', { body: 'We are on it.' });
  assert.equal(err, null);
});

const {
  validateWhatsAppIngestInput,
  normalizePhone,
} = require('../src/features/intake/ingest-whatsapp/schema');
const {
  validateReplyWhatsAppInput,
} = require('../src/features/messages/reply-whatsapp/schema');

/** SDD CRM-009 — WhatsApp ingest validation. */
test('whatsapp ingest rejects missing fields', () => {
  const err = validateWhatsAppIngestInput({
    from: '',
    body: 'Hi',
  });
  assert.equal(err, 'from (phone) and body are required.');
});

test('whatsapp ingest rejects short phone', () => {
  const err = validateWhatsAppIngestInput({
    from: '123',
    body: 'Hi',
  });
  assert.equal(err, 'from must be a valid phone number.');
});

test('whatsapp ingest accepts valid payload', () => {
  const err = validateWhatsAppIngestInput({
    from: '+15551234567',
    body: 'Need help with my order',
    name: 'Ada',
  });
  assert.equal(err, null);
  assert.equal(normalizePhone('+1 (555) 123-4567'), '+15551234567');
});

/** SDD CRM-009 — WhatsApp reply validation. */
test('whatsapp reply rejects empty body', () => {
  const err = validateReplyWhatsAppInput('ticket-1', { body: '  ' });
  assert.equal(err, 'body is required.');
});

test('whatsapp reply rejects bad to', () => {
  const err = validateReplyWhatsAppInput('ticket-1', {
    body: 'Hello',
    to: '12',
  });
  assert.equal(err, 'to must be a valid phone number.');
});

test('whatsapp reply accepts valid body', () => {
  const err = validateReplyWhatsAppInput('ticket-1', { body: 'We are on it.' });
  assert.equal(err, null);
});

const {
  validateChatIngestInput,
} = require('../src/features/intake/ingest-chat/schema');
const {
  validateReplyChatInput,
} = require('../src/features/messages/reply-chat/schema');

/** SDD CRM-010 — live chat ingest validation. */
test('chat ingest rejects missing fields', () => {
  const err = validateChatIngestInput({
    email: '',
    body: '',
  });
  assert.equal(err, 'email and body are required.');
});

test('chat ingest rejects bad email', () => {
  const err = validateChatIngestInput({
    email: 'not-an-email',
    body: 'Hello',
  });
  assert.equal(err, 'email must be a valid address.');
});

test('chat ingest accepts valid payload', () => {
  const err = validateChatIngestInput({
    email: 'visitor@example.com',
    body: 'Need help with billing.',
  });
  assert.equal(err, null);
});

/** SDD CRM-010 — live chat reply validation. */
test('chat reply rejects empty body', () => {
  const err = validateReplyChatInput('ticket-1', { body: '  ' });
  assert.equal(err, 'body is required.');
});

test('chat reply rejects bad to', () => {
  const err = validateReplyChatInput('ticket-1', {
    body: 'Hello',
    to: 'not-email',
  });
  assert.equal(err, 'to must be a valid email address.');
});

test('chat reply accepts valid body', () => {
  const err = validateReplyChatInput('ticket-1', { body: 'We are on it.' });
  assert.equal(err, null);
});

const {
  validateSmsIngestInput,
  normalizePhone: normalizeSmsPhone,
} = require('../src/features/intake/ingest-sms/schema');
const {
  validateReplySmsInput,
} = require('../src/features/messages/reply-sms/schema');

/** SDD CRM-011 — SMS ingest validation. */
test('sms ingest rejects missing fields', () => {
  const err = validateSmsIngestInput({
    from: '',
    body: 'Hi',
  });
  assert.equal(err, 'from (phone) and body are required.');
});

test('sms ingest rejects short phone', () => {
  const err = validateSmsIngestInput({
    from: '123',
    body: 'Hi',
  });
  assert.equal(err, 'from must be a valid phone number.');
});

test('sms ingest accepts valid payload', () => {
  const err = validateSmsIngestInput({
    from: '+15559876543',
    body: 'Need a callback',
    name: 'Sam',
  });
  assert.equal(err, null);
  assert.equal(normalizeSmsPhone('+1 (555) 987-6543'), '+15559876543');
});

/** SDD CRM-011 — SMS reply validation. */
test('sms reply rejects empty body', () => {
  const err = validateReplySmsInput('ticket-1', { body: '  ' });
  assert.equal(err, 'body is required.');
});

test('sms reply rejects bad to', () => {
  const err = validateReplySmsInput('ticket-1', {
    body: 'Hello',
    to: '12',
  });
  assert.equal(err, 'to must be a valid phone number.');
});

test('sms reply accepts valid body', () => {
  const err = validateReplySmsInput('ticket-1', { body: 'We are on it.' });
  assert.equal(err, null);
});
