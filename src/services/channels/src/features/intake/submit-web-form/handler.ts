import { BadRequestException, Injectable } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import {
  SubmitWebFormCommand,
  SubmitWebFormResult,
} from '../intake.types';
import { validateWebFormInput } from './schema';
import { SubmitWebFormService } from './service';

/** SDD CRM-012 / CRM-027 — CQRS handler for web-form intake. */
@Injectable()
@CommandHandler(SubmitWebFormCommand)
export class SubmitWebFormHandler
  implements ICommandHandler<SubmitWebFormCommand, SubmitWebFormResult>
{
  constructor(private readonly service: SubmitWebFormService) {}

  async execute(command: SubmitWebFormCommand): Promise<SubmitWebFormResult> {
    const validationError = validateWebFormInput(command);
    if (validationError) {
      throw new BadRequestException(validationError);
    }
    return this.service.submit(command);
  }
}
