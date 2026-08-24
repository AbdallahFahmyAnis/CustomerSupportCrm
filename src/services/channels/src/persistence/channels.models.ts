/** SDD CRM-012 / CRM-027 / CRM-028 — Channels local store types. */
export interface PortalRequest {
  id: string;
  ticketId: string;
  ticketNumber: string;
  customerId: string;
  email: string;
  name: string;
  subject: string;
  status: string;
  createdAt: string;
}

export interface ChannelMessage {
  id: string;
  ticketId: string;
  channel: 'WebForm';
  direction: 'Inbound';
  body: string;
  fromEmail: string;
  createdAt: string;
}

export interface ChannelsStoreData {
  requests: PortalRequest[];
  messages: ChannelMessage[];
}
