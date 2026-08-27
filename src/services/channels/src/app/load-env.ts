import { config as loadEnv } from 'dotenv';
import { existsSync } from 'fs';
import { resolve } from 'path';

/** Load `.env` before channelsConfig reads process.env. */
function loadChannelEnv(): void {
  const files = [
    resolve(process.cwd(), '../../..', '.env'), // repo root when cwd is src/services/channels
    resolve(process.cwd(), '.env'),
  ];
  for (const file of files) {
    if (existsSync(file)) {
      loadEnv({ path: file, override: false });
    }
  }
}

loadChannelEnv();
