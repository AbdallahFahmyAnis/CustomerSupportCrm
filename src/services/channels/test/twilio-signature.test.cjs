'use strict';

const { createHmac } = require('crypto');
const assert = require('node:assert/strict');
const { test } = require('node:test');
const {
  validateTwilioSignature,
  buildTwilioWebhookUrl,
} = require('../src/infrastructure/twilio/validate-twilio-signature');
const {
  parseTwilioInboundForm,
} = require('../src/infrastructure/twilio/parse-twilio-inbound');

/** SDD CRM-040 — Twilio signature helper. */
function sign(authToken, url, params) {
  const data =
    url +
    Object.keys(params)
      .sort()
      .map((k) => k + params[k])
      .join('');
  return createHmac('sha1', authToken).update(Buffer.from(data, 'utf8')).digest('base64');
}

test('CRM-040 bypasses verification when auth token is empty', () => {
  const result = validateTwilioSignature({
    authToken: '',
    url: 'http://localhost:5000/api/channels/webhooks/twilio/sms',
    params: { From: '+15551234567', Body: 'hello' },
    signature: '',
  });
  assert.equal(result.ok, true);
  assert.equal(result.bypassed, true);
});

test('CRM-040 rejects missing or bad signature when token set', () => {
  const url = 'http://localhost:5000/api/channels/webhooks/twilio/sms';
  const params = { From: '+15551234567', Body: 'hello' };
  const missing = validateTwilioSignature({
    authToken: 'secret',
    url,
    params,
    signature: null,
  });
  assert.equal(missing.ok, false);

  const bad = validateTwilioSignature({
    authToken: 'secret',
    url,
    params,
    signature: 'not-valid',
  });
  assert.equal(bad.ok, false);
});

test('CRM-040 accepts matching Twilio signature', () => {
  const url = buildTwilioWebhookUrl(
    'http://localhost:5000',
    '/api/channels/webhooks/twilio/sms',
  );
  const params = { From: '+15551234567', Body: 'Need help' };
  const token = 'test-auth-token';
  const signature = sign(token, url, params);
  const result = validateTwilioSignature({
    authToken: token,
    url,
    params,
    signature,
  });
  assert.equal(result.ok, true);
  assert.equal(result.bypassed, false);
});

test('CRM-040 parses Twilio form From/Body and strips whatsapp prefix', () => {
  const sms = parseTwilioInboundForm({ From: '+15550001111', Body: 'hi' });
  assert.equal(sms.from, '+15550001111');
  assert.equal(sms.body, 'hi');

  const wa = parseTwilioInboundForm({
    From: 'whatsapp:+15550002222',
    Body: 'yo',
  });
  assert.equal(wa.from, '+15550002222');
  assert.equal(wa.body, 'yo');
});
