export class ChatCommand {
  constructor(
    public readonly message: string,
    public readonly sessionId?: string,
  ) {}
}
