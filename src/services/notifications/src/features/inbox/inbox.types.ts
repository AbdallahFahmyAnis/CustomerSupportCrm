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

/** SDD CRM-016 — create notification from producers. */
export class CreateNotificationCommand {
  constructor(
    public readonly userId: string,
    public readonly title: string,
    public readonly body: string,
    public readonly kind: string,
    public readonly href?: string,
  ) {}
}

