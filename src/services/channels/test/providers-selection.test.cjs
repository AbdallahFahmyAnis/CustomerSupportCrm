const { test, describe } = require('node:test');
const assert = require('node:assert/strict');

const {
  resolveEmailProviderKind,
  resolveSmsProviderKind,
  resolveWhatsAppProviderKind,
} = require('../src/app/config');

/** SDD CRM-040 — provider selection helpers. */
describe('provider selection', () => {
  test('email defaults to dev', () => {
    assert.equal(
      resolveEmailProviderKind({
        sendgridApiKey: '',
        smtpHost: '',
      }),
      'dev',
    );
  });

  test('email prefers sendgrid over smtp', () => {
    assert.equal(
      resolveEmailProviderKind({
        sendgridApiKey: 'SG.x',
        smtpHost: 'smtp.example.com',
      }),
      'sendgrid',
    );
  });

  test('email uses smtp when sendgrid missing', () => {
    assert.equal(
      resolveEmailProviderKind({
        sendgridApiKey: undefined,
        smtpHost: 'smtp.example.com',
      }),
      'smtp',
    );
  });

  test('sms defaults to dev until twilio complete', () => {
    assert.equal(
      resolveSmsProviderKind({
        twilioAccountSid: 'AC',
        twilioAuthToken: '',
        twilioSmsFrom: '+1',
      }),
      'dev',
    );
    assert.equal(
      resolveSmsProviderKind({
        twilioAccountSid: 'ACxxx',
        twilioAuthToken: 'token',
        twilioSmsFrom: '+15551234567',
      }),
      'twilio',
    );
  });

  test('whatsapp requires twilio whatsapp from', () => {
    assert.equal(
      resolveWhatsAppProviderKind({
        twilioAccountSid: 'ACxxx',
        twilioAuthToken: 'token',
        twilioWhatsAppFrom: '',
      }),
      'dev',
    );
    assert.equal(
      resolveWhatsAppProviderKind({
        twilioAccountSid: 'ACxxx',
        twilioAuthToken: 'token',
        twilioWhatsAppFrom: 'whatsapp:+14155238886',
      }),
      'twilio',
    );
  });
});
