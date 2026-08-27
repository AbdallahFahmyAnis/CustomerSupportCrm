import { Controller, NotFoundException, Param, Post, Res } from '@nestjs/common';
import type { Response } from 'express';
import { streamSummaryChunks, summarizeTicket } from '../../../infrastructure/ai/heuristic.provider';
import { DownstreamClient } from '../../../infrastructure/http/downstream.client';

/** SDD CRM-023 deferred / 046 — POST /api/ai/tickets/:id/summary/stream (SSE). */
@Controller()
export class StreamSummaryRoute {
  constructor(private readonly downstream: DownstreamClient) {}

  @Post('api/ai/tickets/:id/summary/stream')
  async stream(@Param('id') id: string, @Res() res: Response) {
    const ticket = await this.downstream.getTicket(id);
    if (!ticket) throw new NotFoundException('Ticket not found.');
    const result = summarizeTicket(ticket);
    try {
      await this.downstream.saveAiSummary(ticket.id, result.summary, result.highlights);
    } catch {
      // best-effort
    }

    res.setHeader('Content-Type', 'text/event-stream; charset=utf-8');
    res.setHeader('Cache-Control', 'no-cache');
    res.setHeader('Connection', 'keep-alive');
    res.flushHeaders?.();

    for (const text of streamSummaryChunks(result.summary)) {
      res.write(`data: ${JSON.stringify({ type: 'token', text })}\n\n`);
      await new Promise((r) => setTimeout(r, 15));
    }

    res.write(
      `data: ${JSON.stringify({
        type: 'done',
        ticketId: ticket.id,
        summary: result.summary,
        highlights: result.highlights,
      })}\n\n`,
    );
    res.end();
  }
}
