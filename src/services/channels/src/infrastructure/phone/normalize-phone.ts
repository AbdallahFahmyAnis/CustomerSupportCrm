/**
 * Normalize phone numbers for Twilio SMS / WhatsApp (E.164 when possible).
 * Egyptian mobiles often stored as 01xxxxxxxxx — convert to +201xxxxxxxxx.
 */
export function normalizePhone(raw: string, defaultCountryCode = '20'): string {
  const trimmed = (raw ?? '').trim().replace(/^whatsapp:/i, '');
  if (!trimmed) {
    return '';
  }

  const hadPlus = trimmed.startsWith('+');
  let digits = trimmed.replace(/\D/g, '');
  if (!digits) {
    return '';
  }

  // Strip trunk international 00…
  if (digits.startsWith('00')) {
    digits = digits.slice(2);
  }

  // Local Egypt mobile: 01xxxxxxxxx (11 digits) → 201xxxxxxxxx
  if (digits.startsWith('0') && digits.length === 11) {
    digits = `${defaultCountryCode}${digits.slice(1)}`;
  }

  // Mistaken +0… / 0 still present after a bad prior normalize
  if (digits.startsWith('0') && digits.length >= 10) {
    digits = `${defaultCountryCode}${digits.replace(/^0+/, '')}`;
  }

  if (hadPlus || digits.length >= 11) {
    return `+${digits}`;
  }

  return digits;
}
