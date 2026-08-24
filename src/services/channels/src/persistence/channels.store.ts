import { existsSync, mkdirSync, readFileSync, writeFileSync } from 'fs';
import { dirname, join } from 'path';
import { Injectable, OnModuleInit } from '@nestjs/common';
import {
  ChannelMessage,
  ChannelsStoreData,
  PortalRequest,
} from './channels.models';

/** SDD CRM-012 — JSON file persistence for portal requests + messages. */
@Injectable()
export class ChannelsStore implements OnModuleInit {
  private readonly filePath =
    process.env.CHANNELS_DATA_PATH ??
    join(process.cwd(), 'data', 'channels-store.json');

  private data: ChannelsStoreData = { requests: [], messages: [] };

  onModuleInit(): void {
    this.load();
    this.seedIfEmpty();
  }

  listRequestsByEmail(email: string): PortalRequest[] {
    const key = email.trim().toLowerCase();
    return this.data.requests
      .filter((r) => r.email === key)
      .sort((a, b) => b.createdAt.localeCompare(a.createdAt));
  }

  listMessagesForTicket(ticketId: string): ChannelMessage[] {
    return this.data.messages
      .filter((m) => m.ticketId === ticketId)
      .sort((a, b) => a.createdAt.localeCompare(b.createdAt));
  }

  addRequest(request: PortalRequest, message: ChannelMessage): void {
    this.data.requests.push(request);
    this.data.messages.push(message);
    this.save();
  }

  private load(): void {
    try {
      if (!existsSync(this.filePath)) {
        return;
      }
      const raw = readFileSync(this.filePath, 'utf8');
      const parsed = JSON.parse(raw) as ChannelsStoreData;
      this.data = {
        requests: parsed.requests ?? [],
        messages: parsed.messages ?? [],
      };
    } catch {
      this.data = { requests: [], messages: [] };
    }
  }

  private save(): void {
    mkdirSync(dirname(this.filePath), { recursive: true });
    writeFileSync(this.filePath, JSON.stringify(this.data, null, 2), 'utf8');
  }

  private seedIfEmpty(): void {
    if (this.data.requests.length > 0) {
      return;
    }

    const createdAt = new Date().toISOString();
    const request: PortalRequest = {
      id: 'seed-portal-request-001',
      ticketId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
      ticketNumber: 'SEED-PORTAL',
      customerId: 'seed-portal-customer',
      email: 'portal.customer@example.com',
      name: 'Portal Customer',
      subject: 'Seeded portal request (demo)',
      status: 'New',
      createdAt,
    };
    const message: ChannelMessage = {
      id: 'seed-portal-message-001',
      ticketId: request.ticketId,
      channel: 'WebForm',
      direction: 'Inbound',
      body: 'This is a seeded web-form message for Track demo when Tickets is empty.',
      fromEmail: request.email,
      createdAt,
    };
    this.data.requests.push(request);
    this.data.messages.push(message);
    this.save();
  }
}
