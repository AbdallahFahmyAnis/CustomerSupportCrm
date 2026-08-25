/** SDD CRM-020 — inbox CQRS types. */
export class ListInboxQuery {
  constructor(public readonly userId: string) {}
}

export class UnreadCountQuery {
  constructor(public readonly userId: string) {}
}

export class MarkReadCommand {
  constructor(
    public readonly userId: string,
    public readonly id: string,
  ) {}
}
