/** Channels service configuration — SDD CRM-012. */
export const channelsConfig = {
  port: Number(process.env.PORT ?? 5201),
  dataPath: process.env.CHANNELS_DATA_PATH,
  customersUrl: (process.env.CUSTOMERS_URL ?? 'http://localhost:5102').replace(
    /\/$/,
    '',
  ),
  ticketsUrl: (process.env.TICKETS_URL ?? 'http://localhost:5103').replace(
    /\/$/,
    '',
  ),
};
