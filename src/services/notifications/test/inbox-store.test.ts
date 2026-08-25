import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import test from 'node:test';
import { NotificationsStore } from '../src/infrastructure/database/notifications.store';
import { DEMO_AGENT_ID } from '../src/domain/notification';

/** SDD CRM-020 — inbox store behaviour. */
test('CRM-020 seed lists unread for demo agent and mark-read works', () => {
  const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'crm-notif-'));
  const store = new NotificationsStore(dir);
  store.ensureSeeded();

  const list = store.listForUser(DEMO_AGENT_ID);
  assert.ok(list.length >= 2);
  const unreadBefore = store.unreadCount(DEMO_AGENT_ID);
  assert.ok(unreadBefore >= 2);

  const other = store.listForUser('00000000-0000-0000-0000-000000000000');
  assert.equal(other.length, 0);

  const target = list.find((n) => !n.readAt);
  assert.ok(target);
  const marked = store.markRead(DEMO_AGENT_ID, target!.id);
  assert.ok(marked?.readAt);
  assert.equal(store.unreadCount(DEMO_AGENT_ID), unreadBefore - 1);

  assert.equal(store.markRead(DEMO_AGENT_ID, 'missing-id'), null);
});
