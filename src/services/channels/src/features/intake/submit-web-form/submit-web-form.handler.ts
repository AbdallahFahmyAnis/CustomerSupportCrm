import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { randomUUID } from 'crypto';
import { ChannelsStore } from '../../../persistence/channels.store';
import { DownstreamClient } from '../../../persistence/downstream.client';
import {
  SubmitWebFormCommand,
  SubmitWebFormResult,
} from './submit-web-form.command';
import { validateWebFormInput } from './validate-web-form';

/** SDD CRM-012 / CRM-027 — validate, create customer+ticket, persist request. */
@Injectable()
@CommandHandler(SubmitWebFormCommand)
export class SubmitWebFormHandler
  implements ICommandHandler<SubmitWebFormCommand, SubmitWebFormResult>
{
  constructor(
    private readonly store: ChannelsStore,
    private readonly downstream: DownstreamClient,
  ) {}

  async execute(command: SubmitWebFormCommand): Promise<SubmitWebFormResult> {
    const validationError = validateWebFormInput(command);
    if (validationError) {
      throw new BadRequestException(validationError);
    }

    const name = command.name.trim();
    const email = command.email.trim().toLowerCase();
    const subject = command.subject.trim();
    const message = command.message.trim();

    const customer = await this.downstream.findOrCreateCustomer(name, email);
    const ticket = await this.downstream.createTicket({
      customerId: customer.id,
      customerName: customer.displayName,
      subject,
      description: message,
    });

    const now = new Date().toISOString();
    const requestId = randomUUID();
    this.store.addRequest(
      {
        id: requestId,
        ticketId: ticket.id,
        ticketNumber: ticket.ticketNumber,
        customerId: customer.id,
        email,
        name: customer.displayName,
        subject,
        status: ticket.status ?? 'New',
        createdAt: now,
      },
      {
        id: randomUUID(),
        ticketId: ticket.id,
        channel: 'WebForm',
        direction: 'Inbound',
        body: message,
        fromEmail: email,
        createdAt: now,
      },
    );

    return {
      requestId,
      ticketId: ticket.id,
      ticketNumber: ticket.ticketNumber,
    };
  }
}
