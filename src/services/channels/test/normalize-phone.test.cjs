const { describe, it } = require('node:test');
const assert = require('node:assert/strict');

/** Mirror of normalize-phone.ts for cjs tests (ts-node loads .ts in app). */
function normalizePhone(raw, defaultCountryCode = '20') {
  const trimmed = String(raw ?? '')
    .trim()
    .replace(/^whatsapp:/i, '');
  if (!trimmed) return '';
  const hadPlus = trimmed.startsWith('+');
  let digits = trimmed.replace(/\D/g, '');
  if (!digits) return '';
  if (digits.startsWith('00')) digits = digits.slice(2);
  if (digits.startsWith('0') && digits.length === 11) {
    digits = `${defaultCountryCode}${digits.slice(1)}`;
  }
  if (digits.startsWith('0') && digits.length >= 10) {
    digits = `${defaultCountryCode}${digits.replace(/^0+/, '')}`;
  }
  if (hadPlus || digits.length >= 11) return `+${digits}`;
  return digits;
}

describe('normalizePhone', () => {
  it('converts Egyptian local mobile to E.164', () => {
    assert.equal(normalizePhone('01090205591'), '+201090205591');
    assert.equal(normalizePhone('+20 10 90205591'), '+201090205591');
  });
});
