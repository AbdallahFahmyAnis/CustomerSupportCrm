import { Injectable, Logger, OnModuleInit } from '@nestjs/common';
import { DataSource, Repository } from 'typeorm';
import { existsSync, mkdirSync, readFileSync, writeFileSync } from 'fs';
import { dirname, join } from 'path';
import {
  ChannelMessage,
  ChannelsStoreData,
  PortalRequest,
} from '../../domain/channels';
import { channelsConfig } from '../../app/config';
import {
  ChannelMessageEntity,
  PortalRequestEntity,
} from './channels.entities';

/**
 * SDD CRM-012 / CRM-037 — Postgres (TypeORM) when CHANNELS_DATABASE_URL is set;
 * JSON file escape hatch otherwise (offline tests).
 */
@Injectable()
export class ChannelsStore implements OnModuleInit {
  private readonly logger = new Logger(ChannelsStore.name);
  private readonly filePath =
    channelsConfig.dataPath ?? join(process.cwd(), 'data', 'channels-store.json');

  private data: ChannelsStoreData = { requests: [], messages: [] };
  private dataSource: DataSource | null = null;
  private requestsRepo: Repository<PortalRequestEntity> | null = null;
  private messagesRepo: Repository<ChannelMessageEntity> | null = null;

  async onModuleInit(): Promise<void> {
    if (channelsConfig.databaseUrl) {
      await this.initPostgres();
      return;
    }

    this.loadJson();
    this.seedJsonIfEmpty();
  }

  private async initPostgres(): Promise<void> {
    try {
      this.dataSource = new DataSource({
        type: 'postgres',
        url: channelsConfig.databaseUrl,
        entities: [PortalRequestEntity, ChannelMessageEntity],
        synchronize: true,
      });
      await this.dataSource.initialize();
      this.requestsRepo = this.dataSource.getRepository(PortalRequestEntity);
      this.messagesRepo = this.dataSource.getRepository(ChannelMessageEntity);
      await this.seedPostgresIfEmpty();
      this.logger.log('Channels store using PostgreSQL (TypeORM).');
    } catch (err) {
      this.logger.error(
        `Postgres init failed; falling back to JSON. ${String(err)}`,
      );
      this.dataSource = null;
      this.loadJson();
      this.seedJsonIfEmpty();
    }
  }

  async listRequestsByEmail(email: string): Promise<PortalRequest[]> {
    const key = email.trim().toLowerCase();
    if (this.requestsRepo) {
      const rows = await this.requestsRepo.find({
        where: { email: key },
        order: { createdAt: 'DESC' },
      });
      return rows.map((r) => this.toRequest(r));
    }

    return this.data.requests
      .filter((r) => r.email.trim().toLowerCase() === key)
      .sort((a, b) => b.createdAt.localeCompare(a.createdAt));
  }

  async listMessagesForTicket(ticketId: string): Promise<ChannelMessage[]> {
    if (this.messagesRepo) {
      const rows = await this.messagesRepo.find({
        where: { ticketId },
        order: { createdAt: 'ASC' },
      });
      return rows.map((m) => this.toMessage(m));
    }

    return this.data.messages
      .filter((m) => m.ticketId === ticketId)
      .sort((a, b) => a.createdAt.localeCompare(b.createdAt));
  }

  async addRequest(request: PortalRequest, message: ChannelMessage): Promise<void> {
    if (this.requestsRepo && this.messagesRepo) {
      await this.requestsRepo.save(request);
      await this.messagesRepo.save({
        ...message,
        fromEmail: message.fromEmail ?? null,
      });
      return;
    }

    this.data.requests.push(request);
    this.data.messages.push(message);
    this.saveJson();
  }

  async addMessage(message: ChannelMessage): Promise<void> {
    if (this.messagesRepo) {
      await this.messagesRepo.save({
        ...message,
        fromEmail: message.fromEmail ?? null,
      });
      return;
    }

    this.data.messages.push(message);
    this.saveJson();
  }

  async findRequestByTicketId(ticketId: string): Promise<PortalRequest | null> {
    if (this.requestsRepo) {
      const row = await this.requestsRepo.findOne({ where: { ticketId } });
      return row ? this.toRequest(row) : null;
    }

    return this.data.requests.find((r) => r.ticketId === ticketId) ?? null;
  }

  private toRequest(r: PortalRequestEntity): PortalRequest {
    return {
      id: r.id,
      ticketId: r.ticketId,
      ticketNumber: r.ticketNumber,
      customerId: r.customerId,
      email: r.email,
      name: r.name,
      subject: r.subject,
      status: r.status,
      createdAt: r.createdAt,
    };
  }

  private toMessage(m: ChannelMessageEntity): ChannelMessage {
    return {
      id: m.id,
      ticketId: m.ticketId,
      channel: m.channel as ChannelMessage['channel'],
      direction: m.direction as ChannelMessage['direction'],
      body: m.body,
      fromEmail: m.fromEmail ?? '',
      createdAt: m.createdAt,
    };
  }

  private loadJson(): void {
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

  private saveJson(): void {
    mkdirSync(dirname(this.filePath), { recursive: true });
    writeFileSync(this.filePath, JSON.stringify(this.data, null, 2), 'utf8');
  }

  private seedPayload(): { request: PortalRequest; message: ChannelMessage } {
    const createdAt = new Date().toISOString();
    const request: PortalRequest = {
      id: 'seed-portal-request-001',
      ticketId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
      ticketNumber: 'SEED-PORTAL',
      customerId: 'seed-portal-customer',
      email: 'customer@crm.local',
      name: 'Demo Customer',
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
    return { request, message };
  }

  private seedJsonIfEmpty(): void {
    if (this.data.requests.length === 0) {
      const { request, message } = this.seedPayload();
      this.data.requests.push(request);
      this.data.messages.push(message);
      this.saveJson();
    }
    this.ensureDemoCustomerJson();
  }

  /** Ensure demo customer@crm.local has at least one trackable request. */
  private ensureDemoCustomerJson(): void {
    const email = 'customer@crm.local';
    if (this.data.requests.some((r) => r.email.toLowerCase() === email)) {
      return;
    }
    const { request, message } = this.seedPayload();
    this.data.requests.push(request);
    this.data.messages.push(message);
    this.saveJson();
  }

  private async seedPostgresIfEmpty(): Promise<void> {
    if (!this.requestsRepo || !this.messagesRepo) {
      return;
    }
    const count = await this.requestsRepo.count();
    if (count === 0) {
      const { request, message } = this.seedPayload();
      await this.addRequest(request, message);
      return;
    }
    await this.ensureDemoCustomerPostgres();
  }

  private async ensureDemoCustomerPostgres(): Promise<void> {
    if (!this.requestsRepo || !this.messagesRepo) {
      return;
    }
    const email = 'customer@crm.local';
    const existing = await this.requestsRepo.count({ where: { email } });
    if (existing > 0) {
      return;
    }
    const { request, message } = this.seedPayload();
    await this.addRequest(request, message);
  }
}
