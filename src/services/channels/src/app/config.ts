/** Channels service configuration — SDD CRM-012 / CRM-037. */
export const channelsConfig = {
  port: Number(process.env.PORT ?? 5201),
  dataPath: process.env.CHANNELS_DATA_PATH,
  /** Postgres URL, e.g. postgres://crm:Crm_Local_Pg_2026!@localhost:5432/crm_channels */
  databaseUrl: process.env.CHANNELS_DATABASE_URL,
  customersUrl: (process.env.CUSTOMERS_URL ?? 'http://localhost:5102').replace(
    /\/$/,
    '',
  ),
  ticketsUrl: (process.env.TICKETS_URL ?? 'http://localhost:5103').replace(
    /\/$/,
    '',
  ),
};
